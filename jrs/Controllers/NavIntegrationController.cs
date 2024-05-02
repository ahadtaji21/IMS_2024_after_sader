using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NavDimension;
using NavGLBudget;
using System.Net;
using Microsoft.Extensions.Configuration;
using jrs.DBContexts;
using jrs.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authorization;
using jrs.Classes;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using System.Data;
using jrs.DA;
using Newtonsoft.Json;

namespace jrs.Controllers
{
    [ApiController]
    [Consumes("application/json")]
    public class NavIntegrationController : Controller
    {
        private readonly GeneralContext _context;
        private readonly IMSLogContext _logContext;
        private readonly IConfiguration Configuration;
        private string _connectionString;
        private string UserId = "";
        private string IP = "";
        public NavIntegrationController(GeneralContext context, IMSLogContext logContext, IConfiguration configuration)
        {
            _context = context;
            _logContext = logContext;
            Configuration = configuration;
        }

        // Altered following : https://medium.com/grensesnittet/integrating-with-soap-web-services-in-net-core-adebfad173fb

        //public IActionResult Index()
        //{
        //    return View();
        //}

        private NetworkCredential GetCredential()
        {
        return new NetworkCredential("NAV.INTEGRATION", "yBZgObvaGJk/RSL5swufffZjnLMx13qdzXRGDqSq3MY=");
 
          //  return new NetworkCredential("luca.tartaglia", "Vr7w7dSMKhhyjrXfjJxkT7e2Osi1VPXTNh1BUMSmQZU=");
        }


        // GET: api/testDimensions
        [HttpGet("api/navDimensionsGet")]
        // [Authorize]
        public async Task<ActionResult<Object>> navDimensionsGet(int id)
        {

            try
            {
                string _serviceUrl = Configuration.GetValue<string>("NavService:urlDim");
                string _serviceUser = Configuration.GetValue<string>("NavService:user");
                string _servicePwd = Configuration.GetValue<string>("NavService:password");

               
                //PMEGLBudgetEntryStaging gls = new PMEGLBudgetEntryStaging();
                                  PMEDimensionValueStaging_PortClient dvs = new PMEDimensionValueStaging_PortClient(    _serviceUrl,
                    new TimeSpan(0, 1, 0),
                    _serviceUser,
                    _servicePwd);

                //PMEDimensionValueStaging_Service dvs_s = new PMEDimensionValueStaging_Service();

                //dvs_s.Credentials = GetCredential();
                //var aa = dvs_s.Read(1);

                var aa = await dvs.ReadAsync(id);

                return StatusCode(200, aa);
            }
            catch (Exception ex)
            {
                //@TODO: Implementare Log
                //ImsLogerrors evt = utility.GetLogError("GenericSqlController", "GenericSelectStructure", ex, UserId, new { templateId = templateId, queryCondition = queryCondition }, IP);
                //_logContext.ImsLogerrors.Add(evt);
                //_logContext.SaveChanges();
                return StatusCode(500);
            }
        }

    


