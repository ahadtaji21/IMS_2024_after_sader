
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

    public class ConfigGenericController : ControllerBase
    {
        private readonly GeneralContext _context;
        private readonly IMSLogContext _logContext;
        private readonly IConfiguration Configuration;
        string UserId = "";
        string IP = "";
        ImsUtility utility = null;

        public ConfigGenericController(GeneralContext context, IMSLogContext logContext, IConfiguration configuration)
        {
            _context = context;
            _logContext = logContext;
            utility = new ImsUtility();
            Configuration = configuration;
        }
      

        /// <summary>
        /// Copy Data from one Office to other Office (specific for JRS configuration stuff)
        /// </summary>
        [HttpPost("/copy-config-from-to-offices", Name = "Copy configuration from to offices")]
        [Authorize]
        public async Task<IActionResult> CopyConfigFromToOffices(string TableName,string ColumnNameOffice,int IdOfficeFrom,int IdOfficeTo,string ColumnNamePK)
        {
          
            try
            {
                IP = utility.GetIP(Response);
                UserId = User.Identity.Name;
                await _context.Database.ExecuteSqlInterpolatedAsync($"dbo.SP_COPY_CONF_FROM_TO_OFFICE @TABLE_TO_COPY={TableName}, @COLUMN_NAME_OFFICE_ID={ColumnNameOffice}, @OFFICE_ID_FROM={IdOfficeFrom}, @OFFICE_ID_TO={IdOfficeTo},@COLUMN_NAME_ID={ColumnNamePK}");

            }
            catch (Exception ex)
            {
                //@TODO: Implementare Log
                ImsLogerrors evt = utility.GetLogError("GenericSqlController", "GenericSelectStructure", ex, UserId, "", IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
                return NotFound();
            }

            return Ok("Copy was successful.");
            //return null;
        }
    }
}