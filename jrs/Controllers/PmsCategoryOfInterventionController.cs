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


namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class PmsCategoryOfInterventionController : ControllerBase
    {
        private readonly PMSContext _context;
        private readonly IMSLogContext _logContext;


        public PmsCategoryOfInterventionController(PMSContext context,IMSLogContext logContext)
        {
            _context = context;
            _logContext = logContext;
        }

        // GET: api/PmsCategoryOfIntervention
        [HttpGet]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<PmsCategoryOfIntervention>>> GetPmsCategoryOfIntervention()
        {
            //return await _context.PmsCategoryOfIntervention.ToListAsync();
            var pmsCategoryOfIntervention = _context.Set<PmsCategoryOfIntervention>()
     .Include(x => x.PmsCoiTosRel).ToList();
            return pmsCategoryOfIntervention;
        }

        // GET: api/PmsCategoryOfIntervention/5
        [HttpGet("{id}")]
        [Consumes("application/json")]
        public async Task<ActionResult<PmsCategoryOfIntervention>> GetPmsCategoryOfIntervention(int id)
        {
            //var pmsCategoryOfIntervention = await _context.PmsCategoryOfIntervention.FindAsync(id);
            var pmsCategoryOfIntervention = _context.Set<PmsCategoryOfIntervention>()
                .Include(x => x.PmsCoiTosRel).FirstOrDefault();
            if (pmsCategoryOfIntervention == null)
            {
                return NotFound();
            }

            return pmsCategoryOfIntervention;
        }

        // PUT: api/PmsCategoryOfIntervention/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Consumes("application/json")]
        //[Authorize]
        public async Task<IActionResult> PutPmsCategoryOfIntervention(int id, PmsCategoryOfIntervention pmsCategoryOfIntervention)
        {

            ImsUtility utility = new ImsUtility();
            //ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            //var Name = ClaimsPrincipal.Current.Identity.Name;
            string UserId = "";
            string IP = "";
            if (id != pmsCategoryOfIntervention.PmsCoiId)
            {
                return BadRequest();
            }
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                     UserId = User.Identity.Name;
                     IP = utility.GetIP(Response);
                    var oldPmsCategoryOfIntervention = await _context.PmsCategoryOfIntervention.FindAsync(id);

                    if (oldPmsCategoryOfIntervention == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        oldPmsCategoryOfIntervention.PmsCoiDeleted = true;
                        _context.Entry(oldPmsCategoryOfIntervention).State = EntityState.Modified;
                        _context.SaveChanges();
                        var pmsCoiTosRels = pmsCategoryOfIntervention.PmsCoiTosRel;
                        pmsCategoryOfIntervention.PmsCoiTosRel=null;
                        pmsCategoryOfIntervention.PmsCoiId = 0;
                      
                        var pmscoi = _context.PmsCategoryOfIntervention.Add(pmsCategoryOfIntervention);
                         _context.SaveChanges();
                         int insertedId = pmsCategoryOfIntervention.PmsCoiId;
                         foreach(PmsCoiTosRel pmsCoiTosRel in pmsCoiTosRels)
                         {
                             pmsCoiTosRel.PmsCoiTosId =0;
                             pmsCoiTosRel.PmsCoiId = insertedId;
                            _context.PmsCoiTosRel.Add(pmsCoiTosRel);
                         }
                         _context.SaveChanges();

                    }

                    transaction.Commit();
                    ImsLogevents evt = utility.GetLogEvent("PmsCategoryOfInterventioControl", "PutPmsCategoryOfIntervention", "COI updated", UserId, pmsCategoryOfIntervention,IP);
                    _logContext.ImsLogevents.Add(evt);
                    _logContext.SaveChanges();
                }
                catch (Exception ex)
                {

                    transaction.Rollback();
                    ImsLogerrors evt = utility.GetLogError("PmsCategoryOfInterventioControl", "PutPmsCategoryOfIntervention",ex, UserId, pmsCategoryOfIntervention,IP);
                    #region modo alternativo
                    /*
                    StringBuilder sb = new StringBuilder();
                    sb.Append("CC:");
                    sb.Append(pmsCategoryOfIntervention.PmsCoiCode);
                    sb.Append(";");
                    sb.Append("CD:");
                    sb.Append(pmsCategoryOfIntervention.PmsCoiDeleted+"");
                    sb.Append(";");
                    sb.Append("CDS:");
                    sb.Append(pmsCategoryOfIntervention.PmsCoiDescription + "");
                    sb.Append(";");
                    sb.Append("CE:");
                    sb.Append(pmsCategoryOfIntervention.PmsCoiEnabled + "");
                    sb.Append(";");
                    sb.Append("CI:");
                    sb.Append(pmsCategoryOfIntervention.PmsCoiId + "");
                    sb.Append(";");
                    utility.GetLogError("PmsCategoryOfInterventioControl", "PutPmsCategoryOfIntervention", ex, UserId, sb.ToString());
                    */
                    #endregion modo alternativo
                    _logContext.ImsLogerrors.Add(evt);
                    _logContext.SaveChanges();
                    throw ex;

                }
            }

            return NoContent();
        }

        // POST: api/PmsCategoryOfIntervention
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<PmsCategoryOfIntervention>> PostPmsCategoryOfIntervention(PmsCategoryOfIntervention pmsCategoryOfIntervention)
        {
            _context.PmsCategoryOfIntervention.Add(pmsCategoryOfIntervention);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPmsCategoryOfIntervention", new { id = pmsCategoryOfIntervention.PmsCoiId }, pmsCategoryOfIntervention);
        }

        // DELETE: api/PmsCategoryOfIntervention/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PmsCategoryOfIntervention>> DeletePmsCategoryOfIntervention(int id)
        {
            var pmsCategoryOfIntervention = await _context.PmsCategoryOfIntervention.FindAsync(id);
            if (pmsCategoryOfIntervention == null)
            {
                return NotFound();
            }

            _context.PmsCategoryOfIntervention.Remove(pmsCategoryOfIntervention);
            await _context.SaveChangesAsync();

            return pmsCategoryOfIntervention;
        }

        private bool PmsCategoryOfInterventionExists(int id)
        {
            return _context.PmsCategoryOfIntervention.Any(e => e.PmsCoiId == id);
        }
 
    }
}