        // GET: api/testDimensions
        [HttpPost("api/navDimensions")]
        // [Authorize]
        public async Task<ActionResult<Object>> navDimensions(NavDimension1 dimension)
        {

            ImsUtility utility = new ImsUtility();
            try
            {
                string _serviceUrl = Configuration.GetValue<string>("NavService:urlDim");
                string _serviceUser = Configuration.GetValue<string>("NavService:user");
                string _servicePwd = Configuration.GetValue<string>("NavService:password");

                //PMEGLBudgetEntryStaging gls = new PMEGLBudgetEntryStaging();
                    PMEDimensionValueStaging_PortClient dvs = new PMEDimensionValueStaging_PortClient(    _serviceUrl,
                    new TimeSpan(0, 1, 0),
                    _serviceUser,
                    _servicePwd);


StringBuilder sb = new StringBuilder();
                //PMEDimensionValueStaging_Service dvs_s = new PMEDimensionValueStaging_Service();

                //dvs_s.Credentials = GetCredential();
                //var aa = dvs_s.Read(1);
                NavDimension.Create request = new NavDimension.Create();
                request.PMEDimensionValueStaging = new PMEDimensionValueStaging();
                request.PMEDimensionValueStaging.PME_Dimension_Code = dimension.DimensionCode;
                request.PMEDimensionValueStaging.PME_Code = (dimension.Code + "").Trim();
                request.PMEDimensionValueStaging.PME_Name = dimension.Name;
                request.PMEDimensionValueStaging.PME_Blocked = dimension.Blocked.ToString().ToLower();
                request.PMEDimensionValueStaging.PME_Global_Dim_No = dimension.GlobalDimensionNo;
                request.PMEDimensionValueStaging.PME_Last_Modified_Date_Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                request.PMEDimensionValueStaging.PME_Mapp_Cat_of_Interv = dimension.MappingCatOfIntervention.ToString().ToLower();
                request.PMEDimensionValueStaging.PME_Cat_of_Interv = dimension.CatOfInterventionCode;
                request.PMEDimensionValueStaging.PME_Project_city = dimension.ProjectCity;
                request.PMEDimensionValueStaging.PME_Company = utility.GetIsoCompany(dimension.Company);
                request.PMEDimensionValueStaging.PME_Guid =dimension.Guid; //dimension.Guid;
                //request.PMEDimensionValueStaging.Key = "";

                sb.Append("PM_DIMENSION_CODE,");sb.Append(dimension.DimensionCode);sb.Append(System.Environment.NewLine.ToString());
                sb.Append("PM_CODE,");sb.Append((dimension.Code + "").Trim());sb.Append(System.Environment.NewLine.ToString());
                sb.Append("PM_NAME,");sb.Append(dimension.Name);sb.Append(System.Environment.NewLine.ToString());
                sb.Append("PM_IS_BLOCKED,");sb.Append(dimension.Blocked.ToString().ToLower());sb.Append(System.Environment.NewLine.ToString());
                sb.Append("PM_DIM_NBR,");sb.Append(dimension.GlobalDimensionNo);sb.Append(System.Environment.NewLine.ToString());
                sb.Append("PM_LAST_MOD_DT,");sb.Append(request.PMEDimensionValueStaging.PME_Last_Modified_Date_Time);sb.Append(System.Environment.NewLine.ToString());

                sb.Append("PM_IS_COI,");sb.Append(dimension.MappingCatOfIntervention.ToString().ToLower());sb.Append(System.Environment.NewLine.ToString());
                sb.Append("PM_COI_CODE,");sb.Append(dimension.CatOfInterventionCode);sb.Append(System.Environment.NewLine.ToString());
                sb.Append("PM_PRJ_LOCATION,");sb.Append(dimension.ProjectCity);sb.Append(System.Environment.NewLine.ToString());
                sb.Append("PM_COMPANY,");sb.Append(dimension.Company);sb.Append(System.Environment.NewLine.ToString());
                sb.Append("PM_GUID,");sb.Append(dimension.Guid);sb.Append(System.Environment.NewLine.ToString()); //dimension.Guid);sb.Append(System.Environment.NewLine.ToString());Environment.NewLine.ToString());

                var aa = await dvs.CreateAsync(request);
                sb.Append("Entry_No,");sb.Append(aa.PMEDimensionValueStaging.Entry_No);sb.Append(System.Environment.NewLine.ToString()); 
                sb.Append("Key,");sb.Append(aa.PMEDimensionValueStaging.Key);sb.Append(System.Environment.NewLine.ToString()); 
 
                return StatusCode(200, aa);
            }
            catch (Exception ex)
            {
                //@TODO: Implementare Log
                ImsLogerrors evt = utility.GetLogError("NavIntegrationController", "navDimensionsPut", ex, UserId, new { dimension = dimension }, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
                return StatusCode(500);
            }
        }




        // GET: api/testGLBudget
        [HttpGet("api/navGLBudgetPut")]
        //  [Authorize]
        public async Task<ActionResult<Object>> navGLBudgetPut(NavBudget1 navBudget)
        {

            ImsUtility utility = new ImsUtility();
            try
            {
                string _serviceUrl = Configuration.GetValue<string>("NavService:urlBudget");
                string _serviceUser = Configuration.GetValue<string>("NavService:user");
                string _servicePwd = Configuration.GetValue<string>("NavService:password");

                PMEGLBudgetEntryStaging_PortClient bes = new PMEGLBudgetEntryStaging_PortClient(
                     _serviceUrl,
                    new TimeSpan(0, 1, 0),
                    _serviceUser,
                    _servicePwd);



                // •	Project code (Global Dimension 1 Code)
                // •	Donor agreement code  (Global Dimension 2 Code)
                // •	Department code (Shortcut Dimension 3 Code)
                // •	Type of service code (Shortcut Dimension 5 Code)
                // •	Cat. Of intervention code (Shortcut Dimension 4 Code)
                // •	Function code (Shortcut Dimension 6 Code)
                // •	Subtrans code (Shortcut Dimension 7 Code)

                NavGLBudget.Create request = new NavGLBudget.Create();
                // request.PMEGLBudgetEntryStaging = new PMEGLBudgetEntryStaging();
                // request.PMEGLBudgetEntryStaging.PM_BDG_NAME = "BUDGET";
                // request.PMEGLBudgetEntryStaging.PM_ACC_NUM = "60401";
                // request.PMEGLBudgetEntryStaging.PM_DATE = DateTime.Now.ToString("yyyy-MM-dd");
                // request.PMEGLBudgetEntryStaging.PM_GBL_DIM1_CODE = "VAT09";
                // request.PMEGLBudgetEntryStaging.PM_GBL_DIM2_CODE = "";

                // request.PMEGLBudgetEntryStaging.PM_AMOUNT = "100.00";
                // request.PMEGLBudgetEntryStaging.PM_DESCRIPTION = "Desc";

                // request.PMEGLBudgetEntryStaging.PM_BDG_DIM1_CODE = "ADVOCACY";
                // request.PMEGLBudgetEntryStaging.PM_BDG_DIM2_CODE = "GSC";
                // request.PMEGLBudgetEntryStaging.PM_BDG_DIM3_CODE = "ADV";
                // request.PMEGLBudgetEntryStaging.PM_BDG_DIM4_CODE = "ADV_00";




                // request.PMEGLBudgetEntryStaging.PM_MEASURE_UNIT_CODE = "";

                // request.PMEGLBudgetEntryStaging.PM_QUANTITY = "50.00";
                // request.PMEGLBudgetEntryStaging.PM_UNIT_COST = "2.00";
                // request.PMEGLBudgetEntryStaging.PM_ENTRY_NMB = "2";
                // request.PMEGLBudgetEntryStaging.PM_LAST_MOD_DT = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // request.PMEGLBudgetEntryStaging.PM_DELETED = "false";

                // request.PMEGLBudgetEntryStaging.PM_COMPANY = "";
                var aa = await bes.CreateAsync(request);
                return StatusCode(200, aa);
            }
            catch (Exception ex)
            {
                //@TODO: Implementare Log
                //ImsLogerrors evt = utility.GetLogError("GenericSqlController", "GenericSelectStructure", ex, UserId, new { templateId = templateId, queryCondition = queryCondition }, IP);
                //_logContext.ImsLogerrors.Add(evt);
                //_logContext.SaveChanges();
                return StatusCode(500);
            }


        }


        // [HttpGet("api/navGLBulkBudgetStub")]
        // public async Task<ActionResult<Object>> navGLBulkBudgetStub(NavBudget1[] navBudget)
        // {
        //     return StatusCode(200, "");
        // }

        // GET: api/testGLBudget
        [HttpPost("api/navGLBulkBudget")]
   
        //  [Authorize]
        public async Task<ActionResult<Object>> navGLBulkBudget(NavPayload navBudgetPayload)
        {

            NavBudget2[] navBudget = JsonConvert.DeserializeObject<NavBudget2[]>(navBudgetPayload.data);
            if (navBudget == null || navBudget.Length == 0)
                return StatusCode(200, "");

            ImsUtility utility = new ImsUtility();
            try
            {
                string _serviceUrl = Configuration.GetValue<string>("NavService:urlBudget");
                string _serviceUser = Configuration.GetValue<string>("NavService:user");
                string _servicePwd = Configuration.GetValue<string>("NavService:password");


                PMEGLBudgetEntryStaging_PortClient bes = new PMEGLBudgetEntryStaging_PortClient(
                     _serviceUrl,
                    new TimeSpan(0, 1, 0),
                    _serviceUser,
                    _servicePwd);



                // •	Project code (Global Dimension 1 Code)
                // •	Donor agreement code  (Global Dimension 2 Code)
                // •	Department code (Shortcut Dimension 3 Code)
                // •	Type of service code (Shortcut Dimension 5 Code)
                // •	Cat. Of intervention code (Shortcut Dimension 4 Code)
                // •	Function code (Shortcut Dimension 6 Code)
                // •	Subtrans code (Shortcut Dimension 7 Code)



                if (navBudget.Length == 1)
                {
                        // Create SINGLE INSERT
                     NavGLBudget.Create request = new NavGLBudget.Create();
                    System.Text.StringBuilder sb = new StringBuilder();
                    request.PMEGLBudgetEntryStaging= new PMEGLBudgetEntryStaging();
                    request.PMEGLBudgetEntryStaging.PME_Budget_Name = navBudget[0].pMBDGNAME;
                    request.PMEGLBudgetEntryStaging.PME_G_L_Account_No = navBudget[0].pMACCNUM;
                    request.PMEGLBudgetEntryStaging.PME_Date = navBudget[0].pMDATE;
                    request.PMEGLBudgetEntryStaging.PME_Global_Dimension_1_Code = navBudget[0].pMGBLDIM1CODE;
                    request.PMEGLBudgetEntryStaging.PME_Global_Dimension_2_Code = navBudget[0].pMGBLDIM2CODE;
                    request.PMEGLBudgetEntryStaging.PME_Amount = navBudget[0].pMAMOUNT;
                    request.PMEGLBudgetEntryStaging.PME_Description = navBudget[0].pMDESCRIPTION;
                  

                    request.PMEGLBudgetEntryStaging.PME_Budget_Dimension_1_Code = navBudget[0].pMBDGDIM1CODE;
                    request.PMEGLBudgetEntryStaging.PME_Budget_Dimension_2_Code = navBudget[0].pMBDGDIM2CODE;
                    request.PMEGLBudgetEntryStaging.PME_Budget_Dimension_3_Code = navBudget[0].pMBDGDIM3CODE;
                    request.PMEGLBudgetEntryStaging.PME_Budget_Dimension_4_Code =navBudget[0].pMBDGDIM4CODE;// navBudget[i].pMBDGDIM4CODE.Substring(0, Math.Min(20,navBudget[i].pMBDGDIM4CODE.Length-1));
                    request.PMEGLBudgetEntryStaging.PME_Budget_Dimension_5_Code =navBudget[0].pMBDGDIM5CODE; //subtrans

                    request.PMEGLBudgetEntryStaging.PME_Unit_of_Measure_Code = navBudget[0].pMMEASUREUNITCODE;

                    request.PMEGLBudgetEntryStaging.PME_Quantity = navBudget[0].pMQUANTITY;
                    request.PMEGLBudgetEntryStaging.PME_Unit_Cost_LCY = navBudget[0].pMUNITCOST;
                    //request.PMEGLBudgetEntryStaging.PM_ENTRY_NMB = GetProgressiveId();
                    request.PMEGLBudgetEntryStaging.PME_Last_Date_Modified = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    request.PMEGLBudgetEntryStaging.PME_Deleted = navBudget[0].pMDELETED;

                    request.PMEGLBudgetEntryStaging.PME_Company =  utility.GetIsoCompany(navBudget[0].pMCOMPANY);
                    //request.PMEGLBudgetEntryStaging.Key="";
                    request.PMEGLBudgetEntryStaging.PME_Entry_No =navBudget[0].pMENTRYNMB;// GetEntryNumber(navBudget[0].pMENTRYNMB)+"";

                                       //request.PMEGLBudgetEntryStaging= new PMEGLBudgetEntryStaging();
                    sb.Append("PM_BDG_NAME,");sb.Append(navBudget[0].pMBDGNAME);sb.Append(System.Environment.NewLine.ToString());
                    sb.Append("PM_ACC_NUM,");sb.Append(navBudget[0].pMACCNUM);sb.Append(System.Environment.NewLine.ToString());
                    sb.Append("PM_DATE");sb.Append(request.PMEGLBudgetEntryStaging.PME_Date);sb.Append(System.Environment.NewLine.ToString());
                    sb.Append("PM_GBL_DIM1_CODE,");sb.Append(navBudget[0].pMGBLDIM1CODE);sb.Append(System.Environment.NewLine.ToString());
                    sb.Append("PM_GBL_DIM2_CODE,");sb.Append(navBudget[0].pMGBLDIM2CODE);sb.Append(System.Environment.NewLine.ToString());

                    sb.Append("PM_AMOUNT,");sb.Append(navBudget[0].pMAMOUNT);sb.Append(System.Environment.NewLine.ToString());
                    sb.Append("PM_DESCRIPTION,");sb.Append(navBudget[0].pMDESCRIPTION);sb.Append(System.Environment.NewLine.ToString());
                  

                    sb.Append("PM_BDG_DIM1_CODE,");sb.Append(navBudget[0].pMBDGDIM1CODE);sb.Append(System.Environment.NewLine.ToString());
                    sb.Append("PM_BDG_DIM2_CODE,");sb.Append(navBudget[0].pMBDGDIM2CODE);sb.Append(System.Environment.NewLine.ToString());
                    sb.Append("PM_BDG_DIM3_CODE,");sb.Append(navBudget[0].pMBDGDIM3CODE);sb.Append(System.Environment.NewLine.ToString());
                    sb.Append("PM_BDG_DIM4_CODE,");sb.Append(navBudget[0].pMBDGDIM4CODE);sb.Append(System.Environment.NewLine.ToString());
                    sb.Append("PM_BDG_DIM5_CODE,");sb.Append(navBudget[0].pMBDGDIM5CODE);sb.Append(System.Environment.NewLine.ToString());
                    // navBudget[i].pMBDGDIM4CODE.Substring(0, Math.Min(20,navBudget[i].pMBDGDIM4CODE.Length-1));


                    sb.Append("PM_MEASURE_UNIT_CODE,");sb.Append(navBudget[0].pMMEASUREUNITCODE);sb.Append(System.Environment.NewLine.ToString());

                    sb.Append("PM_QUANTITY,");sb.Append(navBudget[0].pMQUANTITY);sb.Append(System.Environment.NewLine.ToString());
                    sb.Append("PM_UNIT_COST,");sb.Append(navBudget[0].pMUNITCOST);sb.Append(System.Environment.NewLine.ToString());
                    //sb.Append("PM_ENTRY_NMB = GetProgressiveId();
                    sb.Append("PM_LAST_MOD_DT,");sb.Append(request.PMEGLBudgetEntryStaging.PME_Last_Date_Modified);sb.Append(System.Environment.NewLine.ToString());

                    sb.Append("PM_DELETED,"); sb.Append("false");sb.Append(System.Environment.NewLine.ToString());

                    sb.Append("PM_COMPANY,");sb.Append(navBudget[0].pMCOMPANY);sb.Append(System.Environment.NewLine.ToString());
                    //sb.Append("Key="");
                    sb.Append("PM_ENTRY_NMB,");sb.Append(GetEntryNumber(navBudget[0].pMENTRYNMB));sb.Append(System.Environment.NewLine.ToString()); 
                    var aa = await bes.CreateAsync(request);

                     sb.Append("Entry_No,");sb.Append(aa.PMEGLBudgetEntryStaging.Entry_No);sb.Append(System.Environment.NewLine.ToString()); 
                     sb.Append("Key,");sb.Append(aa.PMEGLBudgetEntryStaging.Key);sb.Append(System.Environment.NewLine.ToString()); 
                    return StatusCode(200, aa);
                
                }
                else
                {
                    // Create BULK INSERT


                    NavGLBudget.CreateMultiple request = new NavGLBudget.CreateMultiple();
                    request.PMEGLBudgetEntryStaging_List = new PMEGLBudgetEntryStaging[navBudget.Length];
                    for (int i = 0; i < navBudget.Length; i++)
                    {
                        request.PMEGLBudgetEntryStaging_List[i] = new PMEGLBudgetEntryStaging();
                        request.PMEGLBudgetEntryStaging_List[i].PME_Budget_Name = navBudget[i].pMBDGNAME;
                        request.PMEGLBudgetEntryStaging_List[i].PME_G_L_Account_No = navBudget[i].pMACCNUM;
                        request.PMEGLBudgetEntryStaging_List[i].PME_Date = navBudget[i].pMDATE;
                        request.PMEGLBudgetEntryStaging_List[i].PME_Global_Dimension_1_Code = navBudget[i].pMGBLDIM1CODE;
                        request.PMEGLBudgetEntryStaging_List[i].PME_Global_Dimension_2_Code = navBudget[i].pMGBLDIM2CODE;

                        request.PMEGLBudgetEntryStaging_List[i].PME_Amount = navBudget[i].pMAMOUNT;
                        request.PMEGLBudgetEntryStaging_List[i].PME_Description = navBudget[i].pMDESCRIPTION;

                        request.PMEGLBudgetEntryStaging_List[i].PME_Budget_Dimension_1_Code = navBudget[i].pMBDGDIM1CODE;
                        request.PMEGLBudgetEntryStaging_List[i].PME_Budget_Dimension_2_Code = navBudget[i].pMBDGDIM2CODE;
                        request.PMEGLBudgetEntryStaging_List[i].PME_Budget_Dimension_3_Code = navBudget[i].pMBDGDIM3CODE;
                        request.PMEGLBudgetEntryStaging_List[i].PME_Budget_Dimension_4_Code = navBudget[i].pMBDGDIM4CODE;// navBudget[i].pMBDGDIM4CODE.Substring(0, Math.Min(20,navBudget[i].pMBDGDIM4CODE.Length-1));
                        request.PMEGLBudgetEntryStaging_List[i].PME_Budget_Dimension_5_Code = navBudget[i].pMBDGDIM5CODE;

                        request.PMEGLBudgetEntryStaging_List[i].PME_Unit_of_Measure_Code = navBudget[i].pMMEASUREUNITCODE;

                        request.PMEGLBudgetEntryStaging_List[i].PME_Quantity = navBudget[i].pMQUANTITY;
                        request.PMEGLBudgetEntryStaging_List[i].PME_Unit_Cost_LCY = navBudget[i].pMUNITCOST;
                        //request.PMEGLBudgetEntryStaging_List[i].PM_ENTRY_NMB = GetProgressiveId();
                        request.PMEGLBudgetEntryStaging_List[i].PME_Last_Date_Modified = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        request.PMEGLBudgetEntryStaging_List[i].PME_Deleted = navBudget[i].pMDELETED;

                        request.PMEGLBudgetEntryStaging_List[i].PME_Company =  utility.GetIsoCompany(navBudget[i].pMCOMPANY);

                       // request.PMEGLBudgetEntryStaging_List[i].Entry_No  = GetEntryNumber(navBudget[i].pMENTRYNMB);
                       request.PMEGLBudgetEntryStaging_List[i].PME_Entry_No = navBudget[i].pMENTRYNMB; //GetEntryNumber(navBudget[i].pMENTRYNMB)+""; 
                        //request.PMEGLBudgetEntryStaging_List[i].Key="";
                        
                    }

                    var aa = await bes.CreateMultipleAsync(request);
                    ImsLogevents evt = utility.GetLogEvent("NavIntegrationController", "navGLBulkBudgetPut", "nav bulk insert", UserId, new { navBudget = navBudgetPayload.data }, IP);
                    _logContext.ImsLogevents.Add(evt);
                    _logContext.SaveChanges();
                     return StatusCode(200, aa);
                }
               
            }
            catch (Exception ex)
            {
                //@TODO: Implementare Log
                ImsLogerrors evt = utility.GetLogError("NavIntegrationController", "navGLBulkBudgetPut", ex, UserId, new { navBudget = navBudgetPayload.data }, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
                return StatusCode(500,ex);
            }


        }


   private int GetEntryNumber(string nbr)
        {

            ImsUtility utility = new ImsUtility();
            //ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            //var Name = ClaimsPrincipal.Current.Identity.Name;
            string UserId = "";
            string IP = "";
            SqlCommand cmdSqldb = null;
            SqlDataReader dr = null;
            string retId = "";

            try
            {
                UserId = User.Identity.Name;
                IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
                {
                    if (_connectionString + "" == "")
                        _connectionString = conn.ConnectionString;
                    else
                        conn.ConnectionString = _connectionString;
                    conn.Open();
                    cmdSqldb = new SqlCommand();
                    cmdSqldb.CommandType = System.Data.CommandType.StoredProcedure;
                    cmdSqldb.Connection = conn;

                    cmdSqldb.CommandText = DatabaseConstants.spPMSSelGetEntryNbr;



                    #region Parameters

                        cmdSqldb.Parameters.AddWithValue("@NBR", nbr);
                    #endregion Parameters
                    dr = cmdSqldb.ExecuteReader();
                    while (dr.Read())
                    {
                        retId = dr["ID"] + "";
                    }

                    // ImsLogevents evt = utility.GetLogEvent("PmsAnnualPlanController", "AddOrUpdatePmsAnnualPlan", msg, UserId, pmsAnnualPlan, IP);
                    // _logContext.ImsLogevents.Add(evt);
                    // _logContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                ImsLogerrors evt = utility.GetLogError("NavIntegrationController", "GetEntryNumber", ex, UserId, nbr, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
                return -1;
            }
            return int.Parse(retId);
        }

        private string GetProgressiveId()
        {

            ImsUtility utility = new ImsUtility();
            //ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            //var Name = ClaimsPrincipal.Current.Identity.Name;
            string UserId = "";
            string IP = "";
            SqlCommand cmdSqldb = null;
            SqlDataReader dr = null;
            string retId = "";

            try
            {
                UserId = User.Identity.Name;
                IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
                {
                    if (_connectionString + "" == "")
                        _connectionString = conn.ConnectionString;
                    else
                        conn.ConnectionString = _connectionString;
                    conn.Open();
                    cmdSqldb = new SqlCommand();
                    cmdSqldb.CommandType = System.Data.CommandType.StoredProcedure;
                    cmdSqldb.Connection = conn;

                    cmdSqldb.CommandText = DatabaseConstants.spPMSSelGetProgressiveId;



                    #region Parameters


                    #endregion Parameters
                    dr = cmdSqldb.ExecuteReader();
                    while (dr.Read())
                    {
                        retId = dr["PROGRESSIVE_ID"] + "";
                    }

                    // ImsLogevents evt = utility.GetLogEvent("PmsAnnualPlanController", "AddOrUpdatePmsAnnualPlan", msg, UserId, pmsAnnualPlan, IP);
                    // _logContext.ImsLogevents.Add(evt);
                    // _logContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                ImsLogerrors evt = utility.GetLogError("NavIntegrationController", "GetProgressiveId", ex, UserId, "", IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
                return retId;
            }
            return retId;
        }
    }

  
}
