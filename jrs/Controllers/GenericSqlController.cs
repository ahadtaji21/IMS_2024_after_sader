
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using jrs.DBContexts;
using jrs.Models;
using jrs.Classes;

using Microsoft.Extensions.Configuration;
using jrs.Services;

namespace jrs.Controllers
{
    /// <summary>
    /// Controller class for all the generic operations to the DB.
    /// </summary>
    // [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class GenericSqlController : ControllerBase
    {
        private readonly GeneralContext _context;
        private readonly IMSLogContext _logContext;
        private readonly IConfiguration Configuration;
        string UserId = "";
        string IP = "";
        ImsUtility utility = null;
        /// <summary>
        /// Constructor for the "GenericSqlController" class.
        /// </summary>
        public GenericSqlController(GeneralContext context, IMSLogContext logContext, IConfiguration configuration)
        {
            _context = context;
            _logContext = logContext;
            utility = new ImsUtility();
            Configuration = configuration;
        }

        /// <summary>
        /// Execute a query to the given view with the provided parameters.
        /// </summary>
        [HttpPost("/generic-sql-select", Name = "Generic sql select")]
        [Authorize]
        public async Task<Object> GenericSelect(GenericSqlSelectPayload payload)
        {

            var retJson = new SqlParameter("RET_JSON", "") { Direction = ParameterDirection.Output, DbType = DbType.String, Size = -1 };
            var execViewName = payload.viewName;
            var columnList = payload.colList;
            var whereCondition = payload.whereCond;
            var orderStatement = payload.orderStmt;
            var itemNumber = payload.itemNumber;
            var skipNumber = payload.skipNumber;
            var officeId = payload.officeId;
            var currentLocale = payload.currentLocale;
            bool ignoreOfficeFilter = payload.ignoreOfficeFilter ?? false;
            GenericConditionRule[] conditionRules = payload.conditionRules ?? null;
            string conditions = null;

            // Manage itemNumber == -1 to recover all items
            if (itemNumber == -1)
            {
                itemNumber = null;
            }

            //Manage condition rules
            if(conditionRules != null && conditionRules.Length > 0){
                string[] conditionStrings = Array.ConvertAll<GenericConditionRule, string>(conditionRules, item => item.ToString());
                conditions = string.Join(",", conditionStrings);
                conditions = $"[{conditions}]";
            }

            try
            {
                IP = utility.GetIP(Response);
                UserId = User.Identity.Name;
                await _context.Database.ExecuteSqlInterpolatedAsync($"dbo.UTIL_SP_GENERIC_QUERY_DATA @VIEW_NAME={execViewName}, @COLUMN_LIST={columnList}, @WHERE_CONDITION={whereCondition}, @ORDER_STATEMENT={orderStatement}, @FETCH_NBR={itemNumber}, @OFFSET_NBR={skipNumber}, @OFFICE_ID={officeId}, @LOCALE={currentLocale}, @IGNORE_OFFICE_FILTER={ignoreOfficeFilter}, @CONDITION_RULES={conditions}, @RET_JSON={retJson} output");
            }
            catch (Exception ex)
            {
                //@TODO: Implementare Log
                ImsLogerrors evt = utility.GetLogError("GenericSqlController", "GenericSelect", ex, UserId, payload, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();

            }

            return retJson.Value;
        }


        /// <summary>
        /// Execute a SP with the given payload.
        /// </summary>
        [HttpPost("/generic-sql-sp-call", Name = "Generic sql SP call")]
        [Authorize]
        public async Task<Object> GenericSPCall(GenericSqlPayload jsonPayload)
        {
            var retJson = new SqlParameter("RET_JSON", "") { Direction = ParameterDirection.Output, DbType = DbType.String, Size = -1 };
            try
            {
                IP = utility.GetIP(Response);
                UserId = User.Identity.Name;
                await _context.Database.ExecuteSqlInterpolatedAsync($"{jsonPayload.spName} @JSON_PAYLOAD={jsonPayload.ToString()}, @RET_JSON={retJson} output");
                ImsLogevents evt = utility.GetLogEvent("GenericSqlController", "GenericSPCall", "GenericSPCall", UserId, jsonPayload, IP);
                _logContext.ImsLogevents.Add(evt);
                _logContext.SaveChanges();

            }
            catch (Exception ex)
            {
                //@TODO: Implementare Log
                ImsLogerrors evt = utility.GetLogError("GenericSqlController", "GenericSPCall", ex, UserId, jsonPayload, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
            }

            return retJson.Value;
        }


        /// <summary>
        /// Return the structure of the request Table/View.
        /// </summary>
        [HttpPost("/generic-sql-select-structure", Name = "Generic sql select structure")]
        [Authorize]
        public async Task<Object> GenericSelectStructure(GenericSqlSelectPayload payload)
        {

            var retJson = new SqlParameter("RET_JSON", "") { Direction = ParameterDirection.Output, DbType = DbType.String, Size = -1 };
            var execViewName = payload.viewName;
            var columnList = payload.colList;
            var whereCondition = payload.whereCond;
            var orderStatement = payload.orderStmt;


            try
            {
                IP = utility.GetIP(Response);
                UserId = User.Identity.Name;
                await _context.Database.ExecuteSqlInterpolatedAsync($"dbo.UTIL_SP_GENERIC_QUERY_DATA_STRUCTURE @VIEW_NAME={execViewName}, @COLUMN_LIST={columnList}, @WHERE_CONDITION={whereCondition}, @ORDER_STATEMENT={orderStatement}, @RET_JSON={retJson} output");
            }
            catch (Exception ex)
            {
                //@TODO: Implementare Log
                ImsLogerrors evt = utility.GetLogError("GenericSqlController", "GenericSelectStructure", ex, UserId, payload, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();

            }

            return retJson.Value;
        }

        /// <summary>
        /// Generate and download .docx file from the requested html.
        /// </summary>
        [HttpPost("/doc-download", Name = "Word doc download")]

        public async Task<IActionResult> DownloadDoc()
        {

            var retJson = new SqlParameter("RET_JSON", "") { Direction = ParameterDirection.Output, DbType = DbType.String, Size = -1 };
            var execViewName = "HR_TIMESHEET";
            var columnList = "HR_TIMESHEET_ID,HR_TIMESHEET_NOTES_USER";
            var whereCondition = "HR_TIMESHEET_ID = 17";
            var orderStatement = "1";


            try
            {
                IP = utility.GetIP(Response);
                UserId = User.Identity.Name;
                await _context.Database.ExecuteSqlInterpolatedAsync($"dbo.UTIL_SP_GENERIC_QUERY_DATA @VIEW_NAME={execViewName}, @COLUMN_LIST={columnList}, @WHERE_CONDITION={whereCondition}, @ORDER_STATEMENT={orderStatement}, @RET_JSON={retJson} output");
            }
            catch (Exception ex)
            {
                //@TODO: Implementare Log
                ImsLogerrors evt = utility.GetLogError("GenericSqlController", "GenericSelectStructure", ex, UserId, "", IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();

            }

            // return retJson.Value;
            return null;
        }

        // // GET: api/template
        // [HttpGet("api/template")]
        // public async Task<ActionResult<IEnumerable<ImsTemplate>>> GetImsTemplate()
        // {
        //     List<ImsTemplate> templates = await _context.ImsTemplate
        //         .Include(t => t.TemplateParams)
        //         .ToListAsync();
        //     return templates;
        // }

        // // GET: api/template-param
        // [HttpGet("api/template-param")]
        // public async Task<ActionResult<IEnumerable<ImsTemplateParams>>> GetImsTemplateParam()
        // {
        //     List<ImsTemplateParams> templateParams = await _context.ImsTemplateParams
        //         .ToListAsync();
        //     return templateParams;
        // }

        // ///<summary>
        // /// Generate an OpenXml Word document based on the defined text in an IMS_TEMPLATE replacing data with a specific query condition.
        // ///</summary>
        // ///<returns>Byte array representing the generate OpenXml Word document.</returns>
        // /// <param name="templateId">ID of the IMS_TEMPLATE to use for generating the document document.</param>
        // /// <param name="queryCondition">Query string to apply to the template data query.</param>
        // // GET: api/merged-text
        // [HttpGet("api/merged-text")]
        // [Authorize]
        // public async Task<IActionResult> MergedText(int templateId, string queryCondition){
        //     string documentText;

        //     // Get Template
        //     List<ImsTemplate> templates = await _context.ImsTemplate.Where(t => t.TemplateId == templateId)
        //         .Include(t => t.TemplateParams)
        //         .ToListAsync();

        //     if(templates.Count == 0){
        //         return NotFound();
        //     }
        //     ImsTemplate template = templates[0];

        //     // Get data to replace
        //     try
        //     {
        //         IP = utility.GetIP(Response);
        //         UserId = User.Identity.Name;

        //         // Setup DB connection params
        //         string connectionString = Configuration.GetConnectionString("jrsdb");
        //         string dataQueryCondition = queryCondition ?? "1=1";
        //         string queryString = "";//$"{template.TemplateDataQuery} WHERE {dataQueryCondition}";

        //         using ( SqlConnection connection = new SqlConnection(connectionString))
        //         {
        //             SqlCommand command = new SqlCommand(queryString, connection);
        //             connection.Open();
        //             SqlDataReader reader = command.ExecuteReader();

        //             // Call Read  on reader before accessing data
        //             reader.Read();
        //             IDataRecord record = (IDataRecord)reader;

        //             // Setup document text
        //             documentText = template.TemplateText;
        //             foreach (ImsTemplateParams param in template.TemplateParams)
        //             {
        //                 string columnValue = record[param.TemplateParamColumnName].ToString();
        //                 documentText = documentText.Replace($"##{param.TemplateParamColumnName}##", columnValue);
        //             }

        //             // Close reader when done reading
        //             reader.Close();

        //             // Generate .docx
        //             DocCreatorService docCreator = new DocCreatorService();
        //             Byte[] retBytes = docCreator.CreateDocxFromHtml(documentText).ToArray();

        //             // Return Byte array
        //             return StatusCode(200, retBytes);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         //@TODO: Implementare Log
        //         ImsLogerrors evt = utility.GetLogError("GenericSqlController", "GenericSelectStructure", ex, UserId, new {templateId=templateId, queryCondition=queryCondition}, IP);
        //         _logContext.ImsLogerrors.Add(evt);
        //         _logContext.SaveChanges();
        //          return StatusCode(500);
        //     }
        // }

    }
}