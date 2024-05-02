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
    public class ImsController : ControllerBase
    {
        private readonly  IMSContext _context;
        private readonly IMSLogContext _logContext;


        public ImsController(IMSContext context,IMSLogContext logContext)
        {
            _context = context;
            _logContext = logContext;
        }



        [HttpGet("GetImsLanguage")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<ImsLanguage>>> GetImsLanguage()
        {
            //return await _context.PmsCategoryOfIntervention.ToListAsync();
            var imsLanguage = _context.Set<ImsLanguage>().Where(x=>x.ImsLanguageLocale != null).ToList();
            return imsLanguage;
        }
         [HttpGet("{id}")]
        [Consumes("application/json")]
       public async Task<ActionResult<ImsLanguage>> GetImsLanguage(int id)
        {
            var imsLanguage = await _context.ImsLanguage.FindAsync(id);

            if (imsLanguage == null)
            {
                return NotFound();
            }

            return imsLanguage;
        }

        [HttpGet("GetImsLabel")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<ImsLabels>>> GetImsLabel()
        {
            //return await _context.PmsCategoryOfIntervention.ToListAsync();
            var imsLabels = _context.Set<ImsLabels>().ToList();
            return imsLabels;
        }
 [HttpGet("GetImsLabelById")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<ImsLabels>>> GetImsLabel(int id_label_number)
        {
            //return await _context.PmsCategoryOfIntervention.ToListAsync();
            var imsLabels = _context.Set<ImsLabels>().Where(x=>x.ImsLabelsNumber == id_label_number).ToList();
            return imsLabels;
        }
    }
}
