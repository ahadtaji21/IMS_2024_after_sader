using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using jrs.DBContexts;
using jrs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScaffoldModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace jrs.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class EducationLevelController : ControllerBase
    {
        private readonly GeneralContext _context;
        public EducationLevelController(GeneralContext genContext)
        {
            _context = genContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EducationalLevel>>> GetList()
        {
            try
            {
                var sql = "SELECT * FROM PME.VI_IndividualEducationalLevelList";

                using (var connection = _context.Database.GetDbConnection())
                {
                    connection.Open();
                    var theList = await connection.QueryAsync(sql);
                    return Ok(theList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
