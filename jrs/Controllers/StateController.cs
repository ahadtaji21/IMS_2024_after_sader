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
    public class StateController : ControllerBase
    {
        private readonly string _connectionString;
        private SqlConnectionStringBuilder sqlConnectionBuilder;
        public StateController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("jrsdb");
            sqlConnectionBuilder = new SqlConnectionStringBuilder(_connectionString);
        }

        [HttpGet]
        public IActionResult Getlist()
        {
            string connectionString = _connectionString;
            var resultList = new List<StateView>();
            sqlConnectionBuilder.MultipleActiveResultSets = false;
            using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
            {
                var sql = "SELECT * FROM IMS.VI_StateList order by state_name ";

                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultList.Add(new StateView
                            {
                                guid_state_id = reader.GetGuid(0),
                                state_name = reader.GetString(1),
                                guid_country_id = reader.GetGuid(2)
                            });
                        }
                        return Ok(resultList);
                    }
                }
            }
        }

        [HttpPost("basedOnCountry")]
        public IActionResult bsasedOnCountry([FromBody] string guid_country_id)
        {
            string connectionString = _connectionString;
            var resultList = new List<StateView>();
            sqlConnectionBuilder.MultipleActiveResultSets = false;
            using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
            {
                var sql = "SELECT * FROM IMS.VI_StateList where guid_country_id = '" + guid_country_id + "'";

                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultList.Add(new StateView
                            {
                                guid_state_id = reader.GetGuid(0),
                                state_name = reader.GetString(1),
                                guid_country_id = reader.GetGuid(2)
                            });
                        }
                        return Ok(resultList);
                    }
                }
            }
        }
    
    }
}
