using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using jrs.DBContexts;
using jrs.Models;

namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class ImsUsersController : ControllerBase
    {
        private readonly GeneralContext _context;

        public ImsUsersController(GeneralContext context)
        {
            _context = context;
        }

        // GET: api/ImsUsers
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ImsUser>>> GetImsUser()
        {
            return await _context.ImsUser.ToListAsync();
        }

        // GET: api/ImsUsers/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ImsUser>> GetImsUser(Guid id)
        {
            var imsUser = await _context.ImsUser.FindAsync(id);

            if (imsUser == null)
            {
                return NotFound();
            }

            return imsUser;
        }

        // PUT: api/ImsUsers/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImsUser(Guid id, ImsUser imsUser)
        {
            if (id != imsUser.ImsUserUid)
            {
                return BadRequest();
            }

            _context.Entry(imsUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImsUserExists(id))
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

        // POST: api/ImsUsers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ImsUser>> PostImsUser(ImsUser imsUser)
        {
            _context.ImsUser.Add(imsUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetImsUser", new { id = imsUser.ImsUserUid }, imsUser);
        }

        // DELETE: api/ImsUsers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ImsUser>> DeleteImsUser(Guid id)
        {
            var imsUser = await _context.ImsUser.FindAsync(id);
            if (imsUser == null)
            {
                return NotFound();
            }

            _context.ImsUser.Remove(imsUser);
            await _context.SaveChangesAsync();

            return imsUser;
        }

        private bool ImsUserExists(Guid id)
        {
            return _context.ImsUser.Any(e => e.ImsUserUid == id);
        }
    }
}
