// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using System.Text;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using jrs.DBContexts;
// using jrs.Models;
// using Microsoft.EntityFrameworkCore.Storage;
// using Microsoft.AspNetCore.Authorization;
// using jrs.Classes;
// using System.Security.Claims;
// using Syncfusion.DocIO;
// using Syncfusion.DocIO.DLS;
// using System.IO;
// using Syncfusion.DocIORenderer;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Data;
// using Microsoft.Data.SqlClient;
// using System.Data;

// namespace jrs.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     [Consumes("application/json")]
//     public class WordController : ControllerBase
//     {
//         private readonly GeneralContext _context;
//         private readonly IMSLogContext _logContext;
//         private readonly IConfiguration Configuration;
//         private string _connectionString;
//         string UserId = "";
//         string IP = "";
//          List<PmsObjective> projectObjectives;
//         ImsUtility utility = null;
//         /// <summary>
//         /// Constructor for the "GenericSqlController" class.
//         /// </summary>
//         public WordController(GeneralContext context, IMSLogContext logContext, IConfiguration configuration)
//         {
//             _context = context;
//             _logContext = logContext;
//             utility = new ImsUtility();
//             Configuration = configuration;
//         }

//         public PmsAnnualPlan GetPmsAnnualPlanMainData(int id)
//         {
//             ImsUtility utility = new ImsUtility();


//             PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan();
//             string viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_LIST WHERE ID=" + id;
//             SqlDataReader dr = null;

//             try
//             {
//                 UserId = User.Identity.Name;
//                 IP = utility.GetIP(Response);
//                 using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
//                 {
//                     if (conn.State != ConnectionState.Open) conn.Open();
//                     _connectionString = conn.ConnectionString;
//                     SqlCommand sqlcmd = new SqlCommand();
//                     sqlcmd.Connection = conn;
//                     sqlcmd.CommandType = CommandType.Text;

//                     sqlcmd.CommandText = viewName;

//                     dr = sqlcmd.ExecuteReader();
//                     while (dr.Read())
//                     {


//                         pmsAnnualPlan.Id = int.Parse(dr["Id"] + "");
//                         pmsAnnualPlan.apcode = (dr["APCODE"] + "");
//                         pmsAnnualPlan.office_id = dr["OFFICE_ID"] + "";
//                         pmsAnnualPlan.office_code = dr["OFFICE_CODE"] + "";
//                         pmsAnnualPlan.code = (dr["CODE"] + "");
//                         pmsAnnualPlan.code_id = (dr["CODE_ID"] + "");
//                         pmsAnnualPlan.descr = (dr["DESCR"] + "");
//                         pmsAnnualPlan.location_descr = (dr["LOCATION_DESCR"] + "");
//                         pmsAnnualPlan.admin_area_id = dr["IMS_ADMIN_AREA_ID"] + "";
//                         pmsAnnualPlan.admin_area_name = (dr["IMS_ADMIN_AREA_NAME"] + "");
//                         pmsAnnualPlan.start_year = (dr["START_YEAR"] + "");
//                         pmsAnnualPlan.end_year = (dr["END_YEAR"] + "");
//                         pmsAnnualPlan.status_id = dr["STATUS_ID"] + "";
//                         //pmsAnnualPlan.status_name = (dr["IMS_STATUS_NAME"] + "");
//                         pmsAnnualPlan.currency_code = (dr["CURRENCY_CODE"] + "");
//                         pmsAnnualPlan.location_id = (dr["LOCATION_ID"] + "");
//                         pmsAnnualPlan.location_description = (dr["LOCATION_DESCRIPTION"] + "");
//                         pmsAnnualPlan.country = (dr["IMS_ADMIN_AREA_NAME"] + "");
//                         // pmsAnnualPlan.activation_state = (dr["ACTIVATION_STATE"] + "");


//                     }
//                 }
//             }
//             catch (Exception ex)
//             {

//                 ImsLogerrors evt = utility.GetLogError("WordController", "GetPmsAnnualPlanMainData", ex, UserId, "", IP);
//                 _logContext.ImsLogerrors.Add(evt);
//                 _logContext.SaveChanges();


//             }

//             return pmsAnnualPlan;

//         }

//         public List<PmsIndicator> GetPmsAnnualOverallGoalIndicator(int id)
//         {
//             ImsUtility utility = new ImsUtility();
//             List<PmsIndicator> pmsIndicator = new List<PmsIndicator>();

//             PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan();
//             string viewName = "SELECT * FROM PMS_VI_OVERALL_GOAL_INDICATOR WHERE PMS_AP_ID=" + id;
//             SqlDataReader dr = null;

//             try
//             {
//                 UserId = User.Identity.Name;
//                 IP = utility.GetIP(Response);
//                 using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
//                 {
//                     conn.ConnectionString = _connectionString;
//                     if (conn.State != ConnectionState.Open) conn.Open();
//                     SqlCommand sqlcmd = new SqlCommand();
//                     sqlcmd.Connection = conn;
//                     sqlcmd.CommandType = CommandType.Text;

//                     sqlcmd.CommandText = viewName;

//                     dr = sqlcmd.ExecuteReader();
//                     while (dr.Read())
//                     {

//                         PmsIndicator pmsInd = new PmsIndicator();
//                         pmsInd.question = (dr["PMS_OVG_IND_QUESTION"] + "");
//                         pmsInd.indicator = (dr["PMS_OVG_IND"] + "");
//                         pmsInd.standard = (dr["PMS_OVG_IND_STANDARD"] + "");
//                         pmsInd.periodicity = (dr["PMS_OVG_IND_PERIODICITY"] + "");
//                         pmsInd.disaggregated = (dr["PMS_OVG_IND_DISAGGREGATED"] + "");
//                         pmsInd.technique = (dr["PMS_OVG_IND_TECHNIQUE"] + "");
//                         pmsInd.value = (dr["VALUE"] + "");

//                         pmsIndicator.Add(pmsInd);



//                     }
//                 }
//             }
//             catch (Exception ex)
//             {

//                 ImsLogerrors evt = utility.GetLogError("WordController", "GetPmsAnnualOverallGoalIndicator", ex, UserId, "", IP);
//                 _logContext.ImsLogerrors.Add(evt);
//                 _logContext.SaveChanges();


//             }

//             return pmsIndicator;

//         }

//         public List<PmsProcess> GetPmsProcess(int id)
//         {
//             ImsUtility utility = new ImsUtility();
//             List<PmsProcess> pmsProcess = new List<PmsProcess>();


