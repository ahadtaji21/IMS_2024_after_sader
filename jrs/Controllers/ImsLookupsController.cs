using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using jrs.DBContexts;
using jrs.Models;
using jrs.DA;
using jrs.Classes;

namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class ImsLookupsController : ControllerBase
    {
        private readonly GeneralContext _context;
        private readonly IMSLogContext _logContext;
        string UserId = "";
        string IP = "";
        public ImsLookupsController(GeneralContext context, IMSLogContext logContext)
        {
            _context = context;
            _logContext = logContext;
        }


        // GET: api/ImsLookup
        [HttpGet]
        // [Authorize]
        public async Task<ActionResult<IEnumerable<ImsLookup>>> GetImsLookup()
        {
            List<ImsLookup> lookups = await _context.ImsLookup
                .Include(l => l.ImsLookupLookupType)
                .ToListAsync();
            return lookups;
        }


        [HttpGet("{ListName}")]
        public async Task<ActionResult<IEnumerable<ImsList>>> GetGeneriList(string ListName)
        {

            List<ImsList> retList = new List<ImsList>();
            ImsUtility utility = new ImsUtility();

            try
            {
                UserId = User.Identity.Name;
                IP = utility.GetIP(Response);
                DalCommon dalCommon = new DalCommon(_context, _logContext, UserId, IP);
                retList = dalCommon.GetGenericList(ListName);
            }
            catch (Exception ex)
            {
                ImsLogerrors evt = utility.GetLogError("ImsLookupsController", "GetGeneriList", ex, UserId,ListName, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();

            }
            return retList;
        }

        [HttpGet("GetGeneriListByCompany")]
        public async Task<ActionResult<IEnumerable<ImsList>>> GetGeneriListByCompany(string ListName,string Company)
        {

            List<ImsList> retList = new List<ImsList>();
            ImsUtility utility = new ImsUtility();

            try
            {
                UserId = User.Identity.Name;
                IP = utility.GetIP(Response);
                DalCommon dalCommon = new DalCommon(_context, _logContext, UserId, IP);
                retList = dalCommon.GetGenericListByCompany(ListName,Company);
            }
            catch (Exception ex)
            {
                ImsLogerrors evt = utility.GetLogError("ImsLookupsController", "GetGeneriListByCompany", ex, UserId,ListName, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();

            }
            return retList;
        }
    }


}


