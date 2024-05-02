using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jrs.DBContexts;
using jrs.Models;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using jrs.Classes;
using System.Text;
namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]


    public class PmsProjectController : ControllerBase
    {
        private readonly GeneralContext _context;
        private readonly IMSLogContext _logContext;

        private string UserId = "";
        private string IP = "";

        public PmsProjectController(GeneralContext context, IMSLogContext logContext)
        {
            _context = context;
            _logContext = logContext;

        }

        // GET: api/PmsTypeOfService
        [HttpGet]
        public async Task<ActionResult<PmsProjectWorkFlow>> GetPmsProjectWorkFlow(int actualStatus)
        {
            ImsUtility utility = new ImsUtility();

            PmsProjectWorkFlow pmsProjectWorkFlow = new PmsProjectWorkFlow();
            string spName =DatabaseConstants.spPMSSelGetProjectWorkflowStatus;
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
                    sqlcmd.Parameters.AddWithValue("@ACTUAL_STATUS", actualStatus);

                    dr = sqlcmd.ExecuteReader();
                    while (dr.Read())
                    {
                        pmsProjectWorkFlow.nextStatus = int.Parse(dr["NEXT_STATUS"] + "");
                        pmsProjectWorkFlow.previousStatus = int.Parse(dr["PREVIOUS_STATUS"] + "");
                    }
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("AS:");
                sb.Append(actualStatus);
                sb.Append(";");
                ImsLogerrors evt = utility.GetLogError("PmsProjectController", "GetPmsProjectWorkFlow", ex, UserId, sb.ToString(), IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
                pmsProjectWorkFlow.nextStatus = -1;
                pmsProjectWorkFlow.previousStatus = -1;

            }

            return pmsProjectWorkFlow;

        }
    }
}