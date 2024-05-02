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
    public class NeighborhoodController : ControllerBase
    {
        private readonly string _connectionString;
        private SqlConnectionStringBuilder sqlConnectionBuilder;
        public NeighborhoodController(IConfiguration configuration)
        {
_connectionString = configuration.GetConnectionString("jrsdb");
sqlConnectionBuilder = new SqlConnectionStringBuilder(_connectionString);
        }
        //IMS.
        [HttpGet]
        public IActionResult Getlist()
        {
            string connectionString = _connectionString;
            var resultList = new List<NeighborhoodView>();
            sqlConnectionBuilder.MultipleActiveResultSets = false;
            using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM IMS.VI_NeighborhoodList", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultList.Add(new NeighborhoodView
                            {
                                guid_neighborhood_id = reader.GetGuid(0),
                                neighborhood_name = reader.GetString(1),
                                guid_city_id = reader.GetGuid(2),
                            });
                        }
                        return Ok(resultList);
                    }
                }
            }
        }


        [HttpPost("basedOnCity")]
        public IActionResult basedOnCity([FromBody] string guid_city_id)
        {
            string connectionString = _connectionString;
            var resultList = new List<NeighborhoodView>();
            sqlConnectionBuilder.MultipleActiveResultSets = false;
            using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
            {
                var sql = "SELECT * FROM IMS.VI_NeighborhoodList where guid_city_id = '" + guid_city_id + "'";

                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultList.Add(new NeighborhoodView
                            {
                                guid_neighborhood_id = reader.GetGuid(0),
                                neighborhood_name = reader.GetString(1),
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