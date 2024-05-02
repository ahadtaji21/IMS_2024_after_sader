using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using jrs.Classes;
using jrs.DBContexts;
using jrs.Models;
using jrs.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;


namespace jrs.Controllers
{

    /// <summary>
    /// Controller class for all the Document Template operations.
    /// </summary>
    [ApiController]
    [Consumes("application/json")]
    public class ImsTemplateController : ControllerBase
    {
        private readonly GeneralContext _context;
        private readonly IMSLogContext _logContext;
        private readonly IConfiguration Configuration;
        private string UserId = "";
        private string IP = "";
        ImsUtility utility = null;

        /// <summary>
        /// Constructor for the "ImsTemplateController" class.
        /// </summary>
        public ImsTemplateController(GeneralContext context, IMSLogContext logContext, IConfiguration configuration)
        {
            _context = context;
            _logContext = logContext;
            utility = new ImsUtility();
            Configuration = configuration;
        }

        // GET: api/template
        [HttpGet("api/template")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ImsTemplate>>> GetImsTemplate(int officeId, int? templateId, int? templateTypeId, string templateTypeCode)
        {
            List<ImsTemplate> templates = new List<ImsTemplate>();

            // Get Templates for office
            ImsTemplate[] tmp = await _context.ImsTemplate
                .Where(template => template.TemplateOfficeList.Any(to => to.OfficeId == officeId && !to.IsDeleted))
                .Include("TemplateType.TemplateParams")
                .Include("TemplateOfficeList")
                .ToArrayAsync();

            // Filter based on params
            if (templateId.HasValue)
            {
                tmp = tmp.Where(template => template.TemplateId == templateId).ToArray();
            }
            if (templateTypeId.HasValue)
            {
                tmp = tmp.Where(template => template.TemplateTypeId == templateTypeId).ToArray();
            }
            if (!String.IsNullOrEmpty(templateTypeCode))
            {
                tmp = tmp.Where(template => template.TemplateType.TemplateTypeCode == templateTypeCode).ToArray();
            }

            // Convert and return
            templates = tmp.ToList();
            return templates;
        }

        // GET: api/template-types
        [HttpGet("api/template-types")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ImsTemplateType>>> GetImsTemplateTypes()
        {
            List<ImsTemplateType> templateTypes = await _context.ImsTemplateType
                .Include("TemplateParams")
                .ToListAsync();
            return templateTypes;
        }

        /// <summary>Get ImsTemplateType for the provided template id.</summary>
        // GET: api/template-type-for-template/1
        [HttpGet("api/template-type-for-template/{templateId}")]
        [Authorize]
        public async Task<ActionResult<ImsTemplateType>> GetImsTemplateTypeForTemplate(int templateId)
        {
            ImsTemplate template = await _context.ImsTemplate
                .Where(template => template.TemplateId == templateId)
                .Include("TemplateType")
                .SingleOrDefaultAsync();
            return template.TemplateType;
        }

        // GET: api/template-param
        [HttpGet("api/template-param-for-type")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ImsTemplateParams>>> GetImsTemplateParamForType(int templateType)
        {
            List<ImsTemplateParams> templateParams = await _context.ImsTemplateParams
                .Where(param => param.TemplateTypeId == templateType)
                .ToListAsync();
            return templateParams;
        }

        ///<summary>
        /// Generate an OpenXml Word document based on the defined text in an IMS_TEMPLATE replacing data with a specific query condition.
        ///</summary>
        ///<returns>Byte array representing the generate OpenXml Word document.</returns>
        /// <param name="templateId">ID of the IMS_TEMPLATE to use for generating the document document.</param>
        /// <param name="queryCondition">Query string to apply to the template data query.</param>
        // GET: api/merged-text
        [HttpGet("api/merged-text")]
        [Authorize]
        public async Task<IActionResult> MergedText(int templateId, string queryCondition)
        {
            string documentText;

            // Get Template
            List<ImsTemplate> templates = await _context.ImsTemplate.Where(t => t.TemplateId == templateId)
                .Include("TemplateType.TemplateParams")
                .ToListAsync();

            if (templates.Count == 0)
            {
                return NotFound();
            }
            ImsTemplate template = templates[0];

            // Get data to replace
            try
            {
                IP = utility.GetIP(Response);
                UserId = User.Identity.Name;

                // Setup DB connection params
                string connectionString = Configuration.GetConnectionString("jrsdb");
                string dataQueryCondition = queryCondition ?? "1=1";
                string queryString = $"{template.TemplateType.TemplateDataQuery} WHERE {dataQueryCondition}";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Call Read  on reader before accessing data
                    reader.Read();
                    IDataRecord record = (IDataRecord)reader;

                    // Setup document text
                    documentText = template.TemplateText;
                    foreach (ImsTemplateParams param in template.TemplateType.TemplateParams)
                    {
                        string columnValue = record[param.TemplateParamColumnName].ToString();
                        documentText = documentText.Replace($"##{param.TemplateParamColumnName}##", columnValue);
                    }

                    // Close reader when done reading
                    reader.Close();

                    // Generate .docx
                    DocCreatorService docCreator = new DocCreatorService();
                    Byte[] retBytes = docCreator.CreateDocxFromHtml(documentText).ToArray();

                    // Return Byte array
                    return StatusCode(200, retBytes);
                }
            }
            catch (Exception ex)
            {
                //@TODO: Implementare Log
                ImsLogerrors evt = utility.GetLogError("GenericSqlController", "GenericSelectStructure", ex, UserId, new { templateId = templateId, queryCondition = queryCondition }, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
                return StatusCode(500);
            }
        }

        ///<summary>
        /// Generate an OpenXml Excel document based on the defined data query and column labels.
        ///</summary>
        ///<returns>Byte array representing the generate OpenXml Excel document.</returns>
        /// <param name="exelSelectPayload">Parameters including query information for data and column label translations.</param>
        // GET: api/create-excel-for-table
        [HttpPost("api/create-excel-for-table")]
        [Authorize]
        public async Task<IActionResult> CreateAndDownloadExcelForTable([FromBody] GenericExelSelectPayload exelSelectPayload)
        {
            try
            {
                IP = utility.GetIP(Response);
                UserId = User.Identity.Name;

                // Query data
                GenericSqlSelectPayload queryPayload = exelSelectPayload;
                DBQueryService dBQuery = new DBQueryService();
                var jsonData = await dBQuery.GenericQueryTableStructureAndData(_context, queryPayload);

                // Create excel document
                DocCreatorService docCreator = new DocCreatorService();
                Byte[] retBytes = docCreator.CreateExcel(jsonData.ToString(), exelSelectPayload.columnLabels).ToArray();

                // Return Byte array
                return StatusCode(200, retBytes);
            }
            catch (Exception ex)
            {
                ImsLogerrors evt = utility.GetLogError("CreateAndDownloadExcelForTable", "CreateAndDownloadExcelForTable", ex, UserId, new { exelSelectPayload = exelSelectPayload }, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
                return StatusCode(500);
            }
        }


        [HttpPost("api/create-excel-for-overview-all-indicators")]
        [Authorize]
        public async Task<IActionResult> CreateAndDownloadExcelForTableOvwInd([FromBody] string exelSelectPayloadStr)
        {
            try
            {
                IP = utility.GetIP(Response);
                UserId = User.Identity.Name; GenericExelSelectPayload[] exelSelectPayload = JsonConvert.DeserializeObject<GenericExelSelectPayload[]>(exelSelectPayloadStr);


                List<ExcelStructure> strPayLoad = new List<ExcelStructure>();
                for (int i = 0; i < exelSelectPayload.Length; i++)
                {
                    DBQueryService dBQuery = new DBQueryService();
                    GenericExelSelectPayload selPayLoad = exelSelectPayload[i];
                    var pyld = new ExcelStructure();
                    var jsonData = await dBQuery.GenericQueryTableStructureAndData(_context, selPayLoad);
                    pyld.jsonTableInfo = jsonData.ToString();
                    pyld.columnLabels = selPayLoad.columnLabels;
                    pyld.sheetName = selPayLoad.sheetName;
                    strPayLoad.Add(pyld);

                }
                // Query data

                // Create excel document
                DocCreatorService docCreator = new DocCreatorService();
                Byte[] retBytes = docCreator.CreateExcelOvwInd(strPayLoad.ToArray()).ToArray();

                // Return Byte array
                return StatusCode(200, retBytes);
            }
            catch (Exception ex)
            {
                ImsLogerrors evt = utility.GetLogError("CreateAndDownloadExcelForTableOvwInd", "CreateAndDownloadExcelForTableOvwInd", ex, UserId, new { exelSelectPayload = exelSelectPayloadStr }, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
                return StatusCode(500);
            }
        }


        [HttpPost("api/create-excel-for-overview-indicators")]
        [Authorize]
        public async Task<IActionResult> CreateAndDownloadExcelForOverviewIndicators([FromBody] GenericExelSelectPayload exelSelectPayload)
        {
            try
            {
                IP = utility.GetIP(Response);
                UserId = User.Identity.Name;
                OutpInd[] rows = JsonConvert.DeserializeObject<OutpInd[]>(exelSelectPayload.bodyValues);

                // // Query data
                // GenericSqlSelectPayload queryPayload = exelSelectPayload;
                // DBQueryService dBQuery = new DBQueryService ();
                // var jsonData = await dBQuery.GenericQueryTableStructureAndData (_context, queryPayload);

                // Create excel document
                DocCreatorService docCreator = new DocCreatorService();
                Byte[] retBytes = docCreator.CreateExcelForOverviewOutputIndicators(exelSelectPayload.columnLabels, rows).ToArray();

                // Return Byte array
                return StatusCode(200, retBytes);
            }
            catch (Exception ex)
            {
                ImsLogerrors evt = utility.GetLogError("CreateAndDownloadExcelForOverviewIndicators", "CreateAndDownloadExcelForOverviewIndicators", ex, UserId, new { exelSelectPayload = exelSelectPayload }, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
                return StatusCode(500);
            }
        }

        ///<summary>
        /// Generate an OpenXml Excel document based on the structure and results of Questionnaires.
        ///</summary>
        ///<returns>Byte array representing the generate OpenXml Excel document.</returns>
        ///<param name="questionnaireId">Id of the questionnaire to get results for.</param>
        ///<param name="exelSelectPayload">Excel query condition object. NOTE: Only "conditionRules" and "columnLabels" will be considered.</param>
        // POST: api/create-excel-for-questionniare
        [HttpPost("api/create-excel-for-questionniare/{questionnaireId}", Name = "Download questionnaire results")]
        [Authorize]
        // public async Task<IActionResult> CreateAndDownloadExcelForQuestionnaire(int questionnaireId, [FromBody] GenericConditionRule[]? conditionRules){
        public async Task<IActionResult> CreateAndDownloadExcelForQuestionnaire(int questionnaireId, [FromBody] GenericExelSelectPayload exelSelectPayload)
        {
            try
            {
                GenericConditionRule[] conditionRules = exelSelectPayload.conditionRules;
                GenericExcelColumnLabel[] columnLabels = exelSelectPayload.columnLabels;

                IP = utility.GetIP(Response);
                UserId = User.Identity.Name;

                // Get Field defintion and results
                DBQueryService dBQuery = new DBQueryService();
                var jsonData = await dBQuery.GenericQuestionnaireStructureAndData(_context, questionnaireId, conditionRules ?? null);

                // Create excel document
                DocCreatorService docCreator = new DocCreatorService();
                Byte[] retBytes = docCreator.CreateExcel(jsonData.ToString(), columnLabels ?? null).ToArray();

                // Return Byte array
                return StatusCode(200, retBytes);

                // return StatusCode(500);
            }
            catch (Exception ex)
            {
                ImsLogerrors evt = utility.GetLogError("GenericSqlController", "GenericSelectStructure", ex, UserId, new { questionnaireId = questionnaireId }, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
                return StatusCode(500);
            }
        }

        ///<summary>
        /// Generate an OpenXml Excel document based on stored procedure.
        ///</summary>
        ///<returns>Byte array representing the generate OpenXml Excel document.</returns>
        /// <param name="exelSelectPayload">
        //Parameters including PROCEDURE_NAME and ProcedureParameter[] PROCEDURE_PARAMETERS.
        //ProcedureParameter[] PROCEDURE_PARAMETERS: {name, value}
        //</param>
        // GET: api/create-excel-SP
        [HttpPost("api/create-excel-SP")]
        [Authorize]
        public async Task<IActionResult> CreateAndDownloadExcelFromSP([FromBody] ExcelStoredProcedurePayload exelSelectPayload)
        {
            try
            {
                IP = utility.GetIP(Response);
                UserId = User.Identity.Name;

                JObject result = new JObject();


                // Query data
                ExcelStoredProcedurePayload queryPayload = exelSelectPayload;
                queryPayload.SQL_PROCEDURE_PARAMETERS = new SqlProcedureParameter[queryPayload.PROCEDURE_PARAMETERS.Length];

                for (int i = 0; i < queryPayload.PROCEDURE_PARAMETERS.Length; i++)
                {
                    var item = queryPayload.PROCEDURE_PARAMETERS[i];

                    SqlProcedureParameter sqlParam = new SqlProcedureParameter();
                    sqlParam.name = item.name;
                    switch (item.type)
                    {
                        case "number": sqlParam.value = Int32.Parse(item.value.ToString()); break;
                        case "text": sqlParam.value = item.value.ToString(); break;
                        case "date": sqlParam.value = DateTime.Parse(item.value.ToString()); break;
                        default: sqlParam.value = item.value.ToString(); break;
                    }
                    queryPayload.SQL_PROCEDURE_PARAMETERS[i] = sqlParam;

                }


                var jsonData = await ExecuteStoredProcedure(_context, queryPayload);
                if (jsonData.ToString() == "[{}]")
                {

                    return StatusCode(200, null);
                }

                JArray jsonArray = JArray.Parse(jsonData.ToString());
                JToken _jArray = jsonArray[0]["Result"];
                JArray jArray = JArray.Parse(_jArray.ToString());
                JObject firstItem = (JObject)jArray[0];
                IEnumerable<JProperty> properties = firstItem.Properties();

                // Extract the keys
                List<string> keys = properties.Select(p => p.Name).ToList();
                List<string> types = new List<string>();
                foreach (JProperty property in firstItem.Properties())
                {
                    string valueType = property.Value.Type.ToString();
                    string theType = "";
                    if (valueType == "Integer" || valueType == "Integer")
                    {
                        theType = "numeric";
                    }
                    else
                    {
                        theType = "string";
                    }

                    types.Add(theType);
                }

                JArray columns = new JArray();
                List<GenericExcelColumnLabel> columns_list = new List<GenericExcelColumnLabel>();
                for (int i = 0; i < keys.Count; i++)
                {
                    JObject column = new JObject();
                    column["column_name"] = keys[i];
                    column["js_type"] = types[i];
                    column["type"] = "text";
                    columns.Add(column);

                    GenericExcelColumnLabel _col = new GenericExcelColumnLabel();
                    _col.columnLabel = keys[i];
                    _col.columnName = keys[i];
                    columns_list.Add(_col);
                }
                result["columns"] = columns;
                result["table_data"] = jArray;
                result["item_count"] = jArray.Count;




                DocCreatorService docCreator = new DocCreatorService();
                Byte[] retBytes = docCreator.CreateExcel(result.ToString(), columns_list.ToArray()).ToArray();

                // Return Byte array
                return StatusCode(200, retBytes);

            }
            catch (Exception ex)
            {
                ImsLogerrors evt = utility.GetLogError("CreateAndDownloadExcelForTable", "CreateAndDownloadExcelForTable", ex, UserId, new { exelSelectPayload = exelSelectPayload }, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
                return StatusCode(500);
            }
        }

        private async Task<string> ExecuteStoredProcedure(DbContext context, ExcelStoredProcedurePayload payload)
        {
            var jsonParameters = JsonConvert.SerializeObject(payload.SQL_PROCEDURE_PARAMETERS.Select(p => new { name = p.name, value = p.value }));
            var retJson = new SqlParameter
            {
                ParameterName = "@RET_JSON",
                SqlDbType = SqlDbType.NVarChar,
                Direction = ParameterDirection.Output,
                Size = -1
            };
            var spName = new SqlParameter("@SP_NAME", payload.PROCEDURE_NAME);
            var spSchema = new SqlParameter("@SP_SCHEMA", payload.PROCEDURE_SCHEMA ?? (object)DBNull.Value);
            var spParameters = new SqlParameter("@SP_PARAMETERS", jsonParameters ?? (object)DBNull.Value);
            
			try
            {
                string sql = "EXEC [dbo].[execute_SP_for_excel] @SP_NAME, @SP_SCHEMA, @SP_PARAMETERS, @RET_JSON OUTPUT";
                await context.Database.ExecuteSqlRawAsync(sql, spName, spSchema, spParameters, retJson);
                string resultJson = (string)retJson.Value;
                return resultJson;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




    }
}