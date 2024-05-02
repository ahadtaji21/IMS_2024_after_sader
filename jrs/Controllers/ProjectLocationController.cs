using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using jrs.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class ProjectLocationController : ControllerBase
    {
        private readonly JRSDBContext _context;
        private readonly string _connectionString;
        private SqlConnectionStringBuilder sqlConnectionBuilder;
        public ProjectLocationController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("jrsdb");
            sqlConnectionBuilder = new SqlConnectionStringBuilder(_connectionString);
        }
        //IMS.VI_ProjectLocationList
        [HttpGet]
        public IActionResult Getlist()
        {
            string connectionString = _connectionString;
            var resultList = new List<ProjectLocationView>();
            sqlConnectionBuilder.MultipleActiveResultSets = false;
            using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM IMS.VI_ProjectLocationList", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultList.Add(new ProjectLocationView
                            {
                                guid_project_location_id = reader.GetGuid(0),
                                guid_project_id = reader.GetGuid(1),
                                guid_city_id = reader.GetGuid(2)
                            });
                        }
                        return Ok(resultList);
                    }
                }
            }
        }
    }
}