//             string viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_SERVICE_PROCESS WHERE SERVICE_ID=" + id + " ORDER BY ID";
//             SqlDataReader dr = null;

//             try
//             {
//                 UserId = User.Identity.Name;
//                 IP = utility.GetIP(Response);
//                 using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
//                 {
//                     conn.ConnectionString = _connectionString;
//                     if (conn.State != ConnectionState.Open) conn.Open();
//                     SqlCommand sqlcmd = new SqlCommand();
//                     sqlcmd.Connection = conn;
//                     sqlcmd.CommandType = CommandType.Text;

//                     sqlcmd.CommandText = viewName;

//                     dr = sqlcmd.ExecuteReader();
//                     while (dr.Read())
//                     {

//                         PmsProcess pmsPrc = new PmsProcess();
//                         pmsPrc.id = (dr["ID"] + "");
//                         pmsPrc.description = (dr["DESCRIPTION"] + "");
//                         pmsPrc.process_type = (dr["ACTIVITY_TYPE_NAME"] + "");
//                         pmsProcess.Add(pmsPrc);



//                     }
//                 }
//             }
//             catch (Exception ex)
//             {

//                 ImsLogerrors evt = utility.GetLogError("WordController", "GetPmsProcess", ex, UserId, "", IP);
//                 _logContext.ImsLogerrors.Add(evt);
//                 _logContext.SaveChanges();


//             }
//             return pmsProcess;
//         }

//         public List<PmsOutput> GetPmsOutput(int id)
//         {
//             ImsUtility utility = new ImsUtility();
//             List<PmsOutput> pmsOutput = new List<PmsOutput>();


//             string viewName = "SELECT * FROM PMS_VI_ACTIVITY_OUTPUT WHERE PMS_ACTIVITY_ID=" + id;
//             SqlDataReader dr = null;

//             try
//             {
//                 UserId = User.Identity.Name;
//                 IP = utility.GetIP(Response);
//                 using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
//                 {
//                     conn.ConnectionString = _connectionString;
//                     if (conn.State != ConnectionState.Open) conn.Open();
//                     SqlCommand sqlcmd = new SqlCommand();
//                     sqlcmd.Connection = conn;
//                     sqlcmd.CommandType = CommandType.Text;

//                     sqlcmd.CommandText = viewName;

//                     dr = sqlcmd.ExecuteReader();
//                     while (dr.Read())
//                     {

//                         PmsOutput pmsOutp = new PmsOutput();
//                         pmsOutp.id = (dr["PMS_ACTIVITY_OUTPUT_REL_ID"] + "");
//                         pmsOutp.description = (dr["PMS_OUTPUT_DESC"] + "");

//                         pmsOutput.Add(pmsOutp);



//                     }
//                 }
//             }
//             catch (Exception ex)
//             {

//                 ImsLogerrors evt = utility.GetLogError("WordController", "GetPmsOutput", ex, UserId, "", IP);
//                 _logContext.ImsLogerrors.Add(evt);
//                 _logContext.SaveChanges();


//             }
//             return pmsOutput;
//         }


//         public List<PmsIndicator> GetPmsAnnualObjectiveIndicator(int id)
//         {
//             ImsUtility utility = new ImsUtility();
//             List<PmsIndicator> pmsIndicator = new List<PmsIndicator>();

//             PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan();
//             string viewName = "SELECT * FROM PMS_VI_OBJ_INDICATOR WHERE PMS_OBJ_ID=" + id;
//             SqlDataReader dr = null;

//             try
//             {
//                 UserId = User.Identity.Name;
//                 IP = utility.GetIP(Response);
//                 using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
//                 {
//                     conn.ConnectionString = _connectionString;
//                     if (conn.State != ConnectionState.Open) conn.Open();
//                     SqlCommand sqlcmd = new SqlCommand();
//                     sqlcmd.Connection = conn;
//                     sqlcmd.CommandType = CommandType.Text;

//                     sqlcmd.CommandText = viewName;

//                     dr = sqlcmd.ExecuteReader();
//                     while (dr.Read())
//                     {

//                         PmsIndicator pmsInd = new PmsIndicator();
//                         pmsInd.question = (dr["PMS_OBJ_IND_QUESTION"] + "");
//                         pmsInd.indicator = (dr["PMS_OBJ_IND"] + "");
//                         pmsInd.standard = (dr["PMS_OBJ_IND_STANDARD"] + "");
//                         pmsInd.periodicity = (dr["PMS_OBJ_IND_PERIODICITY"] + "");
//                         pmsInd.disaggregated = (dr["PMS_OBJ_IND_DISAGGREGATED"] + "");
//                         pmsInd.technique = (dr["PMS_OBJ_IND_TECHNIQUE"] + "");
//                         pmsInd.value = (dr["VALUE"] + "");

//                         pmsIndicator.Add(pmsInd);



//                     }
//                 }
//             }
//             catch (Exception ex)
//             {

//                 ImsLogerrors evt = utility.GetLogError("WordController", "GetPmsAnnualObjectiveIndicator", ex, UserId, "", IP);
//                 _logContext.ImsLogerrors.Add(evt);
//                 _logContext.SaveChanges();


//             }

//             return pmsIndicator;

//         }






//      public List<PmsIndicator> GetPmsOutputIndicator(int id)
//         {
//             ImsUtility utility = new ImsUtility();
//             List<PmsIndicator> pmsIndicator = new List<PmsIndicator>();

//             PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan();
//             string viewName = "SELECT * FROM PMS_VI_OUTPUT_INDICATOR WHERE PMS_PROC_OUTP_REL_ID=" + id;
//             SqlDataReader dr = null;

//             try
//             {
//                 UserId = User.Identity.Name;
//                 IP = utility.GetIP(Response);
//                 using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
//                 {
//                     conn.ConnectionString = _connectionString;
//                     if (conn.State != ConnectionState.Open) conn.Open();
//                     SqlCommand sqlcmd = new SqlCommand();
//                     sqlcmd.Connection = conn;
//                     sqlcmd.CommandType = CommandType.Text;

//                     sqlcmd.CommandText = viewName;

//                     dr = sqlcmd.ExecuteReader();
//                     while (dr.Read())
//                     {

