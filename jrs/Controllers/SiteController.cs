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
    public class SiteController : ControllerBase
    {
private readonly string _connectionString;
private SqlConnectionStringBuilder sqlConnectionBuilder;
        public SiteController(IConfiguration configuration)
        {
_connectionString = configuration.GetConnectionString("jrsdb");
sqlConnectionBuilder = new SqlConnectionStringBuilder(_connectionString);
        }
        //IMS.
        [HttpGet]
        public IActionResult Getlist()
        {
            string connectionString = _connectionString;
            var resultList = new List<SiteView>();
            sqlConnectionBuilder.MultipleActiveResultSets = false;
            using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM IMS.VI_SiteList", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultList.Add(new SiteView
                            {
                                guid_site_id = reader.GetGuid(0),
                                site_name = reader.GetString(1)
                            });
                        }
                        return Ok(resultList);
                    }
                }
            }
        }
    }
}