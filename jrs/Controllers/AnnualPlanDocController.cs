using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using jrs.Classes;
using jrs.DBContexts;
using jrs.Models;
using jrs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace jrs.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    [Consumes ("application/json")]
    public class AnnualPlanDocController : ControllerBase {
        private readonly GeneralContext _context;
        private readonly IMSLogContext _logContext;
        private readonly IConfiguration Configuration;
        private string _connectionString;
        System.Security.Claims.ClaimsPrincipal _User;
        string UserId = "";
        string IP = "";
        List<PmsObjective> projectObjectives;
        ImsUtility utility = null;
        /// <summary>
        /// Constructor for the "GenericSqlController" class.
        /// </summary>
        public AnnualPlanDocController (GeneralContext context, IMSLogContext logContext, IConfiguration configuration) {
            _context = context;
            _logContext = logContext;
            utility = new ImsUtility ();
            Configuration = configuration;
            _connectionString = _context.Database.GetDbConnection ().ConnectionString;
            // _User=User;
        }
        //  public AnnualPlanDocController(GeneralContext context, IMSLogContext logContext, IConfiguration configuration, System.Security.Claims.ClaimsPrincipal user, string ip,string connectionString)
        // {
        //     _context = context;
        //     _logContext = logContext;
        //     utility = new ImsUtility();
        //     Configuration = configuration;
        //     _User=user;
        //     _connectionString= connectionString;
        // }
        #region DB
        public PmsAnnualPlan GetPmsAnnualPlanMainData (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();
            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_LIST WHERE ID=" + id;
            } else {
                viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_LIST_VERSION WHERE ID=" + id + " AND VERSION_ID=" + version_id;
            }
            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();

                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        pmsAnnualPlan.Id = int.Parse (dr["Id"] + "");
                        pmsAnnualPlan.apcode = (dr["APCODE"] + "");
                        pmsAnnualPlan.office_id = dr["OFFICE_ID"] + "";
                        pmsAnnualPlan.office_code = dr["OFFICE_CODE"] + "";
                        pmsAnnualPlan.code = (dr["CODE"] + "");
                        pmsAnnualPlan.code_id = (dr["CODE_ID"] + "");
                        pmsAnnualPlan.descr = (dr["DESCR"] + "");
                        pmsAnnualPlan.location_descr = (dr["LOCATION_DESCR"] + "");
                        pmsAnnualPlan.admin_area_id = dr["IMS_ADMIN_AREA_ID"] + "";
                        pmsAnnualPlan.admin_area_name = (dr["IMS_ADMIN_AREA_NAME"] + "");
                        pmsAnnualPlan.start_year = (dr["START_YEAR"] + "");
                        pmsAnnualPlan.end_year = (dr["END_YEAR"] + "");
                        pmsAnnualPlan.status_id = dr["STATUS_ID"] + "";
                        //pmsAnnualPlan.status_name = (dr["IMS_STATUS_NAME"] + "");
                        pmsAnnualPlan.currency_code = (dr["CURRENCY_CODE"] + "");
                        pmsAnnualPlan.location_id = (dr["LOCATION_ID"] + "");
                        pmsAnnualPlan.location_description = (dr["LOCATION_DESCRIPTION"] + "");
                        pmsAnnualPlan.country = (dr["IMS_ADMIN_AREA_NAME"] + "");
                        // pmsAnnualPlan.activation_state = (dr["ACTIVATION_STATE"] + "");

                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsAnnualPlanMainData", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }

            return pmsAnnualPlan;

        }

        public List<PmsIndicator> GetPmsAnnualOverallGoalIndicator (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsIndicator> pmsIndicator = new List<PmsIndicator> ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();
            string viewName = "";
            if (version_id == 0) {
                viewName = viewName = "SELECT * FROM PMS_VI_OVERALL_GOAL_INDICATOR WHERE PMS_AP_ID=" + id;
            } else {
                viewName = "SELECT * FROM PMS_VI_OVERALL_GOAL_INDICATOR_VER WHERE PMS_AP_ID=" + id + " AND VERSION_ID=" + version_id;
            }

            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        PmsIndicator pmsInd = new PmsIndicator ();
                        pmsInd.question = (dr["PMS_OVG_IND_QUESTION"] + "");
                        pmsInd.indicator = (dr["PMS_OVG_IND"] + "");
                        pmsInd.standard = (dr["PMS_OVG_IND_STANDARD"] + "");
                        pmsInd.periodicity = (dr["PMS_OVG_IND_PERIODICITY"] + "");
                        pmsInd.disaggregated = (dr["PMS_OVG_IND_DISAGGREGATED"] + "");
                        pmsInd.technique = (dr["PMS_OVG_IND_TECHNIQUE"] + "");
                        pmsInd.value = (dr["VALUE"] + "");

                        pmsIndicator.Add (pmsInd);

                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsAnnualOverallGoalIndicator", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }

            return pmsIndicator;

        }

        public List<PmsProcess> GetPmsProcess (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsProcess> pmsProcess = new List<PmsProcess> ();

            string viewName = "";
            if (version_id == 0) {
                viewName = viewName = viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_SERVICE_PROCESS WHERE SERVICE_ID=" + id + " ORDER BY ID";
            } else {
                viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_SERVICE_PROCESS_VER WHERE SERVICE_ID=" + id + " AND VERSION_ID=" + version_id;
            }

            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        PmsProcess pmsPrc = new PmsProcess ();
                        pmsPrc.id = (dr["ID"] + "");
                        pmsPrc.description = (dr["DESCRIPTION"] + "");
                        pmsPrc.process_type = (dr["ACTIVITY_TYPE_NAME"] + "");
                        pmsProcess.Add (pmsPrc);

                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsProcess", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }
            return pmsProcess;
        }

        public List<PmsOutput> GetPmsOutput (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsOutput> pmsOutput = new List<PmsOutput> ();
            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM PMS_VI_ACTIVITY_OUTPUT WHERE PMS_ACTIVITY_ID=" + id;
            } else {
                viewName = "SELECT * FROM PMS_VI_ACTIVITY_OUTPUT_VER WHERE PMS_ACTIVITY_ID=" + id + " AND VERSION_ID=" + version_id;
            }

            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        PmsOutput pmsOutp = new PmsOutput ();
                        pmsOutp.id = (dr["PMS_ACTIVITY_OUTPUT_REL_ID"] + "");
                        pmsOutp.description = (dr["PMS_OUTPUT_DESC"] + "");

                        pmsOutput.Add (pmsOutp);

                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsOutput", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }
            return pmsOutput;
        }

        public List<PmsIndicator> GetPmsAnnualObjectiveIndicator (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsIndicator> pmsIndicator = new List<PmsIndicator> ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();

            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM PMS_VI_OBJ_INDICATOR WHERE PMS_OBJ_ID=" + id;
            } else {
                viewName = "SELECT * FROM PMS_VI_OBJ_INDICATOR_VER WHERE PMS_OBJ_ID=" + id + " AND VERSION_ID=" + version_id;
            }

            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        PmsIndicator pmsInd = new PmsIndicator ();
                        pmsInd.question = (dr["PMS_OBJ_IND_QUESTION"] + "");
                        pmsInd.indicator = (dr["PMS_OBJ_IND"] + "");
                        pmsInd.standard = (dr["PMS_OBJ_IND_STANDARD"] + "");
                        pmsInd.periodicity = (dr["PMS_OBJ_IND_PERIODICITY"] + "");
                        pmsInd.disaggregated = (dr["PMS_OBJ_IND_DISAGGREGATED"] + "");
                        pmsInd.technique = (dr["PMS_OBJ_IND_TECHNIQUE"] + "");
                        pmsInd.value = (dr["VALUE"] + "");

                        pmsIndicator.Add (pmsInd);

                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsAnnualObjectiveIndicator", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }

            return pmsIndicator;

        }
    public List<PmsIndicator> GetPmsOutputIndicatorById (int annual_plan_id,int obj_outc_id, int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsIndicator> pmsIndicator = new List<PmsIndicator> ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();
            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM PMS_VI_OUTPUT_INDICATOR WHERE ANNUAL_PLAN_ID="+annual_plan_id+" AND PMS_OBJ_ID ="+obj_outc_id+" AND PMS_OUTP_ID=" + id;
            } else {
                viewName = "SELECT * FROM PMS_VI_OUTPUT_INDICATOR_VER WHERE  ANNUAL_PLAN_ID="+annual_plan_id+"AND OBJ_UTC_ID ="+obj_outc_id+"  AND PMS_OUTP_ID=" + id + " AND VERSION_ID=" + version_id;
            }

            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        PmsIndicator pmsInd = new PmsIndicator ();
                        pmsInd.question = (dr["PMS_OUTP_IND_QUESTION"] + "");
                        pmsInd.indicator = (dr["PMS_OUTP_IND"] + "");
                        pmsInd.standard = (dr["PMS_OUTP_IND_STANDARD"] + "");
                        pmsInd.periodicity = (dr["PMS_OUTP_IND_PERIODICITY"] + "");
                        pmsInd.disaggregated = (dr["PMS_OUTP_IND_DISAGGREGATED"] + "");
                        pmsInd.technique = (dr["PMS_OUTP_IND_TECHNIQUE"] + "");
                        pmsInd.value = (dr["VALUE"] + "");

                        pmsIndicator.Add (pmsInd);

                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsOutputIndicatorById", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }

            return pmsIndicator;

        }
    
        public List<PmsIndicator> GetPmsOutputIndicator (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsIndicator> pmsIndicator = new List<PmsIndicator> ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();
            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM PMS_VI_OUTPUT_INDICATOR WHERE PMS_PROC_OUTP_REL_ID=" + id;
            } else {
                viewName = "SELECT * FROM PMS_VI_OUTPUT_INDICATOR_VER WHERE PMS_PROC_OUTP_REL_ID=" + id + " AND VERSION_ID=" + version_id;
            }

            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        PmsIndicator pmsInd = new PmsIndicator ();
                        pmsInd.question = (dr["PMS_OUTP_IND_QUESTION"] + "");
                        pmsInd.indicator = (dr["PMS_OUTP_IND"] + "");
                        pmsInd.standard = (dr["PMS_OUTP_IND_STANDARD"] + "");
                        pmsInd.periodicity = (dr["PMS_OUTP_IND_PERIODICITY"] + "");
                        pmsInd.disaggregated = (dr["PMS_OUTP_IND_DISAGGREGATED"] + "");
                        pmsInd.technique = (dr["PMS_OUTP_IND_TECHNIQUE"] + "");
                        pmsInd.value = (dr["VALUE"] + "");

                        pmsIndicator.Add (pmsInd);

                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsOutputIndicator", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }

            return pmsIndicator;

        }
        public PmsOverallGoal GetPmsAnnualOverallGoal (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            PmsOverallGoal pmsOverallGoal = new PmsOverallGoal ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();
            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM PMS_VI_OVERALL_GOAL WHERE PMS_AP_ID=" + id;
            } else {
                viewName = "SELECT * FROM PMS_VI_OVERALL_GOAL_VER WHERE PMS_AP_ID=" + id + " AND VERSION_ID=" + version_id;
            }
            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        pmsOverallGoal.target = (dr["PMS_OVERALL_GOAL_TARGET_DESC"] + "");
                        pmsOverallGoal.goal = dr["PMS_OVERALL_GOAL_DESC"] + "";
                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsAnnualPlanOverallGoal", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }
            return pmsOverallGoal;
        }

        public List<PmsSSS> GetPmsAnnualSSS (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsSSS> retList = new List<PmsSSS> ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();
            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM PMS_VI_STRAT_SUPP_SERV WHERE PMS_AP_ID=" + id;
            } else {
                viewName = "SELECT * FROM PMS_VI_STRAT_SUPP_SERV_VER WHERE PMS_AP_ID=" + id + " AND VERSION_ID=" + version_id;
            }
            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        PmsSSS pmsSSS = new PmsSSS ();

                        pmsSSS.description = (dr["PMS_STRAT_SUPP_SERV_DESC"] + "");
                        pmsSSS.id = dr["PMS_SSS_KEY_ID"] + "";
                        retList.Add (pmsSSS);
                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsAnnualPlanSSS", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }
            return retList;
        }

        public List<PmsCC> GetPmsAnnualCC (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsCC> retList = new List<PmsCC> ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();
            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM PMS_VI_CC WHERE PMS_AP_ID=" + id;
            } else {
                viewName = "SELECT * FROM PMS_VI_CC_VER WHERE PMS_AP_ID=" + id + " AND VERSION_ID=" + version_id;
            }
            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        PmsCC pmsCC = new PmsCC ();

                        pmsCC.description = (dr["PMS_CC_DESC"] + "");
                        pmsCC.id = dr["PMS_CC_KEY_ID"] + "";
                        retList.Add (pmsCC);
                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsAnnualPlanCC", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }
            return retList;
        }

        public List<PmsCCP> GetPmsAnnualCCP (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsCCP> retList = new List<PmsCCP> ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();
            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM PMS_VI_CCP WHERE PMS_AP_ID=" + id;
            } else {
                viewName = "SELECT * FROM PMS_VI_CCP_VER WHERE PMS_AP_ID=" + id + " AND VERSION_ID=" + version_id;
            }
            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        PmsCCP pmsCCP = new PmsCCP ();

                        pmsCCP.description = (dr["PMS_CCP_DESC"] + "");
                        pmsCCP.id = dr["PMS_CCP_KEY_ID"] + "";
                        retList.Add (pmsCCP);
                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsAnnualPlanCCP", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }
            return retList;
        }

        private void AddSSSIndicator (int id, StringBuilder documentText, int version_id) {
            List<PmsIndicator> sssIndicators = GetPmsSSSIndicator (id, version_id);
            //var newPara = documentText.AddParagraph() as WParagraph;
            StringBuilder htmlN = new StringBuilder ();
            htmlN.Append ("<table width='100%' style='border:1px'>");

            htmlN.Append ("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
            htmlN.Append ("<td>");
            htmlN.Append ("QUESTION");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("INDICATOR");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("STANDARD");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("TECHNIQUE");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("PERIODICITY");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("DISAGGREGATED");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("END DATE");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("VALUE");
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            for (int i = 0; i < sssIndicators.Count; i++) {
                htmlN.Append ("<tr>");
                PmsIndicator objIndic = sssIndicators[i];
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.question);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.indicator);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.standard);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.technique);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.periodicity);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.disaggregated);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.endDate);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.value);
                htmlN.Append ("</td>");
                htmlN.Append ("</tr>");

            }
            htmlN.Append ("</table>");
            documentText.Append (htmlN.ToString ());
        }

        private void AddCCIndicator (int id, StringBuilder documentText, int version_id) {
            List<PmsIndicator> ccsIndicators = GetPmsCCIndicator (id, version_id);
            //var newPara = documentText.AddParagraph() as WParagraph;
            StringBuilder htmlN = new StringBuilder ();
            htmlN.Append ("<table width='100%' style='border:1px'>");

            htmlN.Append ("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
            htmlN.Append ("<td>");
            htmlN.Append ("QUESTION");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("INDICATOR");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("STANDARD");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("TECHNIQUE");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("PERIODICITY");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("DISAGGREGATED");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("END DATE");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("VALUE");
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            for (int i = 0; i < ccsIndicators.Count; i++) {
                htmlN.Append ("<tr>");
                PmsIndicator objIndic = ccsIndicators[i];
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.question);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.indicator);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.standard);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.technique);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.periodicity);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.disaggregated);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.endDate);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.value);
                htmlN.Append ("</td>");
                htmlN.Append ("</tr>");

            }
            htmlN.Append ("</table>");
            documentText.Append (htmlN.ToString ());
        }

        private void AddCCPIndicator (int id, StringBuilder documentText, int version_id) {
            List<PmsIndicator> ccpIndicators = GetPmsCCPIndicator (id, version_id);
            //var newPara = documentText.AddParagraph() as WParagraph;
            StringBuilder htmlN = new StringBuilder ();
            htmlN.Append ("<table width='100%' style='border:1px'>");

            htmlN.Append ("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
            htmlN.Append ("<td>");
            htmlN.Append ("QUESTION");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("INDICATOR");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("STANDARD");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("TECHNIQUE");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("PERIODICITY");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("DISAGGREGATED");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("END DATE");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("VALUE");
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            for (int i = 0; i < ccpIndicators.Count; i++) {
                htmlN.Append ("<tr>");
                PmsIndicator objIndic = ccpIndicators[i];
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.question);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.indicator);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.standard);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.technique);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.periodicity);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.disaggregated);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.endDate);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.value);
                htmlN.Append ("</td>");
                htmlN.Append ("</tr>");

            }
            htmlN.Append ("</table>");
            documentText.Append (htmlN.ToString ());
        }

        public List<PmsIndicator> GetPmsSSSIndicator (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsIndicator> pmsIndicator = new List<PmsIndicator> ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();
            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM PMS_VI_STRAT_SUPP_SERV_INDICATOR WHERE PMS_SSS_KEY_ID=" + id;
            } else {
                viewName = "SELECT * FROM PMS_VI_STRAT_SUPP_SERV_INDICATOR_VER WHERE PMS_SSS_KEY_ID=" + id + " AND VERSION_ID=" + version_id;
            }

            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    String[] format = { "yyy-MM-dd" };
                    while (dr.Read ()) {

                        PmsIndicator pmsInd = new PmsIndicator ();
                        pmsInd.question = (dr["PMS_SSS_IND_QUESTION"] + "");
                        pmsInd.indicator = (dr["PMS_SSS_IND"] + "");
                        pmsInd.standard = (dr["PMS_SSS_IND_STANDARD"] + "");
                        pmsInd.periodicity = (dr["PMS_SSS_IND_PERIODICITY"] + "");
                        pmsInd.disaggregated = (dr["PMS_SSS_IND_DISAGGREGATED"] + "");
                        pmsInd.technique = (dr["PMS_SSS_IND_TECHNIQUE"] + "");
                        DateTime dt = DateTime.MinValue;

                        DateTime.TryParse (dr["PMS_AP_SSS_END_DATE"] + "", out dt);
                        if (dt != DateTime.MinValue)
                            pmsInd.endDate = dt.ToString (format[0], DateTimeFormatInfo.InvariantInfo);
                        else
                            pmsInd.endDate = "";

                        pmsInd.value = (dr["VALUE"] + "");

                        pmsIndicator.Add (pmsInd);

                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsSSSIndicator", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }

            return pmsIndicator;

        }

        public List<PmsIndicator> GetPmsCCIndicator (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsIndicator> pmsIndicator = new List<PmsIndicator> ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();
            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM PMS_VI_CC_INDICATOR WHERE PMS_CC_KEY_ID=" + id;
            } else {
                viewName = "SELECT * FROM PMS_VI_CC_INDICATOR_VER WHERE PMS_CC_KEY_ID=" + id + " AND VERSION_ID=" + version_id;
            }

            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    String[] format = { "yyy-MM-dd" };
                    while (dr.Read ()) {

                        PmsIndicator pmsInd = new PmsIndicator ();
                        pmsInd.question = (dr["PMS_CC_IND_QUESTION"] + "");
                        pmsInd.indicator = (dr["PMS_CC_IND"] + "");
                        pmsInd.standard = (dr["PMS_CC_IND_STANDARD"] + "");
                        pmsInd.periodicity = (dr["PMS_CC_IND_PERIODICITY"] + "");
                        pmsInd.disaggregated = (dr["PMS_CC_IND_DISAGGREGATED"] + "");
                        pmsInd.technique = (dr["PMS_CC_IND_TECHNIQUE"] + "");
                        DateTime dt = DateTime.MinValue;

                        DateTime.TryParse (dr["PMS_AP_CC_END_DATE"] + "", out dt);
                        if (dt != DateTime.MinValue)
                            pmsInd.endDate = dt.ToString (format[0], DateTimeFormatInfo.InvariantInfo);
                        else
                            pmsInd.endDate = "";

                        pmsInd.value = (dr["VALUE"] + "");

                        pmsIndicator.Add (pmsInd);

                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsCCIndicator", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }

            return pmsIndicator;

        }

        public List<PmsIndicator> GetPmsCCPIndicator (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsIndicator> pmsIndicator = new List<PmsIndicator> ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();
            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM PMS_VI_CCP_INDICATOR WHERE PMS_CCP_KEY_ID=" + id;
            } else {
                viewName = "SELECT * FROM PMS_VI_CCP_INDICATOR_VER WHERE PMS_CCP_KEY_ID=" + id + " AND VERSION_ID=" + version_id;
            }

            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    String[] format = { "yyy-MM-dd" };
                    while (dr.Read ()) {

                        PmsIndicator pmsInd = new PmsIndicator ();
                        pmsInd.question = (dr["PMS_CCP_IND_QUESTION"] + "");
                        pmsInd.indicator = (dr["PMS_CCP_IND"] + "");
                        pmsInd.standard = (dr["PMS_CCP_IND_STANDARD"] + "");
                        pmsInd.periodicity = (dr["PMS_CCP_IND_PERIODICITY"] + "");
                        pmsInd.disaggregated = (dr["PMS_CCP_IND_DISAGGREGATED"] + "");
                        pmsInd.technique = (dr["PMS_CCP_IND_TECHNIQUE"] + "");
                        DateTime dt = DateTime.MinValue;

                        DateTime.TryParse (dr["PMS_AP_CCP_END_DATE"] + "", out dt);
                        if (dt != DateTime.MinValue)
                            pmsInd.endDate = dt.ToString (format[0], DateTimeFormatInfo.InvariantInfo);
                        else
                            pmsInd.endDate = "";

                        pmsInd.value = (dr["VALUE"] + "");

                        pmsIndicator.Add (pmsInd);

                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsCCPIndicator", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }

            return pmsIndicator;

        }

        public List<PmsNarrative> GetPmsAnnualPlanNarrative (int id, string table_name, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsNarrative> pmsNarrative = new List<PmsNarrative> ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();
            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM IMS_VI_NARRATIVE_BY_TYPE WHERE TABLE_NAME='" + table_name + "' AND REFERENCE_ID=" + id + " ORDER BY CODE";
            } else {
                viewName = "SELECT * FROM IMS_VI_NARRATIVE_BY_TYPE_VER WHERE TABLE_NAME='" + table_name + "' AND REFERENCE_ID=" + id + " AND VERSION_ID=" + version_id + " ORDER BY CODE";;
            }

            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        PmsNarrative pmsNarr = new PmsNarrative ();
                        pmsNarr.descr = (dr["DESCR"] + "");
                        pmsNarr.section_text = dr["SECTION_TEXT"] + "";
                        pmsNarr.section_title = dr["SECTION_TITLE"] + "";
                        pmsNarrative.Add (pmsNarr);

                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsAnnualPlanNarrative", ex, UserId, "id=" + id + ";v_id=" + version_id + ";tn=" + table_name, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }

            return pmsNarrative;

        }

        public List<PmsOverviewObjective> GetPmsAnnualPlanObjectiveOutput (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsOverviewObjective> pmsObjective = new List<PmsOverviewObjective> ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();
            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_OUTCOME_OBJECTIVE_CATEGORY_OUTPUT_PRINT WHERE ANNUAL_PLAN_ID=" + id;
            } else {
                viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_OUTCOME_OBJECTIVE_CATEGORY_OUTPUT_PRINT WHERE ANNUAL_PLAN_ID=" + id + " AND VERSION_ID=" + version_id;
            }

            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        PmsOverviewObjective pmsObj = new PmsOverviewObjective ();
                        pmsObj.id = (dr["ID"] + "");
                        pmsObj.description = (dr["DESCRIPTION"] + "");
                        pmsObj.category_code = dr["CATEGORY_CODE"] + "";
                        pmsObj.output = dr["OUTPUT"] + "";
                        pmsObj.output_id = dr["OUTPUT_ID"] + "";
                        pmsObj.outcome = dr["OUTCOME"] + "";
                        pmsObj.category = dr["CATEGORY"] + "";
                        pmsObjective.Add (pmsObj);

                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsAnnualPlanObjectiveOutput", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }
            return pmsObjective;
        }

        public List<PmsObjective> GetPmsAnnualPlanObjective (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsObjective> pmsObjective = new List<PmsObjective> ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();


            //Print Out Annual Plan OVERALL GOAL
            //1- OUTCOME-BASED OBJECTIVES
            //2- PROGRAMME CATEGORY OF INTERVENTION (CoI)
            //3- N° People Served F
            //4- N° People Served M
            //5- TOTAL X year BUDGET [EURO or USD]

            string viewName = "SELECT * FROM  PME.AnnualPlanOutcomeObjectiveIndicatorBudget where ID=" + id+ "Order by OUTCOME"; ;
            
            
            //if (version_id == 0) {
            //    viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_OUTCOME_OBJECTIVE_CATEGORY_PRINT WHERE ANNUAL_PLAN_ID=" + id;
            //} else {
            //    viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_OUTCOME_OBJECTIVE_PRINT_VER WHERE ANNUAL_PLAN_ID=" + id + " AND VERSION_ID=" + version_id;
            //}

            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        PmsObjective pmsObj = new PmsObjective ();
                        pmsObj.id = (dr["ID"] + "");
                        //pmsObj.description = (dr["DESCRIPTION"] + "");
                        //pmsObj.served_people = dr["SERVED_PEOPLE"] + "";
                        //pmsObj.currency = dr["CURRENCY_REAL"] + "";

                        pmsObj.female_indicator = dr["female_indicator"] + "";
                        pmsObj.male_indicator = dr["male_indicator"] + "";
                        pmsObj.outcome_id = dr["outcome_id"] + "";
                        pmsObj.currency = dr["CURRENCY"] + "";
                        pmsObj.total_gender_indicator = dr["total_gender_indicator"] + "";
                        pmsObj.real_budget = dr["BUDGET_EST"] + "";
                        //pmsObj.real_budget = dr["BUDG_REAL"] + "";
                        pmsObj.outcome = dr["OUTCOME"] + "";
                        //pmsObj.target = dr["PMS_TARGET_DESC"] + "";
                        pmsObj.category = dr["CATEGORY"] + "";
                        pmsObj.total_cat = dr["Total_Cat"] + "";
                        pmsObjective.Add (pmsObj);

                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsAnnualPlanObjective", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }
            return pmsObjective;
        }

        public List<PmsObjective> GetPmsAnnualPlanOutcomeObjective (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsObjective> pmsObjective = new List<PmsObjective> ();

            PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan ();
            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_OUTCOME_OBJECTIVE WHERE ANNUAL_PLAN_ID=" + id;
            } else {
                viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_OUTCOME_OBJECTIVE_VER WHERE ANNUAL_PLAN_ID=" + id + " AND VERSION_ID=" + version_id;
            }

            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        PmsObjective pmsObj = new PmsObjective ();
                        pmsObj.id = (dr["ID"] + "");
                        pmsObj.description = (dr["PMS_OBJ_OUTCOME"] + "");
                        // pmsObj.served_people = dr["SERVED_PEOPLE"] + "";
                        // pmsObj.currency = dr["CURRENCY_REAL"] + "";
                        // pmsObj.estimated_budget = dr["BUDGET_EST"] + "";
                        // pmsObj.real_budget = dr["BUDG_REAL"] + "";
                        // pmsObj.outcome = dr["OUTCOME"] + "";
                        pmsObj.target = dr["PMS_TARGET_DESC"] + "";
                        // pmsObj.category = dr["CATEGORY"] + "";
                        pmsObjective.Add (pmsObj);

                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsAnnualPlanOutcomeObjective", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }
            return pmsObjective;
        }

        public List<PmsService> GetPmsService (int id, int version_id) {
            ImsUtility utility = new ImsUtility ();
            List<PmsService> PmsService = new List<PmsService> ();

            string viewName = "";
            if (version_id == 0) {
                viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_SERVICE WHERE IMS_LANGUAGE_LOCALE='en' and OBJECTIVE_ID=" + id + " ORDER BY ID";
            } else {
                viewName = "SELECT * FROM PMS_VI_ANNUAL_PLAN_SERVICE_VER WHERE IMS_LANGUAGE_LOCALE='en' and OBJECTIVE_ID=" + id + " AND VERSION_ID=" + version_id + " ORDER BY ID";;
            }

            SqlDataReader dr = null;

            try {
                UserId = User.Identity.Name;
                //IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection () as SqlConnection) {
                    conn.ConnectionString = _connectionString;
                    if (conn.State != ConnectionState.Open) conn.Open ();
                    SqlCommand sqlcmd = new SqlCommand ();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.Text;

                    sqlcmd.CommandText = viewName;

                    dr = sqlcmd.ExecuteReader ();
                    while (dr.Read ()) {

                        PmsService pmsSrv = new PmsService ();
                        pmsSrv.id = (dr["ID"] + "");
                        pmsSrv.title = (dr["TITLE"] + "");
                        pmsSrv.start_date = (dr["START_DATE"] + "");
                        pmsSrv.end_date = (dr["END_DATE"] + "");
                        pmsSrv.category_code = (dr["PMS_COI_CODE"] + "");
                        pmsSrv.served_people = dr["SERVED_PEOPLE"] + "";
                        pmsSrv.category = dr["COI_LABEL"] + "";
                        pmsSrv.type_of_service = dr["PMS_TOS_DESCRIPTION"] + "";
                        pmsSrv.periodicity = dr["OC_TYPE"] + "";
                        pmsSrv.location = dr["LOCATION_DETAILS"] + "";
                        PmsService.Add (pmsSrv);

                    }
                }
            } catch (Exception ex) {

                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "GetPmsService", ex, UserId, "id=" + id + ";v_id=" + version_id, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();

            }
            return PmsService;
        }
        #endregion DB

        [HttpGet ("{id}")]
        [Authorize]
        public async Task<ActionResult<byte[]>> CreateDocWithTOC (int id, int version_id) {
            try {

                PmsAnnualPlan pmsAnnualPlanMainData = GetPmsAnnualPlanMainData (id, version_id);
                StringBuilder documentText = new StringBuilder ();
                documentText.Append ("<html><body style='width:960px;'>");
                //Creates new WPicture instance and gets Image from file
                // WPicture picture = new WPicture(document);
                byte[] imagelogo = System.IO.File.ReadAllBytes ("img/logojrs.png");
                string imgData = Convert.ToBase64String (imagelogo);
                documentText.Append ("<p align=\"left\"><img title='LogoJRS.png' width=\"300\" src='data:image/png;base64,");
                documentText.Append (imgData);
                documentText.Append ("' /></p></br>");
                // picture.LoadImage(imagelogo);
                // picture.Height = 50;
                // picture.Width = 200;
                // WParagraph para = document.LastParagraph;
                // if (para != null)
                //     para.ChildEntities.Add(picture);
                // StringBuilder documentText = document.LastdocumentText;
                // para = documentText.AddParagraph() as WParagraph;
                // para.AppendText("Project Annual Plan");
                // para.AppendText("\r");

                string html = @"
              
                <table  width='80%' 
                style='border-collapse:collapse;
                background-color: #134388;
                border-color: #004388;
                font-color:#FFFFFF;
                font-size:18px;
                font-family:Calibri;
                font-weight:bold;'
                color='#FFFFFF'
                font='Calibri'
                border=0
                
                >
      
        <tr>
        <td>Project Name:</td>
        <td>" + pmsAnnualPlanMainData.descr + @"</td>
        </tr>

<tr>
         <td>Project Code:</td>
        <td>" + pmsAnnualPlanMainData.code + @"</td>
        </tr>


        <tr>
        <td>Location:</td>
        <td>" + pmsAnnualPlanMainData.location_description + @"</td>
        </tr>
         <tr>
        <td>Country:</td>
        <td>" + pmsAnnualPlanMainData.country + @"</td>
        </tr>
        <tr>
        <td>Project Start Date:</td>
        <td>" + pmsAnnualPlanMainData.start_year + @"</td>
        </tr>
        <tr>
        <td>Project End Date:</td>
        <td>" + pmsAnnualPlanMainData.end_year + @"</td>
        </tr>
        </table><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/>";

                html += "<p align=\"center\"><table  width=\"80%\"  border=0  cellspacing=0 cellpadding=0><tr style=\"font-weight:bold\" color=\"#134388\">";
                html += @"
         <td>Submitted by:</td>
        <td>To be implemented by:</td>
        </tr>
        <tr>
        <td>[Regional Director]</td>
        <td>[Country Director]</td>
        </tr>

        <tr>
        <td>[Region]</td>
        <td>[Country]</td>
        </tr>
         <tr>
        <td>[Phone / Fax]</td>
        <td>[Phone / Fax]</td>
        </tr>
        <tr>
        <td>[E-mail]</td>
        <td>[E-mail]</td>
        </tr>
        </table>
        </p>";
                documentText.Append (html);
                documentText.Append ("<p style='page-break-before: always' />");

                AddHeadingNoTable (documentText, "h1", "Project executive summary");
                AddDiv (documentText, @"The Executive Summary is a snapshot of the project, presenting the salient parts of the design phase of the project. The reader will have the opportunity to understand the project purpose and its rationale through the summary included in this section.
Please also note that the Project Executive Summary is frequently used as a concept note to share with potential funding partners, before sending the full Project Annual Plan. Therefore, it is important that this section is complete and comprehensible.
</div>");
                //  AddHeading (documentText, "h2", "Project Description");
                AddNarrative (id, "PMS_PROJECT", documentText, 1, version_id);

                //AddHeading (documentText, "h1", "Project Design");
                AddHeading (documentText, "h2", "Overall Goal");
                AddOverallGoal (id, documentText, version_id);
                // AddHeading (documentText, "h3", "Overall Goal Indicators");
                // AddOverallGoalIndicator (id, documentText, version_id);

               // AddHeading (documentText, "h2", "Outcome-based Objectives");

                AddObjective (id, documentText, version_id);
                documentText.Append ("<p " + GetCaptiontyle () + ">Note: this Project Plan should be read in conjunction with the Project Budget which contains the financial detail pertaining to this plan. The budget amounts in the above table should be sourced from the Total column (A+B+C) of the Budget Summary worksheet.</p>");
                documentText.Append("<p " + GetCaptiontyle() + ">If there is a large gender disparity between the F/M beneficiaries being targeted and served through this project, please provide an explanation.</p>");


                AddHeadingNoTable (documentText, "h1", "Problem identification and analysis");
                AddHeadingNoTable (documentText, "h2", "Context: Problems and root causes");
                AddDiv (documentText, "Analyse the situation that will be addressed by the project. If possible include relevant facts, statistics and a map as appendices.");
                AddHeadingNoTable (documentText, "h2", "Rationale: Site and project population selection");
                AddDiv (documentText, @"Explain and justify the boundaries (geographic and thematic) that specify the location of the intervention; describe the estimated total population who will benefit and its socio-economic characteristics (ie. ethnic group, religion, women’s status, livelihoods); describe the process used in selecting the site and the participants and justify why this site and these population was chosen.
Include a map showing the proposed project site.");
                AddHeadingNoTable (documentText, "h2", "Past performance");
                AddDiv (documentText, @"
Provide the history of JRS presence in the country where the project will take place, including partnership with other organisations involved with forcibly displaced people and local communities in the project area.");

                documentText.Append ("<p style='page-break-before: always' />");
                AddHeadingNoTable (documentText, "h1", "Project Design");
                AddHeadingNoTable (documentText, "h2", "Overall Goal");
                AddDiv (documentText, @"
State the Overall Goal of the project: it is the statement of a desired state which is usually general, abstract and long-term towards which project activities contribute.");
                AddHeadingNoTable (documentText, "h2", "Logical Framework, Monitoring and Evaluation");
                AddHeadingNoTable (documentText, "h3", @"Outcome-based objectives, Results Indicators, Processes, M&E plans
[in Appendix] ");
                // AddDiv (documentText, @"OVERVIEW TABLE [Appendix 1]");
                AddMonitoringAndEvaluationTable (id, documentText, version_id);
                AddHeadingNoTable (documentText, "h2", "Addressing gender equality in practice");
                AddDiv (documentText, @"
Examine if the project promotes gender equality and equal opportunities. Describe the system put in place to ensure the project fulfills the requirements of equal participation of men and women, women protection from sexual gender based violence or exploitation, provides equal access to services.");
                AddHeadingNoTable (documentText, "h2", "Environment");

                AddDiv (documentText, @"
Describe if the project encourages communication and cooperation between all the actors involved in environmental decisions (i.e. reduction of risks of environmental degradation in refugee camps), including UN agencies, government officials, private businesses, and citizens.");

                documentText.Append ("<p style='page-break-before: always' />");
                AddHeadingNoTable (documentText, "h1", "Monitoring and Evaluation System (M&E)");
                AddHeadingNoTable (documentText, "h2", "Use of Monitoring and Evaluation results");
                AddDiv (documentText, @"
This section of the Project Annual Plan should describe the monitoring and evaluation of the project. 
For each 'dimension' that has been created in the Logical Framework (i.e. Outcome-based objectives, Outputs, Crosscutting Processes, Principles and Criteria) one or more indicators and the techniques to collect them will be established.  
");
                AddDiv (documentText, @"M&E OVERVIEW TABLE [Appendix 2]");
                AddMonitoringAndEvaluationResults (id, documentText, version_id);
                documentText.Append ("<p style='page-break-before: always' />");
                AddHeadingNoTable (documentText, "h1", "Project Organisational Structure and Staffing");
                AddHeadingNoTable (documentText, "h2", "Human resources requirements");
                AddDiv (documentText, @"
Identify the key staff positions for the project and briefly describe if volunteers or in-kind labour by participants are to be involved.");

                AddHeadingNoTable (documentText, "h2", "Organisational structure");
                AddDiv (documentText, @"
Include a simple organizational chart showing supervisory or other linkages between the project personnel and organizational structures. If necessary, include a brief narrative to explain.
");
                documentText.Append ("<p style='page-break-before: always' />");
                AddHeadingNoTable (documentText, "h1", "Project Feasibility and Exit Strategy");
                AddHeadingNoTable (documentText, "h2", "Project feasibility and coordination");
                AddDiv (documentText, @"Explain how JRS coordinates with the host government, UNHCR and other national or international NGOs to ensure complementarities, prevent duplication and to maximise resources.");
                AddHeadingNoTable (documentText, "h2", "Exit Strategies");
                AddDiv (documentText, @"Provide information on the exit strategies to ensure the on-going and long-term plans of the project activities.");
                documentText.Append ("<p style='page-break-before: always' />");
                AddHeadingNoTable (documentText, "h1", "Potential negative impact of the project");

                AddHeadingNoTable (documentText, "h1", "Risks and Mitigation");

                AddDiv (documentText, @"Describe if the risk of negative impacts on peoples’ security, environment, local economies etc. has been anticipated and removed or reduced and describe the actions you envisage to mitigate the risks.
");
                AddRiskMitigationTable (documentText);

                // for (int i = 0; i < projectObjectives.Count; i++) {

                //     PmsObjective projectObjective = projectObjectives[i];
                //     AddHeading (documentText, "h2", "Outcome-based Objectives");
                //     AddHeading (documentText, "h3", "Objective:" + projectObjective.description);
                //     int idobj = int.Parse (projectObjective.id);
                //     AddObjectiveDetails (i, documentText);
                //     AddNarrative (idobj, "PMS_OBJECTIVE", documentText, 2, version_id);
                //     AddHeading (documentText, "h3", "Objective Indicators");
                //     AddObjectiveIndicator (idobj, documentText, version_id);
                //     AddHeading (documentText, "h3", "Services");
                //     List<PmsService> services = GetPmsService (idobj, version_id);
                //     for (int j = 0; j < services.Count; j++) {
                //         PmsService service = services[j];
                //         AddHeading (documentText, "h4", "Service:" + service.title);
                //         AddServiceDetails (service, documentText);
                //         int srvId = int.Parse (service.id);
                //         AddNarrative (srvId, "PMS_SERVICE", documentText, 4, version_id);
                //         List<PmsProcess> processes = GetPmsProcess (srvId, version_id);
                //         for (int k = 0; k < processes.Count; k++) {
                //             PmsProcess process = processes[k];
                //             AddHeading (documentText, "h5", "Process:" + (k + 1));
                //             AddProcessDetails (process, documentText);
                //             int prcId = int.Parse (process.id);
                //             AddNarrative (prcId, "PMS_ACTIVITY", documentText, 5, version_id);

                //             List<PmsOutput> outputs = GetPmsOutput (prcId, version_id);
                //             for (int z = 0; z < outputs.Count; z++) {
                //                 PmsOutput output = outputs[z];
                //                 AddHeading (documentText, "h6", "Output:" + (z + 1));
                //                 AddOutputDetails (output, documentText);
                //                 int outpId = int.Parse (output.id);
                //                 AddHeading (documentText, "h7", "Output Indicators");
                //                 AddOutputIndicator (outpId, documentText, version_id);

                //             }
                //         }
                //     }

                // }
                // AddCC (id, documentText, version_id);
                // //AddCCP(id, documentText, version_id);
                // AddStrategicAndSupportServices (id, documentText, version_id);
                // documentText.Append ("</body></html>");
                // Generate .docx
                DocCreatorService docCreator = new DocCreatorService ();
                Byte[] retBytes = docCreator.CreateDocxFromHtml (documentText.ToString ()).ToArray ();

                // Return Byte array
                //return StatusCode(200, retBytes);

                //         BinaryWriter Writer = null;
                // string Name = @"fileTOC1.doc";

                // try
                // {
                //     // Create a new stream to write to the file
                //     Writer = new BinaryWriter(System.IO.File.OpenWrite(Name));

                //     // Writer raw data                
                //     Writer.Write(retBytes);
                //     Writer.Flush();
                //     Writer.Close();
                // }
                // catch 
                // {
                //     //...
                //     return null;
                // }

                return StatusCode (200, retBytes);
            } catch (Exception ex) {
                //@TODO: Implementare Log
                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocController", "CreateDocWithTOC", ex, UserId, new { idap = id }, IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();
                return StatusCode (500);
            }
        }

        private static void AddDivJustified (StringBuilder html, string txt) {

            html.Append (@"<div style=' text-align: justify;text-justify: inter-word;  font-size:12px;
                font-family:Calibri;'>" + txt + "</div>");

        }
        private static void AddDiv (StringBuilder html, string txt) {

            html.Append (@"<div style='font-size:12px;text-align:left;
                font-family:Calibri;'>" + txt + "</div>");

        }
        private static void AddHeadingNoTable (StringBuilder txt, string style, string title) {

            switch (style) {
                case "h1":
                    txt.Append (@"<table width='100%' 
 
                style='font-size:18px;
                font-family:Calibri;'
               border=0>");
                    txt.Append ("<tr><td>");
                    txt.Append ("<" + style + ">" + title + "</" + style + ">");
                    txt.Append ("</td></tr></table>");
                    break;
                case "h2":
                    txt.Append (@"<table width='100%' 
 
                style='font-size:16px;
                font-family:Calibri;'
               border=0>");
                    txt.Append ("<tr><td>");
                    txt.Append ("<" + style + ">" + title + "</" + style + ">");
                    txt.Append ("</td></tr></table>");
                    break;
                case "h3":
                    txt.Append (@"<table width='100%' 
 
                style='font-size:14px;
                font-family:Calibri;'
               border=0>");
                    txt.Append ("<tr><td>");
                    txt.Append ("<" + style + ">" + title + "</" + style + ">");
                    txt.Append ("</td></tr></table>");
                    break;
                case "default":
                    txt.Append (@"<table width='100%' 
 
                style='font-size:18px;
                font-family:Calibri;'
               border=0>");
                    txt.Append ("<tr><td>");
                    txt.Append ("<" + style + ">" + title + "</" + style + ">");
                    txt.Append ("</td></tr></table>");
                    break;
            }

            //return txt;
        }
        private static void AddHeading (StringBuilder txt, string style, string title) {

            switch (style) {
                case "h1":
                    txt.Append (@"<table width='100%' 
                style='font-color:#FFFFFF;
                background-color: #134388;
                font-size:14px;
                font-family:Montserrat;
                font-weight:bold;'
                color='#FFFFFF' border=0>");
                    txt.Append ("<tr><td>");
                    txt.Append ("<" + style + ">" + title + "</" + style + ">");
                    txt.Append ("</td></tr></table>");
                    break;
                case "h2":
                    txt.Append (@"<table width='100%' 
                style='font-color:#FFFFFF;
                background-color: #134388;
                font-size:12px;
                font-family:Montserrat;
                font-weight:bold;'
                color='#FFFFFF' border=0>");
                    txt.Append ("<tr><td>");
                    txt.Append ("<" + style + ">" + title + "</" + style + ">");
                    txt.Append ("</td></tr></table>");
                    break;
                case "h3":
                    txt.Append (@"<table width='100%' 
                style='font-color:#FFFFFF;
                background-color: #134388;
                font-size:14px;
                font-family:Calibri;
                font-weight:bold;'
                color='#FFFFFF' border=0>");
                    txt.Append ("<tr><td>");
                    txt.Append ("<" + style + ">" + title + "</" + style + ">");
                    txt.Append ("</td></tr></table>");
                    break;
                default:
                    txt.Append (@"<table width='100%' 
                style='font-color:#FFFFFF;
                background-color: #134388;
                font-size:18px;
                font-family:Calibri;
                font-weight:bold;'
                color='#FFFFFF' border=0>");
                    txt.Append ("<tr><td>");
                    txt.Append ("<" + style + ">" + title + "</" + style + ">");
                    txt.Append ("</td></tr></table>");
                    //return txt;
                    break;
            }
        }

        public void AddMonitoringAndEvaluationTable (int id, StringBuilder htmlN, int version_id) {
            PmsOverallGoal projectGoal = GetPmsAnnualOverallGoal (id, version_id);
            List<PmsOverviewObjective> projectObjectives = GetPmsAnnualPlanObjectiveOutput (id, version_id);
            List<PmsSSS> projectSSSs = GetPmsAnnualSSS (id, version_id);
            List<PmsCC> projectCCs = GetPmsAnnualCC (id, version_id);
            htmlN.Append (value: "<table width='100%' style='border:1px;border-color:'#FAFAFA>");

            htmlN.Append ("<tr " + GetTableHeadRowStyle () + ">");
            htmlN.Append ("<td colspan=4>");
            htmlN.Append ("Overall Goal: ");
            htmlN.Append (projectGoal.goal);
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            htmlN.Append ("<tr " + GetTableHeadRowStyle () + ">");
            htmlN.Append ("<td>");
            htmlN.Append ("Outcome-b Obj.");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("Short Description");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("Category (Code)");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("Outputs");
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            if (projectObjectives != null && projectObjectives.Count > 0) {
                var objs = projectObjectives.GroupBy (x => x.outcome).ToList ();
                if (objs != null && objs.Count > 0) {
                    for (int i = 0; i < objs.Count; i++) {
                        var obj = objs[i];
                        htmlN.Append ("<tr><td>");
                        htmlN.Append (obj.Key);
                        htmlN.Append ("</td>");
                        var categories = obj.ToList ();
                        htmlN.Append ("<td>");
                        htmlN.Append (categories[0].description);

                        htmlN.Append ("</td>");
                        var ctgs = categories.GroupBy (x => x.category_code).ToList ();

                        string ctc_code = "";
                        List<string> outps = new List<string> ();
                        if (ctgs != null && ctgs.Count > 0) {
                            htmlN.Append ("<td>");
                            for (int k = 0; k < ctgs.Count; k++) {
                                if (k == ctgs.Count - 1) {
                                    ctc_code += ctgs[k].Key;
                                } else {
                                    ctc_code += ctgs[k].Key + "<br/>";
                                }
                                var _outp = ctgs[k].ToList ();
                                for (int z = 0; z < _outp.Count; z++) {
                                    if (!outps.Contains (_outp[z].output)) {
                                        outps.Add (_outp[z].output);
                                    }
                                }

                            }
                            htmlN.Append (value: ctc_code);
                            htmlN.Append (value: "</td>");
                            htmlN.Append ("<td>");
                            string outpstr = "";
                            for (int q = 0; q < outps.Count; q++) {
                                if (q == outps.Count - 1) {
                                    outpstr += outps[q];
                                } else {
                                    outpstr += outps[index : q] + "<br/>";
                                }
                            }
                            htmlN.Append (outpstr);
                            htmlN.Append ("</td>");

                        } else {
                            htmlN.Append ("<td>");
                            htmlN.Append ("</td>");
                            htmlN.Append ("<td>");
                            htmlN.Append ("</td>");

                        }
                        htmlN.Append ("</tr>");
                    }
                } else {
                    htmlN.Append ("<tr>");
                    htmlN.Append ("<td>");
                    htmlN.Append ("</td>");
                    htmlN.Append ("<td>");
                    htmlN.Append ("</td>");
                    htmlN.Append ("<td>");
                    htmlN.Append ("</td>");
                    htmlN.Append ("<td>");
                    htmlN.Append ("</td>");
                    htmlN.Append ("</tr>");
                }
            }

            htmlN.Append ("<tr " + GetTableHeadRowStyle () + ">");
            htmlN.Append ("<td colspan=4>");
            htmlN.Append ("Strategic Support Services ");
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            for (int i = 0; i < projectSSSs.Count; i++) {
                PmsSSS projectSSS = projectSSSs[i];
                htmlN.Append ("<tr>");
                htmlN.Append ("<td colspan=4>");
                htmlN.Append (projectSSS.description);
                htmlN.Append ("</td>");
                htmlN.Append ("</tr>");
            }
            htmlN.Append ("<tr " + GetTableHeadRowStyle () + ">");
            htmlN.Append ("<td colspan=4>");
            htmlN.Append ("Principles, Values, Criteria Cross Cutting");
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            for (int i = 0; i < projectCCs.Count; i++) {
                PmsCC projectCC = projectCCs[i];
                htmlN.Append ("<tr>");
                htmlN.Append ("<td colspan=4>");
                htmlN.Append (projectCC.description);
                htmlN.Append ("</td>");
                htmlN.Append ("</tr>");
            }
            htmlN.Append ("</table>");

        }

        public void AddMonitoringAndEvaluationResults (int id, StringBuilder htmlN, int version_id) {
            PmsOverallGoal projectGoal = GetPmsAnnualOverallGoal (id, version_id);
            List<PmsIndicator> goalIndicators = GetPmsAnnualOverallGoalIndicator (id, version_id);
            int goalRows = goalIndicators.Count;
            if (goalRows == 0) goalRows = 1;

            // List<PmsOverviewObjective> projectObjectives = GetPmsAnnualPlanObjectiveOutput (id, version_id);
            List<PmsObjective> projectObjectives = GetPmsAnnualPlanOutcomeObjective (id, version_id);
            List<PmsOverviewObjective> projectObjectivesOutp = GetPmsAnnualPlanObjectiveOutput (id, version_id);

            List<PmsSSS> projectSSS = GetPmsAnnualSSS (id, version_id);
            List<PmsCC> projectCCs = GetPmsAnnualCC (id, version_id);
            htmlN.Append (value: "<table width='100%' style='border:1px;border-color:'#FAFAFA>");

            htmlN.Append ("<tr " + GetTableHeadRowStyle () + ">");
            htmlN.Append ("<td colspan=1 width='34%'>");
            htmlN.Append ("Overall Goal");
            htmlN.Append ("</td>");
            htmlN.Append ("<td colspan=1 width='33%'>");
            htmlN.Append ("Indicators");
            htmlN.Append ("</td>");
            htmlN.Append ("<td colspan=1 width='33%'>");
            htmlN.Append ("Technique");
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            for (int i = 0; i < goalRows; i++) {
                htmlN.Append ("<tr>");

                if (i == 0) {
                    htmlN.Append ("<td colspan=1 rowspan=" + goalRows + "><b>");
                    htmlN.Append (projectGoal.goal);
                    htmlN.Append ("</b></td>");
                }
                if (goalIndicators.Count > 0) {
                    PmsIndicator goalIndic = goalIndicators[i];
                    htmlN.Append ("<td colspan=1 rowspan=1>");
                    htmlN.Append (goalIndic.indicator);
                    htmlN.Append ("</td>");
                    htmlN.Append ("<td colspan=1 rowspan=1>");
                    htmlN.Append (goalIndic.technique);
                    htmlN.Append ("</td>");
                } else {
                    htmlN.Append ("<td colspan=1 rowspan=1>");

                    htmlN.Append ("</td>");
                    htmlN.Append ("<td colspan=1 rowspan=1>");
                    htmlN.Append ("</td>");
                }
                htmlN.Append ("</tr>");
            }

            for (int i = 0; i < projectObjectives.Count; i++) {
                htmlN.Append ("<tr " + GetTableHeadRowStyle () + ">");
                htmlN.Append ("<td colspan=1 width='34%'>");
                htmlN.Append ("Outcome based objectives ");
                htmlN.Append ("</td>");
                htmlN.Append ("<td colspan=1 width='33%'>");
                htmlN.Append ("Indicators");
                htmlN.Append ("</td>");
                htmlN.Append ("<td colspan=1 width='33%'>");
                htmlN.Append ("Technique");
                htmlN.Append ("</td>");
                htmlN.Append ("</tr>");
                htmlN.Append ("<tr>");
                PmsObjective projectObjective = projectObjectives[index : i];
                htmlN.Append ("<td colspan=3 width='100%'>");
                htmlN.Append ("<b>Target:</b>");
                htmlN.Append (projectObjective.target);
                htmlN.Append ("</td>");
                htmlN.Append ("</tr>");
                List<PmsIndicator> objectiveIndicators = GetPmsAnnualObjectiveIndicator (int.Parse (projectObjective.id), version_id);
                int objIndRows = objectiveIndicators.Count;
                if (objIndRows > 0) {
                    for (int j = 0; j < objectiveIndicators.Count; j++) {
                        htmlN.Append ("<tr>");

                        if (j == 0) {
                            htmlN.Append ("<td colspan=1 rowspan=" + objIndRows + " style='background-color:#F6BE00;'><b>");
                            htmlN.Append (projectObjective.description);
                            htmlN.Append ("</b></td>");
                        }
                        if (objectiveIndicators.Count > 0) {

                            PmsIndicator objIndic = objectiveIndicators[j];
                            htmlN.Append ("<td colspan=1 rowspan=1>");
                            htmlN.Append (objIndic.indicator);
                            htmlN.Append ("</td>");
                            htmlN.Append ("<td colspan=1 rowspan=1>");
                            htmlN.Append (objIndic.technique);
                            htmlN.Append ("</td>");
                        }
                        htmlN.Append ("</tr>");

                    }
                } else {
                    htmlN.Append ("<tr>");
                    htmlN.Append ("<td colspan=1 rowspan=1 style='background-color:#F6BE00;'><b>");
                    htmlN.Append (projectObjective.description);
                    htmlN.Append ("</b></td>");
                    htmlN.Append ("<td colspan=1 rowspan=1>");
                    htmlN.Append ("</td>");
                    htmlN.Append ("<td colspan=1 rowspan=1>");
                    htmlN.Append ("</td>");
                    htmlN.Append ("</tr>");
                }
                htmlN.Append ("<tr " + GetTableHeadRowStyle () + ">");
                htmlN.Append ("<td colspan=1 width='34%'>");
                htmlN.Append ("Outputs");
                htmlN.Append ("</td>");
                htmlN.Append ("<td colspan=1 width='33%'>");
                htmlN.Append ("Indicators");
                htmlN.Append ("</td>");
                htmlN.Append ("<td colspan=1 width='33%'>");
                htmlN.Append ("Technique");
                htmlN.Append ("</td>");
                htmlN.Append ("</tr>");

                List<PmsOverviewObjective> alloutputs = projectObjectivesOutp.Where (x => x.id == projectObjective.id).ToList ();
              var outputs =  alloutputs
      .Select(x => new { x.output, x.output_id }).Distinct().ToList();
                int outpRows = outputs.Count;
                if (outpRows > 0) {

                    for (int q = 0; q < outpRows; q++) {
                        if (outputs[q].output_id != "") {
                            List<PmsIndicator> outputIndicators = GetPmsOutputIndicatorById (id,int.Parse(projectObjective.id), int.Parse (outputs[q].output_id), version_id);
                            int outIndRows = outputIndicators.Count;
                            if (outIndRows > 0) {
                                for (int r = 0; r < outIndRows; r++) {
                                    htmlN.Append ("<tr >");
                                    if (r == 0) {
                                    
                                        htmlN.Append ("<td colspan=1 rowspan=" + outIndRows + " width='34%'>");

                                        htmlN.Append (outputs[q].output);
                                        htmlN.Append ("</td>");
                                    }
                                    htmlN.Append ("<td colspan=1 width='33%'>");
                                    htmlN.Append (outputIndicators[r].indicator);
                                    htmlN.Append ("</td>");
                                    htmlN.Append ("<td colspan=1 width='33%'>");
                                    htmlN.Append (outputIndicators[index : r].technique);
                                    htmlN.Append ("</td>");
                                    htmlN.Append ("</tr>");
                                }
                            } else {
                                htmlN.Append ("<tr >");
                                htmlN.Append ("<td colspan=1 width='34%'>");
                                htmlN.Append (outputs[q].output);
                                htmlN.Append ("</td>");
                                htmlN.Append ("<td colspan=1 width='33%'>");
                                htmlN.Append ("</td>");
                                htmlN.Append ("<td colspan=1 width='33%'>");

                                htmlN.Append ("</td>");
                                htmlN.Append ("</tr>");
                            }
                        } else {
                            if(outputs[q].output != "" || outpRows==1)
                            {
                            htmlN.Append ("<tr >");
                            htmlN.Append ("<td colspan=1 width='34%'>");
                            htmlN.Append (outputs[q].output);
                            htmlN.Append ("</td>");
                            htmlN.Append ("<td colspan=1 width='33%'>");
                            htmlN.Append ("</td>");
                            htmlN.Append ("<td colspan=1 width='33%'>");

                            htmlN.Append ("</td>");
                            htmlN.Append ("</tr>");
                            }
                        }
                    }

                } else {
                    AddBlanchRow(htmlN);
                }
            }



            htmlN.Append ("<tr " + GetTableHeadRowStyle () + ">");
            htmlN.Append ("<td colspan=3>");
            htmlN.Append ("Principles, Values, Criteria Cross Cutting Indicators  ");
            htmlN.Append ("</td>");
            
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            htmlN.Append ("<tr " + GetTableHeadRowStyle () + ">");
            htmlN.Append ("<td colspan=1 width='34%'>");
            htmlN.Append ("Principles, Values…");
            htmlN.Append ("</td>");
            htmlN.Append ("<td colspan=1 width='33%'>");
            htmlN.Append ("Indicators");
            htmlN.Append ("</td>");
            htmlN.Append ("<td colspan=1 width='33%'>");
            htmlN.Append ("Technique");
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            int ccRows = projectCCs.Count;
            if(ccRows>0)
            {
 for (int z = 0; z < ccRows; z++) {

                PmsCC projectCC = projectCCs[z];
                List<PmsIndicator> ccsIndicators = GetPmsCCIndicator (int.Parse(projectCC.id), version_id);
                int ccIndRows=ccsIndicators.Count;
                if(ccIndRows>0)
                {
                    for(int h=0;h<ccIndRows;h++)
                    {
                    htmlN.Append ("<tr >");
                    if(h==0)
                    {
                    htmlN.Append ("<td colspan=1 rowspan="+ccIndRows+" width='34%'>");
                    htmlN.Append (projectCC.description);
                    htmlN.Append ("</td>");
                    }
                    htmlN.Append ("<td colspan=1 width='33%'>");
                    htmlN.Append(ccsIndicators[h].indicator);
                    htmlN.Append ("</td>");
                    htmlN.Append ("<td colspan=1 width='33%'>");
                    htmlN.Append(ccsIndicators[h].technique);
                    htmlN.Append ("</td>");
                    htmlN.Append ("</tr>");
                    }
                }
                else
                {
                    htmlN.Append ("<tr >");
                    htmlN.Append ("<td colspan=1 width='34%'>");
                    htmlN.Append (projectCC.description);
                    htmlN.Append ("</td>");
                    htmlN.Append ("<td colspan=1 width='33%'>");
                    htmlN.Append ("</td>");
                    htmlN.Append ("<td colspan=1 width='33%'>");

                    htmlN.Append ("</td>");
                    htmlN.Append ("</tr>");
                }


            }
            }
            else
            {
                AddBlanchRow(htmlN);
            }



            htmlN.Append ("<tr " + GetTableHeadRowStyle () + ">");
            htmlN.Append ("<td colspan=3>");
            htmlN.Append ("Strategic and Support Services Indicators");
            htmlN.Append ("</td>");
            
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            htmlN.Append ("<tr " + GetTableHeadRowStyle () + ">");
            htmlN.Append ("<td colspan=1 width='34%'>");
            htmlN.Append ("Strategic and Support Services");
            htmlN.Append ("</td>");
            htmlN.Append ("<td colspan=1 width='33%'>");
            htmlN.Append ("Indicators");
            htmlN.Append ("</td>");
            htmlN.Append ("<td colspan=1 width='33%'>");
            htmlN.Append ("Technique");
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            int ssRows = projectSSS.Count;
            if(ssRows>0)
            {
 for (int z = 0; z < ssRows; z++) {

                PmsSSS projectsss = projectSSS[z];
                List<PmsIndicator> sssIndicators = GetPmsSSSIndicator (int.Parse(projectsss.id), version_id);
                int ssIndRows=sssIndicators.Count;
                if(ssIndRows>0)
                {
                    for(int h=0;h<ssIndRows;h++)
                    {
                    htmlN.Append ("<tr >");
                    if(h==0)
                    {
                    htmlN.Append ("<td colspan=1 rowspan="+ssIndRows+" width='34%'>");
                    htmlN.Append (projectsss.description);
                    htmlN.Append ("</td>");
                    }
                    htmlN.Append ("<td colspan=1 width='33%'>");
                    htmlN.Append(sssIndicators[h].indicator);
                    htmlN.Append ("</td>");
                    htmlN.Append ("<td colspan=1 width='33%'>");
                    htmlN.Append(sssIndicators[h].technique);
                    htmlN.Append ("</td>");
                    htmlN.Append ("</tr>");
                    }
                }
                else
                {
                    htmlN.Append ("<tr >");
                    htmlN.Append ("<td colspan=1 width='34%'>");
                    htmlN.Append (projectsss.description);
                    htmlN.Append ("</td>");
                    htmlN.Append ("<td colspan=1 width='33%'>");
                    htmlN.Append ("</td>");
                    htmlN.Append ("<td colspan=1 width='33%'>");

                    htmlN.Append ("</td>");
                    htmlN.Append ("</tr>");
                }


            }
            }
            else
            {
                AddBlanchRow(htmlN);
            }	

            htmlN.Append ("</table>");

        }
        private void AddBlanchRow(StringBuilder htmlN)
        {
            {
                    htmlN.Append ("<tr >");
                    htmlN.Append ("<td colspan=1 width='34%'>");
                    htmlN.Append ("</td>");
                    htmlN.Append ("<td colspan=1 width='33%'>");
                    htmlN.Append ("</td>");
                    htmlN.Append ("<td colspan=1 width='33%'>");

                    htmlN.Append ("</td>");
                    htmlN.Append ("</tr>");
                }
        }
        public void AddRiskMitigationTable (StringBuilder htmlN) {
            htmlN.Append ("<table width='100%' style='border:1px;border-color:'#FAFAFA>");

            htmlN.Append ("<tr " + GetTableHeadRowStyle () + ">");
            htmlN.Append ("<td>");
            htmlN.Append ("RISK");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("ACTION (RISK MITIGATION)");
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            for (int i = 0; i < 4; i++) {

                htmlN.Append ("<tr><td></td><td></td></tr>");
            }
            htmlN.Append ("</table>");

        }
        private void AddObjective (int id, StringBuilder documentText, int version_id) {
            projectObjectives = GetPmsAnnualPlanObjective (id, version_id);
            //PrintOut AnnualPlan Budget Front End

            StringBuilder htmlN = new StringBuilder ();
            htmlN.Append ("<table width='100%' style='border:1px;border-color:'#FAFAFA;border-spacing:0px;>");
            htmlN.Append ("<tr" + GetTableHeadRowStyle () + ">");
            htmlN.Append ("<td>");
            htmlN.Append ("OUTCOME-BASED OBJECTIVES");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("PROGRAMME CATEGORY OF INTERVENTION (CoI)");
            htmlN.Append ("</td>");
            htmlN.Append("<td>");
            htmlN.Append("N° People Served F");
            htmlN.Append("</td>");
            htmlN.Append("<td>");
            htmlN.Append("N° People Served M");
            htmlN.Append("</td>");
            htmlN.Append("<td>");
            htmlN.Append("Total People Served");
            htmlN.Append("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("TOTAL X year BUDGET "+ projectObjectives[0].currency);
            htmlN.Append ("</td>");
            // htmlN.Append("<td>");
            // htmlN.Append("BUDGET");
            // htmlN.Append("</td>");
            // htmlN.Append("<td>");
            // htmlN.Append("ESTIMATED BUDGET");
            // htmlN.Append("</td>");
            htmlN.Append ("</tr>");
            string outcome = "";
            
            for (int i = 0; i < projectObjectives.Count; i++) {
                htmlN.Append ("<tr>");
                PmsObjective projectObjective = projectObjectives[i];
                htmlN.Append ("<td>");
                htmlN.Append (projectObjective.outcome);
               htmlN.Append ("</td>");
            
                htmlN.Append ("<td>");
                htmlN.Append (projectObjective.category);
                htmlN.Append ("</td>");

                //Gender Indicator

                htmlN.Append("<td>");
                htmlN.Append(projectObjective.female_indicator);
                htmlN.Append("</td>");
            
                htmlN.Append("<td>");
                htmlN.Append(projectObjective.male_indicator);
                htmlN.Append("</td>");
            
                htmlN.Append("<td>");
                htmlN.Append(projectObjective.total_gender_indicator);
                htmlN.Append("</td>");

                if (projectObjective.outcome != outcome)
                {
                    htmlN.Append("<td GridSpan Val='" + projectObjective.total_cat+"'> ");
                    htmlN.Append(projectObjective.real_budget + " " + projectObjective.currency);
                    htmlN.Append("</td>");
                }
 


                htmlN.Append("</tr>");
                outcome = projectObjective.outcome;

            }
            htmlN.Append ("</table>");
            documentText.Append (htmlN.ToString ());
        }

        private void AddObjectiveDetails (int idx, StringBuilder documentText) {
            PmsObjective projectObjective = projectObjectives[idx];
            //var newPara = documentText.AddParagraph() as WParagraph;
            StringBuilder htmlN = new StringBuilder ();
            htmlN.Append ("<table width='100%' style='border:1px'>");
            htmlN.Append ("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
            htmlN.Append ("<td>");
            htmlN.Append ("OUTCOME BASED OBJECTIVE");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("TARGET");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("OUTCOME");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("NUMBER OF SERVICE EROGATED");
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            htmlN.Append ("<tr>");
            htmlN.Append ("<td>");
            htmlN.Append (projectObjective.description);
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append (projectObjective.target);
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append (projectObjective.outcome);
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append (projectObjective.served_people);
            htmlN.Append ("</td>");
            // htmlN.Append("<td>");
            // htmlN.Append(projectObjective.real_budget + " " + projectObjective.currency);
            // htmlN.Append("</td>");
            // htmlN.Append("<td>");
            // htmlN.Append(projectObjective.estimated_budget + " " + projectObjective.currency);
            // htmlN.Append("</td>");
            htmlN.Append ("</tr>");
            htmlN.Append ("</table>");
            documentText.Append (htmlN.ToString ());
        }

        private void AddServiceDetails (PmsService service, StringBuilder documentText) {

            //var newPara = documentText.AddParagraph() as WParagraph;
            StringBuilder htmlN = new StringBuilder ();
            htmlN.Append ("<table width='100%' style='border:1px'>");
            DateTime start_date = DateTime.MinValue;
            DateTime.TryParse (service.start_date, out start_date);
            DateTime end_date = DateTime.MaxValue;
            DateTime.TryParse (service.end_date, out end_date);
            htmlN.Append ("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
            htmlN.Append ("<td>");
            htmlN.Append ("SERVICE");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("CATEGORY");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("TYPE OF SERVICE");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("START DATE");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("END DATE");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("PERIODICITY");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("NUMBER OF SERVICES PROVIDED");
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");

            htmlN.Append ("<tr>");
            htmlN.Append ("<td>");
            htmlN.Append (service.title);
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append (service.category_code + " - " + service.category);
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append (service.type_of_service);
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append (start_date.ToShortDateString ());
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append (end_date.Date.ToShortDateString ());
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append (service.periodicity);
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append (service.served_people);
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");

            htmlN.Append ("</table>");
            documentText.Append (htmlN.ToString ());
        }

        private void AddOverallGoal (int id, StringBuilder documentText, int version_id) {
            PmsOverallGoal projectGoal = GetPmsAnnualOverallGoal (id, version_id);
            // //var newPara = documentText.AddParagraph() as WParagraph;
            StringBuilder htmlN = new StringBuilder ();
            htmlN.Append ("<table width='100%' style='border:none; font-family:Calibri;font-size:10px;' border=0>");

            // htmlN.Append ("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
            // htmlN.Append ("<td>");
            // htmlN.Append ("DESCRIPTION");
            // htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            htmlN.Append ("<tr>");
            htmlN.Append ("<td>");
            htmlN.Append (projectGoal.goal);
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");

            htmlN.Append ("</table>");
            documentText.Append (htmlN.ToString ());
        }

        private void AddCC (int id, StringBuilder documentText, int version_id) {
            List<PmsCC> projectCCs = GetPmsAnnualCC (id, version_id);
            AddHeading (documentText, "h2", "Principles, Values, Criteria Cross Cutting");

            for (int i = 0; i < projectCCs.Count; i++) {

                PmsCC projectCC = projectCCs[i];

                AddHeading (documentText, "h3", "Principles, Values, Criteria Cross Cutting: " + projectCC.description);
                int idobj = int.Parse (projectCC.id);

                AddHeading (documentText, "h3", "Principles, Values, Criteria Cross Cutting Indicators");
                AddCCIndicator (idobj, documentText, version_id);

            }
        }
        private void AddCCP (int id, StringBuilder documentText, int version_id) {
            List<PmsCCP> projectCCPs = GetPmsAnnualCCP (id, version_id);
            AddHeading (documentText, "h2", "Cross Cutting Processs");
            for (int i = 0; i < projectCCPs.Count; i++) {

                PmsCCP projectCCP = projectCCPs[i];

                AddHeading (documentText, "h3", "Cross Cutting Process: " + projectCCP.description);
                int idobj = int.Parse (projectCCP.id);

                AddHeading (documentText, "h3", "Cross Cutting Process Indicators");
                AddCCPIndicator (idobj, documentText, version_id);

            }
        }

        private void AddStrategicAndSupportServices (int id, StringBuilder documentText, int version_id) {
            List<PmsSSS> projectSSSs = GetPmsAnnualSSS (id, version_id);

            for (int i = 0; i < projectSSSs.Count; i++) {

                PmsSSS projectSSS = projectSSSs[i];
                AddHeading (documentText, "h2", "Strategic And Support Services");
                AddHeading (documentText, "h3", "Strategic And Support Service: " + projectSSS.description);
                int idobj = int.Parse (projectSSS.id);

                AddHeading (documentText, "h3", "Strategic And Support Service Indicators");
                AddSSSIndicator (idobj, documentText, version_id);

            }
        }

        private void AddOverallGoalIndicator (int id, StringBuilder documentText, int version_id) {
            List<PmsIndicator> goalIndicators = GetPmsAnnualOverallGoalIndicator (id, version_id);
            //var newPara = documentText.AddParagraph() as WParagraph;
            StringBuilder htmlN = new StringBuilder ();
            htmlN.Append ("<table width='100%' style='border:1px'>");

            htmlN.Append ("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
            htmlN.Append ("<td>");
            htmlN.Append ("QUESTION");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("INDICATOR");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("STANDARD");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("TECHNIQUE");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("PERIODICITY");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("DISAGGREGATED");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("VALUE");
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            for (int i = 0; i < goalIndicators.Count; i++) {
                htmlN.Append ("<tr>");
                PmsIndicator goalIndic = goalIndicators[i];
                htmlN.Append ("<td>");
                htmlN.Append (goalIndic.question);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (goalIndic.indicator);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (goalIndic.standard);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (goalIndic.technique);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (goalIndic.periodicity);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (goalIndic.disaggregated);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (goalIndic.value);
                htmlN.Append ("</td>");
                htmlN.Append ("</tr>");

            }
            htmlN.Append ("</table>");
            documentText.Append (htmlN.ToString ());
        }
        private void AddOutputIndicator (int id, StringBuilder documentText, int version_id) {
            List<PmsIndicator> outputIndicators = GetPmsOutputIndicator (id, version_id);
            //var newPara = documentText.AddParagraph() as WParagraph;
            StringBuilder htmlN = new StringBuilder ();
            htmlN.Append ("<table width='100%' style='border:1px'>");

            htmlN.Append ("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
            htmlN.Append ("<td>");
            htmlN.Append ("QUESTION");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("INDICATOR");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("STANDARD");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("TECHNIQUE");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("PERIODICITY");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("DISAGGREGATED");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("VALUE");
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            for (int i = 0; i < outputIndicators.Count; i++) {
                htmlN.Append ("<tr>");
                PmsIndicator objIndic = outputIndicators[i];
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.question);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.indicator);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.standard);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.technique);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.periodicity);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.disaggregated);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.value);
                htmlN.Append ("</td>");
                htmlN.Append ("</tr>");

            }
            htmlN.Append ("</table>");
            documentText.Append (htmlN.ToString ());
        }

        private void AddObjectiveIndicator (int id, StringBuilder documentText, int version_id) {
            List<PmsIndicator> objectiveIndicators = GetPmsAnnualObjectiveIndicator (id, version_id);
            //var newPara = documentText.AddParagraph() as WParagraph;
            StringBuilder htmlN = new StringBuilder ();
            htmlN.Append ("<table width='100%' style='border:1px'>");

            htmlN.Append ("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
            htmlN.Append ("<td>");
            htmlN.Append ("QUESTION");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("INDICATOR");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("STANDARD");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("TECHNIQUE");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("PERIODICITY");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("DISAGGREGATED");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("VALUE");
            htmlN.Append ("</td>");
            htmlN.Append ("</tr>");
            for (int i = 0; i < objectiveIndicators.Count; i++) {
                htmlN.Append ("<tr>");
                PmsIndicator objIndic = objectiveIndicators[i];
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.question);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.indicator);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.standard);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.technique);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.periodicity);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.disaggregated);
                htmlN.Append ("</td>");
                htmlN.Append ("<td>");
                htmlN.Append (objIndic.value);
                htmlN.Append ("</td>");
                htmlN.Append ("</tr>");

            }
            htmlN.Append ("</table>");
            documentText.Append (htmlN.ToString ());
        }
        private void AddOutputDetails (PmsOutput output, StringBuilder documentText) {

            //var newPara = documentText.AddParagraph() as WParagraph;
            StringBuilder htmlN = new StringBuilder ();
            htmlN.Append ("<table width='100%' style='border:1px'>");
            htmlN.Append ("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
            htmlN.Append ("<td>");
            htmlN.Append ("OUTPUT DESCRIPTION");
            htmlN.Append ("</td>");

            htmlN.Append ("</tr>");

            htmlN.Append ("<tr>");
            htmlN.Append ("<td>");
            htmlN.Append (output.description);
            htmlN.Append ("</td>");

            htmlN.Append ("</tr>");

            htmlN.Append ("</table>");
            documentText.Append (htmlN.ToString ());
        }

        private void AddProcessDetails (PmsProcess process, StringBuilder documentText) {

            //var newPara = documentText.AddParagraph() as WParagraph;
            StringBuilder htmlN = new StringBuilder ();
            htmlN.Append ("<table width='100%' style='border:1px'>");
            htmlN.Append ("<tr style='font-size:10px;font-weight:bold;border:1px;border-style:solid;background-color:cyan;'>");
            htmlN.Append ("<td>");
            htmlN.Append ("PROCESS DESCRIPTION");
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append ("PROCESS TYPE");
            htmlN.Append ("</td>");

            htmlN.Append ("</tr>");

            htmlN.Append ("<tr>");
            htmlN.Append ("<td>");
            htmlN.Append (process.description);
            htmlN.Append ("</td>");
            htmlN.Append ("<td>");
            htmlN.Append (process.process_type);
            htmlN.Append ("</td>");

            htmlN.Append ("</tr>");

            htmlN.Append ("</table>");
            documentText.Append (htmlN.ToString ());
        }

        public string GetTableHeadRowStyle () {
            string style = @" style='background-color: #134388;
                border-color: #004388;
                font-color:#FFFFFF;
                font-size:12px;
                font-family:Calibri;
                font-weight:bold;'
                color='#FFFFFF'
                font='Calibri' ";
            return style;
        }

        public string GetCaptiontyle () {
            string style = @" style='
                font-style=italic;
                font-color: blue;
                font-size:10px;
                font-family:Calibri;
                color=blue
                font='Calibri' ";
            return style;
        }

        public void AddNarrative (int id, string tableName, StringBuilder documentText, int startingLevel, int version_id) {

            List<PmsNarrative> projetcNarratives = GetPmsAnnualPlanNarrative (id, tableName, version_id);

            for (int i = 0; i < projetcNarratives.Count; i++) {

                PmsNarrative projetcNarrative = projetcNarratives[i];

                switch (startingLevel) {
                    case 1:
                        if (projetcNarrative.section_title == "") {
                            AddHeading (documentText, "h3", projetcNarrative.descr);

                        } else {
                            AddHeading (documentText, "h3", projetcNarrative.section_title);

                        }
                        break;
                    case 2:
                        if (projetcNarrative.section_title == "") {
                            AddHeading (documentText, "h3", projetcNarrative.descr);

                        } else {
                            AddHeading (documentText, "h5", projetcNarrative.section_title);

                        }
                        break;
                    case 3:
                        if (projetcNarrative.section_title == "") {
                            AddHeading (documentText, "h5", projetcNarrative.descr);

                        } else {
                            AddHeading (documentText, "h6", projetcNarrative.section_title);

                        }
                        break;
                    case 4:
                        if (projetcNarrative.section_title == "") {
                            AddHeading (documentText, "h6", projetcNarrative.descr);

                        } else {
                            AddHeading (documentText, "h7", projetcNarrative.section_title);

                        }
                        break;
                    case 5:
                        if (projetcNarrative.section_title == "") {
                            AddHeading (documentText, "h7", projetcNarrative.descr);

                        } else {
                            AddHeading (documentText, "h8", projetcNarrative.section_title);

                        }
                        break;
                    case 6:
                        if (projetcNarrative.section_title == "") {
                            AddHeading (documentText, "h8", projetcNarrative.descr);

                        } else {
                            AddHeading (documentText, "h9", projetcNarrative.section_title);

                        }
                        break;
                }
                //var newPara = documentText.AddParagraph() as WParagraph;
                StringBuilder htmlN = new StringBuilder ();
                htmlN.Append ("<table width='100%' border=0 cellspacing=0 cellpadding=0 style='font-family:Calibri;font-size:12px'>");

                htmlN.Append ("<tr><td>");
                htmlN.Append (projetcNarrative.section_text);
                htmlN.Append ("</td></tr>");
                htmlN.Append ("</table>");
                documentText.Append (htmlN.ToString ());
            }
        }
        // private void AddHeaderFooter(WordDocument document)
        // {
        //     foreach (StringBuilder documentText in document.documentTexts)
        //     {
        //         IWParagraph paragraph = documentText.HeadersFooters.Header.AddParagraph();
        //         IWTextRange textRange = paragraph.AppendText("This is Header");
        //         paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
        //         textRange.CharacterFormat.Bold = true;
        //         textRange.CharacterFormat.FontSize = 20;
        //         paragraph = documentText.HeadersFooters.Footer.AddParagraph();
        //         textRange = paragraph.AppendText("This is Footer");
        //         paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
        //         textRange.CharacterFormat.Bold = true;
        //         textRange.CharacterFormat.FontSize = 20;
        //     }
        // }
    }
    public class PmsNarrative {
        public string descr { get; set; }
        public string section_text { get; set; }
        public string section_title { get; set; }
    }

    public class PmsObjective {
        public string id { get; set; }
        public string description { get; set; }
        public string served_people { get; set; }
        public string target { get; set; }
        public string outcome_id { get; set; }
        public string male_indicator { get; set; }
        public string female_indicator { get; set; }
        public string total_gender_indicator { get; set; }
        public string outcome { get; set; }
        public string category { get; set; }
        public string real_budget { get; set; }
        public string estimated_budget { get; set; }
        public string currency { get; set; }
        public string total_cat { get; set; }
    }

    public class PmsOverviewObjective {
        public string id { get; set; }
        public string description { get; set; }
        public string outcome { get; set; }
        public string category { get; set; }
        public string category_code { get; set; }
        public string output { get; set; }
        public string output_id { get; set; }
    }

    public class PmsOverallGoal {
        public string target { get; set; }
        public string goal { get; set; }

    }

    public class PmsSSS {
        public string description { get; set; }
        public string id { get; set; }

    }

    public class PmsCC {
        public string description { get; set; }
        public string id { get; set; }

    }

    public class PmsCCP {
        public string description { get; set; }
        public string id { get; set; }

    }

    public class PmsIndicator {
        public string question { get; set; }
        public string indicator { get; set; }
        public string standard { get; set; }
        public string periodicity { get; set; }
        public string disaggregated { get; set; }
        public string technique { get; set; }
        public string endDate { get; set; }
        public string value { get; set; }

    }
    public class PmsService {
        public string id { get; set; }
        public string title { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string category_code { get; set; }
        public string served_people { get; set; }
        public string category { get; set; }
        public string type_of_service { get; set; }
        public string periodicity { get; set; }
        public string location { get; set; }
    }
    public class PmsProcess {
        public string id { get; set; }
        public string description { get; set; }
        public string process_type { get; set; }
    }

    public class PmsOutput {
        public string id { get; set; }
        public string description { get; set; }

    }
}