//                         PmsIndicator pmsInd = new PmsIndicator();
//                         pmsInd.question = (dr["PMS_OUTP_IND_QUESTION"] + "");
//                         pmsInd.indicator = (dr["PMS_OUTP_IND"] + "");
//                         pmsInd.standard = (dr["PMS_OUTP_IND_STANDARD"] + "");
//                         pmsInd.periodicity = (dr["PMS_OUTP_IND_PERIODICITY"] + "");
//                         pmsInd.disaggregated = (dr["PMS_OUTP_IND_DISAGGREGATED"] + "");
//                         pmsInd.technique = (dr["PMS_OUTP_IND_TECHNIQUE"] + "");
//                         pmsInd.value = (dr["VALUE"] + "");

//                         pmsIndicator.Add(pmsInd);



//                     }
//                 }
//             }
//             catch (Exception ex)
//             {

//                 ImsLogerrors evt = utility.GetLogError("WordController", "GetPmsOutputIndicator", ex, UserId, "", IP);
//                 _logContext.ImsLogerrors.Add(evt);
//                 _logContext.SaveChanges();


//             }

//             return pmsIndicator;

//         }




//         public PmsOverallGoal GetPmsAnnualOverallGoal(int id)
//         {
//             ImsUtility utility = new ImsUtility();
//             PmsOverallGoal pmsOverallGoal = new PmsOverallGoal();

//             PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan();
//             string viewName = "SELECT * FROM PMS_VI_OVERALL_GOAL WHERE PMS_AP_ID=" + id;
//             SqlDataReader dr = null;

//             try
//             {
//                 UserId = User.Identity.Name;
//                 IP = utility.GetIP(Response);
//                 using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
//                 {
//                     conn.ConnectionString = _connectionString;
//                     if (conn.State != ConnectionState.Open) conn.Open();
//                     SqlCommand sqlcmd = new SqlCommand();
//                     sqlcmd.Connection = conn;
//                     sqlcmd.CommandType = CommandType.Text;

//                     sqlcmd.CommandText = viewName;

//                     dr = sqlcmd.ExecuteReader();
//                     while (dr.Read())
//                     {


