using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using jrs.Models;
using jrs.DBContexts;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class HouseholdDataSectionController : ControllerBase
    {
private readonly string _connectionString;
private SqlConnectionStringBuilder sqlConnectionBuilder;
        public HouseholdDataSectionController(IConfiguration configuration)
        {
_connectionString = configuration.GetConnectionString("jrsdb");
sqlConnectionBuilder = new SqlConnectionStringBuilder(_connectionString);
        }
        //PME.
        [HttpGet]
        public IActionResult Getlist()
        {
            string connectionString = _connectionString;
            var resultList = new List<HouseholdDataSectionView>();
            sqlConnectionBuilder.MultipleActiveResultSets = false;
            using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM PME.VI_HouseholdDataSectionList", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultList.Add(new HouseholdDataSectionView
                            {
                                guid_household_datasection_id = reader.GetGuid(0),
                                guid_household_id = reader.GetGuid(1),
                                guid_data_section_id = reader.GetGuid(2),
                            });
                        }
                        return Ok(resultList);
                    }
                }
            }
        }
    }
}