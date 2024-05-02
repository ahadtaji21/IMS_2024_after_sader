using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jrs.DBContexts;
using jrs.Models;

namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class PmsTypeOfServiceController : ControllerBase
    {
        private readonly PMSContext _context;

        public PmsTypeOfServiceController(PMSContext context)
        {
            _context = context;
        }

        // GET: api/PmsTypeOfService
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PmsTypeOfService>>> GetPmsTypeOfService()
        {
            return await _context.PmsTypeOfService.ToListAsync();
        }

        // GET: api/PmsTypeOfService/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PmsTypeOfService>> GetPmsTypeOfService(int id)
        {
            var pmsTypeOfService = await _context.PmsTypeOfService.FindAsync(id);

            if (pmsTypeOfService == null)
            {
                return NotFound();
            }

            return pmsTypeOfService;
        }

        // PUT: api/PmsTypeOfService/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPmsTypeOfService(int id, PmsTypeOfService pmsTypeOfService)
        {
            if (id != pmsTypeOfService.PmsTosId)
            {
                return BadRequest();
            }

            _context.Entry(pmsTypeOfService).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PmsTypeOfServiceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PmsTypeOfService
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<PmsTypeOfService>> PostPmsTypeOfService(PmsTypeOfService pmsTypeOfService)
        {
            _context.PmsTypeOfService.Add(pmsTypeOfService);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPmsTypeOfService", new { id = pmsTypeOfService.PmsTosId }, pmsTypeOfService);
        }

        // DELETE: api/PmsTypeOfService/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PmsTypeOfService>> DeletePmsTypeOfService(int id)
        {
            var pmsTypeOfService = await _context.PmsTypeOfService.FindAsync(id);
            if (pmsTypeOfService == null)
            {
                return NotFound();
            }

            _context.PmsTypeOfService.Remove(pmsTypeOfService);
            await _context.SaveChangesAsync();

            return pmsTypeOfService;
        }

        private bool PmsTypeOfServiceExists(int id)
        {
            return _context.PmsTypeOfService.Any(e => e.PmsTosId == id);
        }
    }
}
