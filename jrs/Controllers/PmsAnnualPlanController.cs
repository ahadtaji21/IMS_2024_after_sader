using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jrs.DBContexts;
using jrs.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authorization;
using jrs.Classes;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using System.Data;
using jrs.DA;


namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class PmsAnnualPlanController : ControllerBase
    {
        private readonly GeneralContext _context;
        private readonly IMSLogContext _logContext;

        private string UserId = "";
        private string IP = "";


        public PmsAnnualPlanController(GeneralContext context, IMSLogContext logContext)
        {
            _context = context;
            _logContext = logContext;
        }

        // GET: api/PmsAnnualPlan
        [HttpGet]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<PmsAnnualPlan>>> GetPmsAnnualPlan()
        {
            ImsUtility utility = new ImsUtility();


            List<PmsAnnualPlan> pmsAnnualPlans = new List<PmsAnnualPlan>();
            string spName = DatabaseConstants.spPMSSelGetAnnualPlanList;
            SqlDataReader dr = null;

            try
            {
                UserId = User.Identity.Name;
                IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.StoredProcedure;
          
                    sqlcmd.CommandText = spName;
                    
                    dr = sqlcmd.ExecuteReader();
                    while (dr.Read())
                    {
                        PmsAnnualPlan pmsAnnualPlan = new PmsAnnualPlan();

                        pmsAnnualPlan.Id = int.Parse(dr["Id"] + "");
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
                        pmsAnnualPlan.status_name = (dr["IMS_STATUS_NAME"] + "");
                        pmsAnnualPlan.currency_code = (dr["CURRENCY_CODE"] + "");
                        pmsAnnualPlan.location_id = (dr["LOCATION_ID"] + "");
                        pmsAnnualPlan.location_description = (dr["LOCATION_DESCRIPTION"] + "");
                        pmsAnnualPlan.activation_state = (dr["ACTIVATION_STATE"] + "");
                        pmsAnnualPlan.guid = (dr["GUID"] + "");
                        pmsAnnualPlans.Add(pmsAnnualPlan);

                    }
                }
            }
            catch (Exception ex)
            {

                ImsLogerrors evt = utility.GetLogError("PmsAnnualPlanController", "GetPmsAnnualPlan", ex, UserId, "", IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();


            }

            return pmsAnnualPlans;

        }


        [HttpPost("{id}")]
        [Consumes("application/json")]
        public async Task<ActionResult<string>> AddOrUpdatePmsAnnualPlan(int id, PmsAnnualPlan pmsAnnualPlan)
        {

            ImsUtility utility = new ImsUtility();
            //ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            //var Name = ClaimsPrincipal.Current.Identity.Name;
            string UserId = "";
            string IP = "";
            int _id=0;
            SqlCommand cmdSqldb = null;
            SqlDataReader dr = null;
            if (id != pmsAnnualPlan.Id)
            {
                return BadRequest();
            }
            try
            {
                UserId = User.Identity.Name;
                IP = utility.GetIP(Response);
                using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
                {
                       conn.Open();
                    cmdSqldb = new SqlCommand();
                    cmdSqldb.CommandType = System.Data.CommandType.StoredProcedure;
                    cmdSqldb.Connection = conn;
                 
                    cmdSqldb.CommandText = DatabaseConstants.spPMSUpdAnnualPlanDetails;

                  

                    #region Parameters
                    cmdSqldb.Parameters.AddWithValue("@ID", pmsAnnualPlan.Id);
                    cmdSqldb.Parameters.AddWithValue("@OFFICE_ID", pmsAnnualPlan.office_id);
                    cmdSqldb.Parameters.AddWithValue("@CODE", pmsAnnualPlan.code);
                    cmdSqldb.Parameters.AddWithValue("@CODE_ID", pmsAnnualPlan.code_id);
                    cmdSqldb.Parameters.AddWithValue("@DESCR", pmsAnnualPlan.descr);
                    cmdSqldb.Parameters.AddWithValue("@LOCATION_DESCR", " ");
                    cmdSqldb.Parameters.AddWithValue("@STATUS_ID", pmsAnnualPlan.status_id);
                    cmdSqldb.Parameters.AddWithValue("@START_YEAR", pmsAnnualPlan.start_year);
                    cmdSqldb.Parameters.AddWithValue("@END_YEAR", pmsAnnualPlan.end_year);
                    cmdSqldb.Parameters.AddWithValue("@CURRENCY_CODE", pmsAnnualPlan.currency_code);
                    cmdSqldb.Parameters.AddWithValue("@LOCATION_ID", pmsAnnualPlan.location_id);
                    cmdSqldb.Parameters.AddWithValue("@GUID", pmsAnnualPlan.guid);
                    cmdSqldb.Parameters.AddWithValue("@VERSION", 1);
                    #endregion Parameters
                    dr = cmdSqldb.ExecuteReader();
                      while (dr.Read())
                    {
                        _id = int.Parse("0"+dr["ID"]);
                    }
                    string msg="AP Added";
                    if(id>0) msg="AP Updated";
                    ImsLogevents evt = utility.GetLogEvent("PmsAnnualPlanController", "AddOrUpdatePmsAnnualPlan", msg, UserId, pmsAnnualPlan, IP);
                    _logContext.ImsLogevents.Add(evt);
                    _logContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                ImsLogerrors evt = utility.GetLogError("PmsAnnualPlanController", "AddOrUpdatePmsAnnualPlan_1", ex, UserId, pmsAnnualPlan, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
                return StatusCode(500,ex.Message);
            }
             return StatusCode(200,_id);
        }


    //     // GET: api/PmsAnnualPlan/5
    //     [HttpGet("{id}")]
    //     [Consumes("application/json")]
    //     public async Task<ActionResult<PmsAnnualPlan>> GetPmsAnnualPlan(int id)
    //     {
    //         //var pmsAnnualPlan = await _context.PmsAnnualPlan.FindAsync(id);
    //         var pmsAnnualPlan = _context.Set<PmsAnnualPlan>()
    //             .Include(x => x.PmsCoiTosRel).FirstOrDefault();
    //         if (pmsAnnualPlan == null)
    //         {
    //             return NotFound();
    //         }

    //         return pmsAnnualPlan;
    //     }

    //     // PUT: api/PmsAnnualPlan/5
    //     // To protect from overposting attacks, please enable the specific properties you want to bind to, for
    //     // more details see https://aka.ms/RazorPagesCRUD.
    //     [HttpPut("{id}")]
    //     [Consumes("application/json")]
    //     //[Authorize]
    //     public async Task<IActionResult> PutPmsAnnualPlan(int id, PmsAnnualPlan pmsAnnualPlan)
    //     {

    //         ImsUtility utility = new ImsUtility();
    //         //ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

    //         //var Name = ClaimsPrincipal.Current.Identity.Name;
    //         string UserId = "";
    //         string IP = "";
    //         if (id != pmsAnnualPlan.PmsCoiId)
    //         {
    //             return BadRequest();
    //         }
    //         using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
    //         {
    //             try
    //             {
    //                  UserId = User.Identity.Name;
    //                  IP = utility.GetIP(Response);
    //                 var oldPmsAnnualPlan = await _context.PmsAnnualPlan.FindAsync(id);

    //                 if (oldPmsAnnualPlan == null)
    //                 {
    //                     return NotFound();
    //                 }
    //                 else
    //                 {
    //                     oldPmsAnnualPlan.PmsCoiDeleted = true;
    //                     _context.Entry(oldPmsAnnualPlan).State = EntityState.Modified;
    //                     _context.SaveChanges();
    //                     var pmsCoiTosRels = pmsAnnualPlan.PmsCoiTosRel;
    //                     pmsAnnualPlan.PmsCoiTosRel=null;
    //                     pmsAnnualPlan.PmsCoiId = 0;

    //                     var pmscoi = _context.PmsAnnualPlan.Add(pmsAnnualPlan);
    //                      _context.SaveChanges();
    //                      int insertedId = pmsAnnualPlan.PmsCoiId;
    //                      foreach(PmsCoiTosRel pmsCoiTosRel in pmsCoiTosRels)
    //                      {
    //                          pmsCoiTosRel.PmsCoiTosId =0;
    //                          pmsCoiTosRel.PmsCoiId = insertedId;
    //                         _context.PmsCoiTosRel.Add(pmsCoiTosRel);
    //                      }
    //                      _context.SaveChanges();

    //                 }

    //                 transaction.Commit();
    //                 ImsLogevents evt = utility.GetLogEvent("PmsCategoryOfInterventioControl", "PutPmsAnnualPlan", "COI updated", UserId, pmsAnnualPlan,IP);
    //                 _logContext.ImsLogevents.Add(evt);
    //                 _logContext.SaveChanges();
    //             }
    //             catch (Exception ex)
    //             {

    //                 transaction.Rollback();
    //                 ImsLogerrors evt = utility.GetLogError("PmsCategoryOfInterventioControl", "PutPmsAnnualPlan",ex, UserId, pmsAnnualPlan,IP);
    //                 #region modo alternativo
    //                 /*
    //                 StringBuilder sb = new StringBuilder();
    //                 sb.Append("CC:");
    //                 sb.Append(pmsAnnualPlan.PmsCoiCode);
    //                 sb.Append(";");
    //                 sb.Append("CD:");
    //                 sb.Append(pmsAnnualPlan.PmsCoiDeleted+"");
    //                 sb.Append(";");
    //                 sb.Append("CDS:");
    //                 sb.Append(pmsAnnualPlan.PmsCoiDescription + "");
    //                 sb.Append(";");
    //                 sb.Append("CE:");
    //                 sb.Append(pmsAnnualPlan.PmsCoiEnabled + "");
    //                 sb.Append(";");
    //                 sb.Append("CI:");
    //                 sb.Append(pmsAnnualPlan.PmsCoiId + "");
    //                 sb.Append(";");
    //                 utility.GetLogError("PmsCategoryOfInterventioControl", "PutPmsAnnualPlan", ex, UserId, sb.ToString());
    //                 */
    //                 #endregion modo alternativo
    //                 _logContext.ImsLogerrors.Add(evt);
    //                 _logContext.SaveChanges();
    //                 throw ex;

    //             }
    //         }

    //         return NoContent();
    //     }

    //     // POST: api/PmsAnnualPlan
    //     // To protect from overposting attacks, please enable the specific properties you want to bind to, for
    //     // more details see https://aka.ms/RazorPagesCRUD.
    //     [HttpPost]
    //     [Consumes("application/json")]
    //     public async Task<ActionResult<PmsAnnualPlan>> PostPmsAnnualPlan(PmsAnnualPlan pmsAnnualPlan)
    //     {
    //         _context.PmsAnnualPlan.Add(pmsAnnualPlan);
    //         await _context.SaveChangesAsync();

    //         return CreatedAtAction("GetPmsAnnualPlan", new { id = pmsAnnualPlan.PmsCoiId }, pmsAnnualPlan);
    //     }

    //     // DELETE: api/PmsAnnualPlan/5
    //     [HttpDelete("{id}")]
    //     public async Task<ActionResult<PmsAnnualPlan>> DeletePmsAnnualPlan(int id)
    //     {
    //         var pmsAnnualPlan = await _context.PmsAnnualPlan.FindAsync(id);
    //         if (pmsAnnualPlan == null)
    //         {
    //             return NotFound();
    //         }

    //         _context.PmsAnnualPlan.Remove(pmsAnnualPlan);
    //         await _context.SaveChangesAsync();

    //         return pmsAnnualPlan;
    //     }

    //     private bool PmsAnnualPlanExists(int id)
    //     {
    //         return _context.PmsAnnualPlan.Any(e => e.PmsCoiId == id);
    //     }

}
}
