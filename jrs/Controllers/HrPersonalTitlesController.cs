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
    public class HrPersonalTitlesController : ControllerBase
    {
        private readonly BiodataContext _context;

        public HrPersonalTitlesController(BiodataContext context)
        {
            _context = context;
        }

        // GET: api/HrPersonalTitles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HrPersonalTitle>>> GetHrPersonalTitle()
        {
            return await _context.HrPersonalTitle.ToListAsync();
        }

        // GET: api/HrPersonalTitles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HrPersonalTitle>> GetHrPersonalTitle(int id)
        {
            var hrPersonalTitle = await _context.HrPersonalTitle.FindAsync(id);

            if (hrPersonalTitle == null)
            {
                return NotFound();
            }

            return hrPersonalTitle;
        }

        // PUT: api/HrPersonalTitles/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHrPersonalTitle(int id, HrPersonalTitle hrPersonalTitle)
        {
            if (id != hrPersonalTitle.HrPersonalTitleId)
            {
                return BadRequest();
            }

            _context.Entry(hrPersonalTitle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HrPersonalTitleExists(id))
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

        // POST: api/HrPersonalTitles
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<HrPersonalTitle>> PostHrPersonalTitle(HrPersonalTitle hrPersonalTitle)
        {
            _context.HrPersonalTitle.Add(hrPersonalTitle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHrPersonalTitle", new { id = hrPersonalTitle.HrPersonalTitleId }, hrPersonalTitle);
        }

        // DELETE: api/HrPersonalTitles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<HrPersonalTitle>> DeleteHrPersonalTitle(int id)
        {
            var hrPersonalTitle = await _context.HrPersonalTitle.FindAsync(id);
            if (hrPersonalTitle == null)
            {
                return NotFound();
            }

            _context.HrPersonalTitle.Remove(hrPersonalTitle);
            await _context.SaveChangesAsync();

            return hrPersonalTitle;
        }

        private bool HrPersonalTitleExists(int id)
        {
            return _context.HrPersonalTitle.Any(e => e.HrPersonalTitleId == id);
        }
    }
}
