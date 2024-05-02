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
    public class HrGendersController : ControllerBase
    {
        private readonly BiodataContext _context;

        public HrGendersController(BiodataContext context)
        {
            _context = context;
        }

        // GET: api/HrGenders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HrGender>>> GetHrGender()
        {
            List<HrGender> ret = await _context.HrGender.ToListAsync();
            return ret;
        }

        // GET: api/HrGenders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HrGender>> GetHrGender(int id)
        {
            var hrGender = await _context.HrGender.FindAsync(id);

            if (hrGender == null)
            {
                return NotFound();
            }

            return hrGender;
        }


        //// PUT: api/HrGenders/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for
        //// more details see https://aka.ms/RazorPagesCRUD.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutHrGender(int id, HrGender hrGender)
        //{
        //    if (id != hrGender.HrGenderId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(hrGender).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!HrGenderExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/HrGenders
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for
        //// more details see https://aka.ms/RazorPagesCRUD.
        //[HttpPost]
        //public async Task<ActionResult<HrGender>> PostHrGender(HrGender hrGender)
        //{
        //    _context.HrGender.Add(hrGender);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetHrGender", new { id = hrGender.HrGenderId }, hrGender);
        //}

        //// DELETE: api/HrGenders/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<HrGender>> DeleteHrGender(int id)
        //{
        //    var hrGender = await _context.HrGender.FindAsync(id);
        //    if (hrGender == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.HrGender.Remove(hrGender);
        //    await _context.SaveChangesAsync();

        //    return hrGender;
        //}

        private bool HrGenderExists(int id)
        {
            return _context.HrGender.Any(e => e.HrGenderId == id);
        }
    }
}