//                         pmsOverallGoal.target = (dr["PMS_OVERALL_GOAL_TARGET_DESC"] + "");
//                         pmsOverallGoal.goal = dr["PMS_OVERALL_GOAL_DESC"] + "";
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {

//                 ImsLogerrors evt = utility.GetLogError("WordController", "GetPmsAnnualPlanOverallGoal", ex, UserId, "", IP);
//                 _logContext.ImsLogerrors.Add(evt);
//                 _logContext.SaveChanges();


//             }
//             return pmsOverallGoal;
//         }



//         public List<PmsNarrative> GetPmsAnnualPlanNarrative(int id, string table_name)
//         {
//             ImsUtility utility = new ImsUtility();
//             List<PmsNarrative> pmsNarrative = new List<PmsNarrative>();

//             PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan();
//             string viewName = "SELECT * FROM IMS_VI_NARRATIVE_BY_TYPE WHERE TABLE_NAME='" + table_name + "' AND REFERENCE_ID=" + id + " ORDER BY CODE";
//             SqlDataReader dr = null;

//             try
//             {
//                 UserId = User.Identity.Name;
//                 IP = utility.GetIP(Response);
//                 using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
//                 {
//                     conn.ConnectionString = _connectionString;
//                     if (conn.State != ConnectionState.Open) conn.Open();
//                     SqlCommand sqlcmd = new SqlCommand();
//                     sqlcmd.Connection = conn;
//                     sqlcmd.CommandType = CommandType.Text;

//                     sqlcmd.CommandText = viewName;

//                     dr = sqlcmd.ExecuteReader();
//                     while (dr.Read())
//                     {

//                         PmsNarrative pmsNarr = new PmsNarrative();
//                         pmsNarr.descr = (dr["DESCR"] + "");
//                         pmsNarr.section_text = dr["SECTION_TEXT"] + "";
//                         pmsNarr.section_title = dr["SECTION_TITLE"] + "";
//                         pmsNarrative.Add(pmsNarr);



//                     }
//                 }
//             }
//             catch (Exception ex)
//             {

//                 ImsLogerrors evt = utility.GetLogError("WordController", "GetPmsAnnualPlanNarrative", ex, UserId, "", IP);
//                 _logContext.ImsLogerrors.Add(evt);
//                 _logContext.SaveChanges();


//             }

//             return pmsNarrative;

//         }
//         public List<PmsObjective> GetPmsAnnualPlanObjective(int id)
//         {
//             ImsUtility utility = new ImsUtility();
//             List<PmsObjective> pmsObjective = new List<PmsObjective>();

//             PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan();
//             string viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_OUTCOME_OBJECTIVE_PRINT WHERE ANNUAL_PLAN_ID=" + id;
//             SqlDataReader dr = null;

//             try
//             {
//                 UserId = User.Identity.Name;
//                 IP = utility.GetIP(Response);
//                 using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
//                 {
//                     conn.ConnectionString = _connectionString;
//                     if (conn.State != ConnectionState.Open) conn.Open();
//                     SqlCommand sqlcmd = new SqlCommand();
//                     sqlcmd.Connection = conn;
//                     sqlcmd.CommandType = CommandType.Text;

//                     sqlcmd.CommandText = viewName;

//                     dr = sqlcmd.ExecuteReader();
//                     while (dr.Read())
//                     {

//                         PmsObjective pmsObj = new PmsObjective();
//                         pmsObj.id = (dr["ID"] + "");
//                         pmsObj.description = (dr["DESCRIPTION"] + "");
//                         pmsObj.served_people = dr["SERVED_PEOPLE"] + "";
//                         pmsObj.currency = dr["CURRENCY_REAL"] + "";
//                         pmsObj.estimated_budget = dr["BUDGET_EST"] + "";
//                         pmsObj.real_budget = dr["BUDG_REAL"] + "";
//                         pmsObj.outcome = dr["OUTCOME"] + "";
//                         pmsObj.target = dr["PMS_TARGET_DESC"] + "";
//                         pmsObjective.Add(pmsObj);



//                     }
//                 }
//             }
//             catch (Exception ex)
//             {

//                 ImsLogerrors evt = utility.GetLogError("WordController", "GetPmsAnnualPlanObjective", ex, UserId, "", IP);
//                 _logContext.ImsLogerrors.Add(evt);
//                 _logContext.SaveChanges();


//             }
//             return pmsObjective;
//         }

//         public List<PmsService> GetPmsService(int id)
//         {
//             ImsUtility utility = new ImsUtility();
//             List<PmsService> PmsService = new List<PmsService>();


//             string viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_SERVICE WHERE IMS_LANGUAGE_LOCALE='en' and OBJECTIVE_ID=" + id + " ORDER BY ID";
//             SqlDataReader dr = null;

//             try
//             {
//                 UserId = User.Identity.Name;
//                 IP = utility.GetIP(Response);
//                 using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
//                 {
//                     conn.ConnectionString = _connectionString;
//                     if (conn.State != ConnectionState.Open) conn.Open();
//                     SqlCommand sqlcmd = new SqlCommand();
//                     sqlcmd.Connection = conn;
//                     sqlcmd.CommandType = CommandType.Text;

//                     sqlcmd.CommandText = viewName;

//                     dr = sqlcmd.ExecuteReader();
//                     while (dr.Read())
//                     {

//                         PmsService pmsSrv = new PmsService();
//                         pmsSrv.id = (dr["ID"] + "");
//                         pmsSrv.title = (dr["TITLE"] + "");
//                         pmsSrv.start_date = (dr["START_DATE"] + "");
//                         pmsSrv.end_date = (dr["END_DATE"] + "");
//                         pmsSrv.category_code = (dr["PMS_COI_CODE"] + "");
//                         pmsSrv.served_people = dr["SERVED_PEOPLE"] + "";
//                         pmsSrv.category = dr["COI_LABEL"] + "";
//                         pmsSrv.type_of_service = dr["PMS_TOS_DESCRIPTION"] + "";
//                         pmsSrv.periodicity = dr["OC_TYPE"] + "";
//                         pmsSrv.location = dr["LOCATION_DETAILS"] + "";
//                         PmsService.Add(pmsSrv);



//                     }
//                 }
//             }
//             catch (Exception ex)
//             {

//                 ImsLogerrors evt = utility.GetLogError("WordController", "GetPmsService", ex, UserId, "", IP);
//                 _logContext.ImsLogerrors.Add(evt);
//                 _logContext.SaveChanges();


//             }
//             return PmsService;
//         }


//         [HttpGet("{id}")]
//         public byte[] CreateDocWithTOC(int id)
//         {
//             PmsAnnualPlan pmsAnnualPlanMainData = GetPmsAnnualPlanMainData(id);
//             using (WordDocument document = new WordDocument())
//             {
//                 document.EnsureMinimal();
//                 document.LastSection.PageSetup.Margins.All = 72;

//                 //Creates new WPicture instance and gets Image from file
//                 WPicture picture = new WPicture(document);
//                 byte[] imagelogo = System.IO.File.ReadAllBytes("img/logojrs.png");
//                 picture.LoadImage(imagelogo);
//                 picture.Height = 50;
//                 picture.Width = 200;
//                 WParagraph para = document.LastParagraph;
//                 if (para != null)
//                     para.ChildEntities.Add(picture);
//                 WSection section = document.LastSection;
//                 para = section.AddParagraph() as WParagraph;
//                 para.AppendText("Project Annual Plan");
//                 para.AppendText("\r");

//                 string html = @"<table style='font-size:10px;font-weight:bold;border:1px'>
//         <tr>
//         <td>Project Name:</td>
//         <td>" + pmsAnnualPlanMainData.descr + @"</td>
//         </tr>
//         <tr>
//         <td>Project Code:</td>
//         <td>" + pmsAnnualPlanMainData.code + @"</td>
//         </tr>
//         <tr>
//         <td>Location:</td>
//         <td>" + pmsAnnualPlanMainData.location_description + @"</td>
//         </tr>
//          <tr>
//         <td>Country:</td>
//         <td>" + pmsAnnualPlanMainData.country + @"</td>
//         </tr>
//         <tr>
//         <td>Project Start Date:</td>
//         <td>" + pmsAnnualPlanMainData.start_year + @"</td>
//         </tr>
//         <tr>
//         <td>Project Planned End Date:</td>
//         <td>" + pmsAnnualPlanMainData.end_year + @"</td>
//         </tr>
//         </table>";
//                 para.AppendHTML(html);
//                 para = document.LastSection.AddParagraph() as WParagraph;
//                 para.AppendBreak(BreakType.PageBreak);

//                 // para.AppendText("Project Name:" + pmsAnnualPlanMainData.descr);para.AppendText("\r");
//                 // para.AppendText("Project Code:" + pmsAnnualPlanMainData.code);para.AppendText("\r");
//                 // para.AppendText("Location:" + pmsAnnualPlanMainData.location_descr);para.AppendText("\r");
//                 // para.AppendText("Country:" + pmsAnnualPlanMainData.location_description);para.AppendText("\r");
//                 // para.AppendText("Project Start Date:" + pmsAnnualPlanMainData.start_year);para.AppendText("\r");
//                 // para.AppendText("Project Planned End Date:" + pmsAnnualPlanMainData.end_year);


//                 // para.AppendBreak(BreakType.PageBreak);



//                 para.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
//                 para.ApplyStyle(BuiltinStyle.Heading4);
//                 para = document.LastSection.AddParagraph() as WParagraph;

//                 //Insert TOC field in the Word document.
//                 TableOfContent toc = para.AppendTOC(1, 3);
//                 //Sets the heading levels 1 through 3 to include in the TOC.
//                 toc.LowerHeadingLevel = 1;
//                 toc.UpperHeadingLevel = 9;
//                 //Adds content to the Word document with built-in heading styles.





//                 WParagraph newPara = section.AddParagraph() as WParagraph;
//                 newPara.AppendBreak(BreakType.PageBreak);
//                 AddHeading(section, BuiltinStyle.Heading1, "Project executive summary", "");
//                 AddNarrative(id, "PMS_PROJECT", section, 1);
//                 AddHeading(section, BuiltinStyle.Heading2, "Outcome-based Objectives", "");

//                 AddObjective(id, section);
//                 AddHeading(section, BuiltinStyle.Heading1, "Project Design", "");
//                 AddHeading(section, BuiltinStyle.Heading2, "Overall Goal", "");
//                 AddOverallGoal(id, section);
//                 AddHeading(section, BuiltinStyle.Heading3, "Overall Goal Indicators", "");
//                 AddOverallGoalIndicator(id, section);
//                 for (int i = 0; i < projectObjectives.Count; i++)
//                 {

//                     PmsObjective projectObjective = projectObjectives[i];
//                     AddHeading(section, BuiltinStyle.Heading2, "Outcome-based Objectives", "");
//                     AddHeading(section, BuiltinStyle.Heading3, "Objective:" + projectObjective.description, "");
//                     int idobj = int.Parse(projectObjective.id);
//                     AddObjectiveDetails(i, section);
//                     AddNarrative(idobj, "PMS_OBJECTIVE", section, 2);
//                     AddHeading(section, BuiltinStyle.Heading3, "Objective Indicators", "");
//                     AddObjectiveIndicator(idobj, section);
//                     AddHeading(section, BuiltinStyle.Heading3, "Services", "");
//                     List<PmsService> services = GetPmsService(idobj);
//                     for (int j = 0; j < services.Count; j++)
//                     {
//                         PmsService service = services[j];
//                         AddHeading(section, BuiltinStyle.Heading4, "Service:" + service.title, "");
//                         AddServiceDetails(service, section);
//                         int srvId = int.Parse(service.id);
//                         AddNarrative(srvId, "PMS_SERVICE", section, 4);
//                         List<PmsProcess> processes = GetPmsProcess(srvId);
//                         for (int k = 0; k < processes.Count; k++)
//                         {
//                             PmsProcess process = processes[k];
//                             AddHeading(section, BuiltinStyle.Heading5, "Process:" + (k + 1), "");
//                             AddProcessDetails(process, section);
//                             int prcId = int.Parse(process.id);
//                             AddNarrative(prcId, "PMS_ACTIVITY", section, 5);

//                             List<PmsOutput> outputs = GetPmsOutput(prcId);
//                             for (int z = 0; z < outputs.Count; z++)
//                             {
//                                 PmsOutput output = outputs[z];
//                                 AddHeading(section, BuiltinStyle.Heading6, "Output:" + (z + 1), "");
//                                 AddOutputDetails(output, section);
//                                 int outpId = int.Parse(output.id);
//                                 AddHeading(section, BuiltinStyle.Heading7, "Output Indicators", "");
//                                 AddOutputIndicator(outpId, section);


//                             }
//                         }
//                     }


//                 }







// //                 AddHeading(section, BuiltinStyle.Heading2, "Section 1", "This is the built-in heading 2 style. A document can contain any number of sections. Sections are used to apply same formatting for a group of paragraphs. You can insert sections by inserting section breaks.");
// //                 AddHeading(section, BuiltinStyle.Heading3, "Paragraph 1", "This is the built-in heading 3 style. Each section contains any number of paragraphs. A paragraph is a set of statements that gives a meaning for the text.");
// //                 AddHeading(section, BuiltinStyle.Heading3, "Paragraph 2", "This is the built-in heading 3 style. This demonstrates the paragraphs at the same level and style as that of the previous one. A paragraph can have any number formatting. This can be attained by formatting each text range in the paragraph.");
// //                 //Adds a new section to the Word document.
// //                 section = document.AddSection() as WSection;
// //                 section.PageSetup.Margins.All = 72;
// //                 section.BreakCode = SectionBreakCode.NewPage;
// //                 AddHeading(section, BuiltinStyle.Heading2, "Section 2", "This is the built-in heading 2 style. A document can contain any number of sections. Sections are used to apply same formatting for a group of paragraphs. You can insert sections by inserting section breaks.");
// //                 AddHeading(section, BuiltinStyle.Heading3, "Paragraph 1", "This is the built-in heading 3 style. Each section contains any number of paragraphs. A paragraph is a set of statements that gives a meaning for the text.");
// //                 AddHeading(section, BuiltinStyle.Heading3, "Paragraph 2", "This is the built-in heading 3 style. This demonstrates the paragraphs at the same level and style as that of the previous one. A paragraph can have any number formatting. This can be attained by formatting each text range in the paragraph.");

// //                 //Adds a new section to the Word document.
// //                 section = document.AddSection() as WSection;
// //                 section.PageSetup.Margins.All = 72;
// //                 section.BreakCode = SectionBreakCode.NewPage;



// //                 IWParagraph paragraph = section.AddParagraph();
// //                 //Adds the text for the headings
// //                 paragraph.AppendText("First Chapter");
// //                 //Sets a built-in heading style.
// //                 paragraph.ApplyStyle(BuiltinStyle.Heading1);
// //                 //Adds the text into the paragraph



// //                 string htmlText = @"<html><head><title> XHTML to Doc 1 </title></head><body>
// //                                   <p>The standard Lorem Ipsum passage, used since the 1500s
// // Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.

// // Section 1.10.32 of de Finibus Bonorum et Malorum, written by Cicero in 45 BC
// // Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?

// // 1914 translation by H. Rackham
// // But I must explain to you how all this mistaken idea of denouncing pleasure and praising pain was born and I will give you a complete account of the system, and expound the actual teachings of the great explorer of the truth, the master-builder of human happiness. No one rejects, dislikes, or avoids pleasure itself, because it is pleasure, but because those who do not know how to pursue pleasure rationally encounter consequences that are extremely painful. Nor again is there anyone who loves or pursues or desires to obtain pain of itself, because it is pain, but because occasionally circumstances occur in which toil and pain can procure him some great pleasure. To take a trivial example, which of us ever undertakes laborious physical exercise, except to obtain some advantage from it? But who has any right to find fault with a man who chooses to enjoy a pleasure that has no annoying consequences, or one who avoids a pain that produces no resultant pleasure?

// // Section 1.10.33 of de Finibus Bonorum et Malorum, written by Cicero in 45 BC
// // At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.

// // 1914 translation by H. Rackham
// // On the other hand, we denounce with righteous indignation and dislike men who are so beguiled and demoralized by the charms of pleasure of the moment, so blinded by desire, that they cannot foresee the pain and trouble that are bound to ensue; and equal blame belongs to those who fail in their duty through weakness of will, which is the same as saying through shrinking from toil and pain. These cases are perfectly simple and easy to distinguish. In a free hour, when our power of choice is untrammelled and when nothing prevents our being able to do what we like best, every pleasure is to be welcomed and every pain avoided. But in certain circumstances and owing to the claims of duty or the obligations of business it will frequently occur that pleasures have to be repudiated and annoyances accepted. The wise man therefore always holds in these matters to this principle of selection: he rejects pleasures to secure other greater pleasures, or else he endures pains to avoid worse pains.</p><p></p>
// //                                   <p>It is possible to convert a valid XHTML content to Word Document content. The passed content should be either XHTML 1.0 or 1.1 compliant.</p><p></p>
// //                                   <p><font color='#000000' size='2'><b>XHTML 1.1 Transitional</b></font></p><p>It is the same as HTML 4.01 Transitional, but it follows XML syntax rules. It supports everything found in XHTML 1.0 Strict, but also permits the use of a number of elements and attributes that are judged presentational, in order to ease the transition from HTML 3.2 and earlier.  </p>
// //                                   </body></html>";

// //                 //Validates whether the string is in proper XHTML
// //                 section.Body.IsValidXHTML(htmlText, document.XHTMLValidateOption);
// //                 //Inserts HTML string to the document
// //                 section.Body.InsertXHTML(htmlText);
// //                 // AddHeading(section,BuiltinStyle.Heading2,"html",section.Body.ToString());


// //                 paragraph = section.AddParagraph();

// //                 paragraph.AppendText("Second Chapter");
// //                 //Sets a built-in heading style.
// //                 paragraph.ApplyStyle(BuiltinStyle.Heading1);
// //                 //Adds the text into the paragraph



// //                 htmlText = "<html>" + FormatHtml() + "</html>";

// //                 //Validates whether the string is in proper XHTML
// //                 section.Body.IsValidXHTML(htmlText, document.XHTMLValidateOption);
// //                 //Inserts HTML string to the document
// //                 section.Body.InsertXHTML(htmlText);








//                 //Updates the table of contents.
//                 document.UpdateTableOfContents();
//                 //Saves the file in the given path
//                 // Stream docStream = File.Create(Path.GetFullPath(@"../../../TOC-creation.docx"));
//                 // document.Save(docStream, FormatType.Docx);
//                 // docStream.Dispose();
//                 //AddHeaderFooter(document);
//                 MemoryStream stream = new MemoryStream();
//                 document.Save(stream, FormatType.Docx);
//                 stream.Position = 0;

//                 // using (FileStream file = new FileStream("fileTOC1.doc", FileMode.Create, System.IO.FileAccess.Write))
//                 // {
//                     byte[]  bytes = new byte[stream.Length];
//                     stream.Read(bytes, 0, (int)stream.Length);
//                     // file.Write(bytes, 0, bytes.Length);
//                     stream.Close();
//                 //}
//                  return bytes;
//             }
//         }
//         private string FormatHtml()
//         {

//             string str = "";
//             str = @"<div class=""mce-toc"">
//     <h2>Table of Contents</h2>
//     <ul>
//         <li><a href=""#mcetoc_1f7tjf0ib2"">Some initial content</a>
//             <ul>
//                 <li><a href=""#mcetoc_1f7tjf0ib3"">Test test</a></li>
//             </ul>
//         </li>
//     </ul>
// </div>
// <h1 id=""mcetoc_1f7tjf0ib2"">Some initial content</h1>
// <p>cpoedje omacw omee sef sfe sf&nbsp; fse sefe fef</p>
// <p><img title=""Logo_NM_Green_Grey.PNG""
//         src=""data:image/png;base64,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""
//         alt="""" width=""316"" height=""244"" /></p>
// <h2 id=""mcetoc_1f7tjf0ib3""><a id=""Some_Bookmark""></a>Test test</h2>
// <p>&nbsp;</p>
// <p>&nbsp;</p>";
//             str = str.Replace("<br>", "<br/>");
//             str = str.Replace("<br >", "<br/>");
//             str = str.Replace("< br>", "<br/>");
//             str = str.Replace("< br >", "<br/>");
//             return str;
//         }
//         private static void AddHeading(WSection section, BuiltinStyle builtinStyle, string headingText, string paragraghText)
//         {
//             WParagraph newPara = section.AddParagraph() as WParagraph;
//             WTextRange text = newPara.AppendText(headingText) as WTextRange;
//             newPara.ApplyStyle(builtinStyle);
//             newPara = section.AddParagraph() as WParagraph;
//             newPara.AppendText(paragraghText);
//             section.AddParagraph();
//         }


//         private void AddObjective(int id, WSection section)
//         {
//             projectObjectives = GetPmsAnnualPlanObjective(id);
//             var newPara = section.AddParagraph() as WParagraph;
//             StringBuilder htmlN = new StringBuilder();
//             htmlN.Append("<table style='border:1px'>");

//             htmlN.Append("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
//             htmlN.Append("<td>");
//             htmlN.Append("OUTCOME BASED OBJECTIVE");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("TARGET");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("OUTCOME");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("NUMBER OF SERVICES PROVIDED");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("BUDGET");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("ESTIMATED BUDGET");
//             htmlN.Append("</td>");
//             htmlN.Append("</tr>");
//             for (int i = 0; i < projectObjectives.Count; i++)
//             {
//                 htmlN.Append("<tr>");
//                 PmsObjective projectObjective = projectObjectives[i];
//                 htmlN.Append("<td>");
//                 htmlN.Append(projectObjective.description);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(projectObjective.target);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(projectObjective.outcome);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(projectObjective.served_people);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(projectObjective.real_budget + " " + projectObjective.currency);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(projectObjective.estimated_budget + " " + projectObjective.currency);
//                 htmlN.Append("</td>");
//                 htmlN.Append("</tr>");

//             }
//             htmlN.Append("</table>");
//             newPara.AppendHTML(htmlN.ToString());
//         }


//         private void AddObjectiveDetails(int idx, WSection section)
//         {
//             PmsObjective projectObjective = projectObjectives[idx];
//             var newPara = section.AddParagraph() as WParagraph;
//             StringBuilder htmlN = new StringBuilder();
//             htmlN.Append("<table style='border:1px'>");

//             htmlN.Append("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
//             htmlN.Append("<td>");
//             htmlN.Append("OUTCOME BASED OBJECTIVE");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("TARGET");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("OUTCOME");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("NUMBER OF SERVICE EROGATED");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("BUDGET");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("ESTIMATED BUDGET");
//             htmlN.Append("</td>");
//             htmlN.Append("</tr>");

//             htmlN.Append("<tr>");
//             htmlN.Append("<td>");
//             htmlN.Append(projectObjective.description);
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append(projectObjective.target);
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append(projectObjective.outcome);
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append(projectObjective.served_people);
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append(projectObjective.real_budget + " " + projectObjective.currency);
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append(projectObjective.estimated_budget + " " + projectObjective.currency);
//             htmlN.Append("</td>");
//             htmlN.Append("</tr>");


//             htmlN.Append("</table>");
//             newPara.AppendHTML(htmlN.ToString());
//         }

//         private void AddServiceDetails(PmsService service, WSection section)
//         {

//             var newPara = section.AddParagraph() as WParagraph;
//             StringBuilder htmlN = new StringBuilder();
//             htmlN.Append("<table style='border:1px'>");
//             DateTime start_date = DateTime.MinValue;
//             DateTime.TryParse(service.start_date, out start_date);
//             DateTime end_date = DateTime.MaxValue;
//             DateTime.TryParse(service.end_date, out end_date);
//             htmlN.Append("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
//             htmlN.Append("<td>");
//             htmlN.Append("SERVICE");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("CATEGORY");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("TYPE OF SERVICE");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("START DATE");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("END DATE");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("PERIODICITY");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("NUMBER OF SERVICES PROVIDED");
//             htmlN.Append("</td>");
//             htmlN.Append("</tr>");

//             htmlN.Append("<tr>");
//             htmlN.Append("<td>");
//             htmlN.Append(service.title);
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append(service.category_code + " - " + service.category);
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append(service.type_of_service);
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append(start_date.ToShortDateString());
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append(end_date.Date.ToShortDateString());
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append(service.periodicity);
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append(service.served_people);
//             htmlN.Append("</td>");
//             htmlN.Append("</tr>");


//             htmlN.Append("</table>");
//             newPara.AppendHTML(htmlN.ToString());
//         }

//         private void AddOverallGoal(int id, WSection section)
//         {
//             PmsOverallGoal projectGoal = GetPmsAnnualOverallGoal(id);
//             var newPara = section.AddParagraph() as WParagraph;
//             StringBuilder htmlN = new StringBuilder();
//             htmlN.Append("<table style='border:1px'>");

//             htmlN.Append("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
//             htmlN.Append("<td>");
//             htmlN.Append("TARGET");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("TARGET");
//             htmlN.Append("</td>");
//             htmlN.Append("</tr>");

//             htmlN.Append("<tr>");

//             htmlN.Append("<td>");
//             htmlN.Append(projectGoal.target);
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append(projectGoal.goal);
//             htmlN.Append("</td>");
//             htmlN.Append("</tr>");


//             htmlN.Append("</table>");
//             newPara.AppendHTML(htmlN.ToString());
//         }

//         private void AddOverallGoalIndicator(int id, WSection section)
//         {
//             List<PmsIndicator> goalIndicators = GetPmsAnnualOverallGoalIndicator(id);
//             var newPara = section.AddParagraph() as WParagraph;
//             StringBuilder htmlN = new StringBuilder();
//             htmlN.Append("<table style='border:1px'>");

//             htmlN.Append("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
//             htmlN.Append("<td>");
//             htmlN.Append("QUESTION");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("INDICATOR");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("STANDARD");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("TECHNIQUE");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("PERIODICITY");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("DISAGGREGATED");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("VALUE");
//             htmlN.Append("</td>");
//             htmlN.Append("</tr>");
//             for (int i = 0; i < goalIndicators.Count; i++)
//             {
//                 htmlN.Append("<tr>");
//                 PmsIndicator goalIndic = goalIndicators[i];
//                 htmlN.Append("<td>");
//                 htmlN.Append(goalIndic.question);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(goalIndic.indicator);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(goalIndic.standard);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(goalIndic.technique);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(goalIndic.periodicity);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(goalIndic.disaggregated);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(goalIndic.value);
//                 htmlN.Append("</td>");
//                 htmlN.Append("</tr>");

//             }
//             htmlN.Append("</table>");
//             newPara.AppendHTML(htmlN.ToString());
//         }
//        private void AddOutputIndicator(int id, WSection section)
//         {
//             List<PmsIndicator> outputIndicators = GetPmsOutputIndicator(id);
//             var newPara = section.AddParagraph() as WParagraph;
//             StringBuilder htmlN = new StringBuilder();
//             htmlN.Append("<table style='border:1px'>");

//             htmlN.Append("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
//             htmlN.Append("<td>");
//             htmlN.Append("QUESTION");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("INDICATOR");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("STANDARD");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("TECHNIQUE");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("PERIODICITY");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("DISAGGREGATED");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("VALUE");
//             htmlN.Append("</td>");
//             htmlN.Append("</tr>");
//             for (int i = 0; i < outputIndicators.Count; i++)
//             {
//                 htmlN.Append("<tr>");
//                 PmsIndicator objIndic = outputIndicators[i];
//                 htmlN.Append("<td>");
//                 htmlN.Append(objIndic.question);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(objIndic.indicator);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(objIndic.standard);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(objIndic.technique);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(objIndic.periodicity);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(objIndic.disaggregated);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(objIndic.value);
//                 htmlN.Append("</td>");
//                 htmlN.Append("</tr>");

//             }
//             htmlN.Append("</table>");
//             newPara.AppendHTML(htmlN.ToString());
//         }
 
//         private void AddObjectiveIndicator(int id, WSection section)
//         {
//             List<PmsIndicator> objectiveIndicators = GetPmsAnnualObjectiveIndicator(id);
//             var newPara = section.AddParagraph() as WParagraph;
//             StringBuilder htmlN = new StringBuilder();
//             htmlN.Append("<table style='border:1px'>");

//             htmlN.Append("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
//             htmlN.Append("<td>");
//             htmlN.Append("QUESTION");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("INDICATOR");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("STANDARD");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("TECHNIQUE");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("PERIODICITY");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("DISAGGREGATED");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("VALUE");
//             htmlN.Append("</td>");
//             htmlN.Append("</tr>");
//             for (int i = 0; i < objectiveIndicators.Count; i++)
//             {
//                 htmlN.Append("<tr>");
//                 PmsIndicator objIndic = objectiveIndicators[i];
//                 htmlN.Append("<td>");
//                 htmlN.Append(objIndic.question);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(objIndic.indicator);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(objIndic.standard);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(objIndic.technique);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(objIndic.periodicity);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(objIndic.disaggregated);
//                 htmlN.Append("</td>");
//                 htmlN.Append("<td>");
//                 htmlN.Append(objIndic.value);
//                 htmlN.Append("</td>");
//                 htmlN.Append("</tr>");

//             }
//             htmlN.Append("</table>");
//             newPara.AppendHTML(htmlN.ToString());
//         }
//         private void AddOutputDetails(PmsOutput output, WSection section)
//         {

//             var newPara = section.AddParagraph() as WParagraph;
//             StringBuilder htmlN = new StringBuilder();
//             htmlN.Append("<table style='border:1px'>");
//             htmlN.Append("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
//             htmlN.Append("<td>");
//             htmlN.Append("OUTPUT DESCRIPTION");
//             htmlN.Append("</td>");

//             htmlN.Append("</tr>");

//             htmlN.Append("<tr>");
//             htmlN.Append("<td>");
//             htmlN.Append(output.description);
//             htmlN.Append("</td>");


//             htmlN.Append("</tr>");


//             htmlN.Append("</table>");
//             newPara.AppendHTML(htmlN.ToString());
//         }

//         private void AddProcessDetails(PmsProcess process, WSection section)
//         {

//             var newPara = section.AddParagraph() as WParagraph;
//             StringBuilder htmlN = new StringBuilder();
//             htmlN.Append("<table style='border:1px'>");
//             htmlN.Append("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
//             htmlN.Append("<td>");
//             htmlN.Append("PROCESS DESCRIPTION");
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append("PROCESS TYPE");
//             htmlN.Append("</td>");

//             htmlN.Append("</tr>");

//             htmlN.Append("<tr>");
//             htmlN.Append("<td>");
//             htmlN.Append(process.description);
//             htmlN.Append("</td>");
//             htmlN.Append("<td>");
//             htmlN.Append(process.process_type);
//             htmlN.Append("</td>");

//             htmlN.Append("</tr>");


//             htmlN.Append("</table>");
//             newPara.AppendHTML(htmlN.ToString());
//         }

//         private void AddNarrative(int id, string tableName, WSection section, int startingLevel)
//         {


//             List<PmsNarrative> projetcNarratives = GetPmsAnnualPlanNarrative(id, tableName);

//             for (int i = 0; i < projetcNarratives.Count; i++)
//             {

//                 PmsNarrative projetcNarrative = projetcNarratives[i];

//                 switch (startingLevel)
//                 {
//                     case 1:
//                         if (projetcNarrative.section_title == "")
//                         {
//                             AddHeading(section, BuiltinStyle.Heading2, projetcNarrative.descr, "");

//                         }
//                         else
//                         {
//                             AddHeading(section, BuiltinStyle.Heading3, projetcNarrative.section_title, "");

//                         }
//                         break;
//                     case 2:
//                         if (projetcNarrative.section_title == "")
//                         {
//                             AddHeading(section, BuiltinStyle.Heading3, projetcNarrative.descr, "");

//                         }
//                         else
//                         {
//                             AddHeading(section, BuiltinStyle.Heading4, projetcNarrative.section_title, "");

//                         }
//                         break;
//                     case 3:
//                         if (projetcNarrative.section_title == "")
//                         {
//                             AddHeading(section, BuiltinStyle.Heading4, projetcNarrative.descr, "");

//                         }
//                         else
//                         {
//                             AddHeading(section, BuiltinStyle.Heading5, projetcNarrative.section_title, "");

//                         }
//                         break;
//                     case 4:
//                         if (projetcNarrative.section_title == "")
//                         {
//                             AddHeading(section, BuiltinStyle.Heading5, projetcNarrative.descr, "");

//                         }
//                         else
//                         {
//                             AddHeading(section, BuiltinStyle.Heading6, projetcNarrative.section_title, "");

//                         }
//                         break;
//                     case 5:
//                         if (projetcNarrative.section_title == "")
//                         {
//                             AddHeading(section, BuiltinStyle.Heading6, projetcNarrative.descr, "");

//                         }
//                         else
//                         {
//                             AddHeading(section, BuiltinStyle.Heading7, projetcNarrative.section_title, "");

//                         }
//                         break;
//                     case 6:
//                         if (projetcNarrative.section_title == "")
//                         {
//                             AddHeading(section, BuiltinStyle.Heading7, projetcNarrative.descr, "");

//                         }
//                         else
//                         {
//                             AddHeading(section, BuiltinStyle.Heading8, projetcNarrative.section_title, "");

//                         }
//                         break;
//                 }
//                 var newPara = section.AddParagraph() as WParagraph;
//                 StringBuilder htmlN = new StringBuilder();
//                 htmlN.Append("<table>");

//                 htmlN.Append("<tr><td>");
//                 htmlN.Append(projetcNarrative.section_text);
//                 htmlN.Append("</td></tr>");
//                 htmlN.Append("</table>");
//                 newPara.AppendHTML(htmlN.ToString());
//             }
//         }
//         private void AddHeaderFooter(WordDocument document)
//         {
//             foreach (WSection section in document.Sections)
//             {
//                 IWParagraph paragraph = section.HeadersFooters.Header.AddParagraph();
//                 IWTextRange textRange = paragraph.AppendText("This is Header");
//                 paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
//                 textRange.CharacterFormat.Bold = true;
//                 textRange.CharacterFormat.FontSize = 20;
//                 paragraph = section.HeadersFooters.Footer.AddParagraph();
//                 textRange = paragraph.AppendText("This is Footer");
//                 paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
//                 textRange.CharacterFormat.Bold = true;
//                 textRange.CharacterFormat.FontSize = 20;
//             }
//         }
//     }
//     public class PmsNarrative
//     {
//         public string descr { get; set; }
//         public string section_text { get; set; }
//         public string section_title { get; set; }
//     }


//     public class PmsObjective
//     {
//         public string id { get; set; }
//         public string description { get; set; }
//         public string served_people { get; set; }
//         public string target { get; set; }
//         public string outcome { get; set; }
//         public string real_budget { get; set; }

//         public string estimated_budget { get; set; }
//         public string currency { get; set; }
//     }

//     public class PmsOverallGoal
//     {
//         public string target { get; set; }
//         public string goal { get; set; }

//     }


//     public class PmsIndicator
//     {
//         public string question { get; set; }
//         public string indicator { get; set; }
//         public string standard { get; set; }
//         public string periodicity { get; set; }
//         public string disaggregated { get; set; }
//         public string technique { get; set; }

//         public string value { get; set; }

//     }
//     public class PmsService
//     {
//         public string id { get; set; }
//         public string title { get; set; }
//         public string start_date { get; set; }
//         public string end_date { get; set; }
//         public string category_code { get; set; }
//         public string served_people { get; set; }
//         public string category { get; set; }
//         public string type_of_service { get; set; }
//         public string periodicity { get; set; }
//         public string location { get; set; }
//     }
//     public class PmsProcess
//     {
//         public string id { get; set; }
//         public string description { get; set; }
//         public string process_type { get; set; }
//     }


//     public class PmsOutput
//     {
//         public string id { get; set; }
//         public string description { get; set; }

//     }
// }




