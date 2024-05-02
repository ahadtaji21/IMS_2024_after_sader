using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using jrs.DBContexts;
using jrs.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class HrBiodataController : ControllerBase
    {
        private readonly BiodataContext _context;
        private readonly GeneralContext _genContext;
        private readonly string _connectionString;
        private SqlConnectionStringBuilder sqlConnectionBuilder;


        public HrBiodataController(BiodataContext context, GeneralContext genContext, IConfiguration configuration)
        {

            _context = context;
            _genContext = genContext;
            _connectionString = configuration.GetConnectionString("jrsdb");
            sqlConnectionBuilder = new SqlConnectionStringBuilder(_connectionString);

        }

        // GET: api/HrBiodata
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<HrBiodata>>> GetHrBiodata()
        {
            var lookUps = await ImsLookup.GetLookupsForType(new string[] {"GENDER", "PERTIT"}, _genContext);

            var biodatas = await _context.HrBiodata
                            // .Select( b => new HrBiodata {
                            //     HrBiodataId = b.HrBiodataId,
                            //     HrBiodataUserUid = b.HrBiodataUserUid,
                            //     HrBiodataSurname = b.HrBiodataSurname,
                            //     HrBiodataMiddleName = b.HrBiodataMiddleName,
                            //     HrBiodataName = b.HrBiodataName,
                            //     HrBiodataGenederLookupId = b.HrBiodataGenederLookupId,
                            //     HrBiodataPersonalTitleLookupId = b.HrBiodataPersonalTitleLookupId
                            // })
                            .ToListAsync();
            
            //Resolve lookups
            biodatas = biodatas.Select(b => {
                if(b.HrBiodataGenederLookupId.HasValue){
                    b.LookupGender = lookUps.Find(l => l.ImsLookupId == b.HrBiodataGenederLookupId);
                }
                if(b.HrBiodataPersonalTitleLookupId.HasValue){
                    b.LookupPersonalTtitle = lookUps.Find(l => l.ImsLookupId == b.HrBiodataPersonalTitleLookupId);
                }
                return b;
            }).ToList();

            return biodatas;
        }

        // GET: api/HrBiodata/5
        [HttpGet("{id}")]
        [Authorize("Bearer")]
        public async Task<ActionResult<HrBiodata>> GetHrBiodata(int id)
        {
            var hrBiodata = await _context.HrBiodata.FindAsync(id);

            if (hrBiodata == null)
            {
                return NotFound();
            }

            hrBiodata.LookupGender = _genContext.ImsLookup.Find(hrBiodata.HrBiodataGenederLookupId);
            // _context.Entry(hrBiodata)
            //     .Reference( b => b.LookupGender)
            //     .Load();

            return hrBiodata;
        }

        // PUT: api/HrBiodata/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHrBiodata(int id, HrBiodata hrBiodata)
        {
            if (id != hrBiodata.HrBiodataId)
            {
                return BadRequest();
            }

            _context.Entry(hrBiodata).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HrBiodataExists(id))
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

        // POST: api/HrBiodata
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<HrBiodata>> PostHrBiodata(HrBiodata hrBiodata)
        {
            _context.HrBiodata.Add(hrBiodata);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHrBiodata", new { id = hrBiodata.HrBiodataId }, hrBiodata);
        }

        // DELETE: api/HrBiodata/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<HrBiodata>> DeleteHrBiodata(int id)
        {
            var hrBiodata = await _context.HrBiodata.FindAsync(id);
            if (hrBiodata == null)
            {
                return NotFound();
            }

            _context.HrBiodata.Remove(hrBiodata);
            await _context.SaveChangesAsync();

            return hrBiodata;
        }

        private bool HrBiodataExists(int id)
        {
            return _context.HrBiodata.Any(e => e.HrBiodataId == id);
        }


        [HttpPost("GetStatistics")]
        public IActionResult GetStatistics(PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                sqlConnectionBuilder.MultipleActiveResultSets = false;


                var hrBiodataStatistics_Gender = new List<HrBiodataStatistics_Gender>();
                var hrBiodataStatistics_Count = new List<HrBiodataStatistics_Count>();
                var hrBiodataStatistics_Departments = new List<HrBiodataStatistics_Departments>();

                var hrBiodataStatistics = new HrBiodataStatistics();

                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("dbo.BioDataStatistics_Gender", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@office_id", body.office_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                hrBiodataStatistics_Gender.Add(new HrBiodataStatistics_Gender
                                {
                                    gender = reader.GetString(0),
                                    year = reader.GetInt32(1),
                                    count = reader.GetInt32(2)
                                });
                            }
                        }
                    }
                    using (SqlCommand command = new SqlCommand("BioDataStatistics_Count", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@office_id", body.office_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                hrBiodataStatistics_Count.Add(new HrBiodataStatistics_Count
                                {
                                    count = reader.GetInt32(0)
                                });
                            }
                        }
                    }
                    using (SqlCommand command = new SqlCommand("BioDataStatistics_Departments", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@office_id", body.office_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                hrBiodataStatistics_Departments.Add(new HrBiodataStatistics_Departments
                                {
                                    department_name = reader.GetString(0),
                                    count = reader.GetInt32(1)
                                });
                            }
                        }
                    }

                    hrBiodataStatistics.hrBiodataStatistics_Gender = hrBiodataStatistics_Gender;
                    hrBiodataStatistics.hrBiodataStatistics_Count = hrBiodataStatistics_Count;
                    hrBiodataStatistics.hrBiodataStatistics_Departments = hrBiodataStatistics_Departments;


                    return Ok(hrBiodataStatistics);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }



    }
}
