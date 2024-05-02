using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using jrs.Classes;
using jrs.DA;
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
    public class AnnualPlanDocGeneratorController : ControllerBase {
        private readonly GeneralContext _context;
        private readonly IMSLogContext _logContext;
        private readonly IConfiguration Configuration;
        private string _connectionString;
        string UserId = "";
        string IP = "";
        List<PmsObjective> projectObjectives;
        ImsUtility utility = null;
        /// <summary>
        /// Constructor for the "GenericSqlController" class.
        /// </summary>
        public AnnualPlanDocGeneratorController (GeneralContext context, IMSLogContext logContext, IConfiguration configuration) {
            _context = context;
            _logContext = logContext;
            utility = new ImsUtility ();
            Configuration = configuration;
        }
        public static void SearchAndReplace (string document) {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open (document, true)) {
                string docText = null;
                using (StreamReader sr = new StreamReader (wordDoc.MainDocumentPart.GetStream ())) {
                    docText = sr.ReadToEnd ();
                }

                // Regex regexText = new Regex ("#%FULLNAME%#");
                // docText = regexText.Replace (docText, "Alberto");

                using (StreamWriter sw = new StreamWriter (wordDoc.MainDocumentPart.GetStream (FileMode.Create))) {
                    sw.Write (docText);
                }
            }
        }

        [HttpGet ("{id}")]
        public async Task<ActionResult<Object>> GetPMSTemplate (int id, int version) {

            //   AnnualPlanDocGeneratorController.SearchAndReplace(@"/Users/albertotricarico/Downloads/sharednew/Appendix I_Grant Agreement Letter Template with Tag.docx");
            //   return StatusCode (500);

            string template = "";
            ImsUtility utility = new ImsUtility ();
            byte[] retBytes = null;

            try {
                UserId = User.Identity.Name;
                IP = utility.GetIP (Response);
                _connectionString = _context.Database.GetDbConnection ().ConnectionString;
                DalDOC dalGMT = new DalDOC (_context, _logContext, UserId, IP);
                byte[] document = dalGMT.GetDOCTemplate ("PMS_AP");
                StringBuilder documentText = new StringBuilder();
                AnnualPlanDocController apdc = null;//new AnnualPlanDocController (_context, _logContext, Configuration, User, IP, _connectionString);
                PmsAnnualPlan pap = apdc.GetPmsAnnualPlanMainData (id, version);
                apdc.AddNarrative(id, "PMS_PROJECT", documentText, 1, version);
                using (MemoryStream stream = new MemoryStream ()) {
                    stream.Write (document, 0, (int) document.Length);
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open (stream, true)) {
                        string docText = null;
                        using (StreamReader sr = new StreamReader (wordDoc.MainDocumentPart.GetStream ())) {
                            docText = sr.ReadToEnd ();
                        }

                        Regex regexText = new Regex ("#PMS_PT#");
                        docText = regexText.Replace (docText, pap.descr);
                        regexText = new Regex ("#PMS_PC#");
                        docText = regexText.Replace (docText, pap.apcode);
                        regexText = new Regex ("#PMS_LOC#");
                        docText = regexText.Replace (docText, pap.location_descr);
                        regexText = new Regex ("#PMS_SD#");
                        docText = regexText.Replace (docText, pap.start_year + "");
                        regexText = new Regex ("#PMS_ED#");
                        docText = regexText.Replace (docText, pap.end_year + "");
                        regexText = new Regex ("#PMS_CONTEXT_NRTV#");
                        string nrtv =  Regex.Replace(documentText.ToString(), "<.*?>", "");
                        nrtv=nrtv.Replace("&nbsp;"," ");
                        //nrtv=nrtv.Replace("\n","");
                        
                        docText = regexText.Replace (docText, nrtv);
                        using (StreamWriter sw = new StreamWriter (wordDoc.MainDocumentPart.GetStream (FileMode.Create))) {
                            sw.Write (docText);
                        }
                    }
                    retBytes = stream.ToArray ();
                    //stream.Read (retBytes, 0, (int) stream.Length);
                }

                // Return Byte array
                return StatusCode (200, retBytes);
            } catch (Exception ex) {
                ImsLogerrors evt = utility.GetLogError ("AnnualPlanDocGeneratorController", "GetGMTTemplate", ex, UserId, "", IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();
                return StatusCode (500);
            }

        }
        public byte[] ReadFully (Stream input) {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream ()) {
                int read;
                while ((read = input.Read (buffer, 0, buffer.Length)) > 0) {
                    ms.Write (buffer, 0, read);
                }
                return ms.ToArray ();
            }
        }

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
                    if (conn.State != ConnectionState.Open) {
                        conn.Open ();
                    }
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

    }
}