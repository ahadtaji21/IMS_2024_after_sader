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
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class CountryController : ControllerBase
    {
        private readonly string _connectionString;
        private SqlConnectionStringBuilder sqlConnectionBuilder;
        public CountryController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("jrsdb");
            sqlConnectionBuilder = new SqlConnectionStringBuilder(_connectionString);
        }
        [HttpGet]
        public IActionResult Getlist()
        {
            string connectionString = _connectionString;
            var resultList = new List<Country_view>();
            sqlConnectionBuilder.MultipleActiveResultSets = false;
            using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM IMS.VI_CountryList order by country_name", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultList.Add(new Country_view
                            {
                                guid_country_id = reader.GetGuid(0),
                                country_name = reader.GetString(1),
                                phone_code = reader.GetString(2),
                            });
                        }
                        return Ok(resultList);
                    }
                }
            }
        }

        [HttpGet("getNationalities")]
        public IActionResult GetNationalities()
        {
            string connectionString = _connectionString;
            var resultList = new List<Nationality_view>();
            sqlConnectionBuilder.MultipleActiveResultSets = false;
            using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM IMS.VI_NationalityList order by nationality", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultList.Add(new Nationality_view
                            {
                                guid_country_id = reader.GetGuid(0),
                                nationality = reader.GetString(1),
                                country_code = reader.GetString(2)
                            });
                        }
                        return Ok(resultList);
                    }
                }
            }
        }

    }
}