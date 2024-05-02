using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using jrs.Models;
using jrs.DBContexts;

namespace jrs.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class CanceledHouseholdController : ControllerBase
    {
        private readonly GeneralContext _context;

        public CanceledHouseholdController(GeneralContext context)
        {
            _context = context;
        }

    
    }
}
