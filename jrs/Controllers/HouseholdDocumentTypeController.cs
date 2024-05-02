using jrs.DBContexts;
using jrs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class HouseholdDocumentTypeController : ControllerBase
    {
private readonly string _connectionString;
private SqlConnectionStringBuilder sqlConnectionBuilder;
        public HouseholdDocumentTypeController
            (IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("jrsdb");
            sqlConnectionBuilder = new SqlConnectionStringBuilder(_connectionString);


        }
        [HttpGet]
        public IActionResult Getlist()
        {
            string connectionString = _connectionString;
            var resultList = new List<HouseholdDocumentTypeView>();
            sqlConnectionBuilder.MultipleActiveResultSets = false;
            using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM [IMS].[VI_HouseholdDocumentTypeList]", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultList.Add(new HouseholdDocumentTypeView
                            {
                                guid_document_type_id = reader.GetGuid(0),
                                document_type = reader.GetString(1)
                            });
                        }
                        return Ok(resultList);
                    }
                }
            }

        }
    }
}