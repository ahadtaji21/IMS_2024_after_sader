using jrs.DBContexts;
using jrs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using DocumentFormat.OpenXml.Math;

namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class DonorController : ControllerBase
    {
        private readonly string _connectionString;
        private SqlConnectionStringBuilder sqlConnectionBuilder;
        public DonorController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("jrsdb");
            sqlConnectionBuilder = new SqlConnectionStringBuilder(_connectionString);
        }
        [HttpGet]
        public IActionResult Getlist()
        {
            string connectionString = _connectionString;
            var resultList = new List<DonorView>();
            sqlConnectionBuilder.MultipleActiveResultSets = false;
            using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT * FROM GMT.VI_DonorList", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultList.Add(new DonorView
                            {
                                guid_donor_id = reader.GetGuid(0),
                                donor_name = reader.GetString(1),
                                donor_code = reader.GetString(2)

                            });
                        }
                        return Ok(resultList);
                    }
                }
            }
        }


        [HttpPost("GetListPagination")]
        public IActionResult GetListPagination(PaginationBody body)
        {
            try
            {
                if (body.page <= 0) body.page = 1;
                if (body.limit <= 0) body.page = 10;

                string connectionString = _connectionString;
                var resultList = new List<Donor>();
                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "SELECT guid_donor_id, donor_name, donor_code, donor_category " +
                        "FROM JRSDB.GMT.VI_Donor " +
                        "ORDER BY created_date desc " + 
                        "OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY", connection))
                                        
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@Offset", (body.page - 1) * body.limit);
                        command.Parameters.AddWithValue("@Limit", body.limit);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new Donor
                                {
                                    guid_donor_id = reader.GetGuid(0),
                                    donor_name = reader.GetString(1),
                                    donor_code = reader.GetString(2),
                                    donor_category = reader.GetString(3),
                                });
                            }
                        }
                        int totalCount = 0;
                        using (SqlCommand cmd = new SqlCommand("SELECT count(distinct guid_donor_id) [count] FROM JRSDB.GMT.VI_Donor", connection))
                        {
                            cmd.CommandType = CommandType.Text;
                            totalCount = (int)cmd.ExecuteScalar();

                            using (SqlDataReader reader2 = cmd.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    totalCount = (int)reader2.GetInt32(0);
                                }
                            }
                        }
                        int totalPages = (int)Math.Ceiling((double)totalCount / body.limit);
                        int skip = (body.page - 1) * body.limit;


                        return Ok(new { TotalCount = totalCount, Data = resultList, totalPages = totalPages, currentPage = body.page });


                    }


                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }


        [HttpPost("GetListSearch")]
        public IActionResult GetListSearch(PaginationBody body)
        {
            try
            {
                string where_phrase = body.searchBody.Length > 0 ? "where " : "";

                for(var i =0; i< body.searchBody.Length; i++)
                {
                    where_phrase += body.searchBody[i].search_key + " LIKE '%" + body.searchBody[i].search_query + "%'";
                    if (i + 1 < body.searchBody.Length)
                        where_phrase += " and ";


                }


               string connectionString = _connectionString;
                var resultList = new List<Donor>();
                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "SELECT guid_donor_id, donor_name, donor_code, donor_category " +
                        "FROM JRSDB.GMT.VI_Donor " +
                        where_phrase + 
                        " ORDER BY created_date desc " , connection))

                    {
                        command.CommandType = CommandType.Text;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new Donor
                                {
                                    guid_donor_id = reader.GetGuid(0),
                                    donor_name = reader.GetString(1),
                                    donor_code = reader.GetString(2),
                                    donor_category = reader.GetString(3),
                                });
                            }
                        }
                        return Ok(new { Data = resultList });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }


        [HttpPost("GetDonnorByGuid")]
        public IActionResult GetDonnorByGuid(PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                Donor resultDonnor = new Donor();
                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("[GMT].[GetDonor]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@guid_donor_id", body.guid_donor_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                resultDonnor.guid_donor_id = reader.GetGuid(0);
                                resultDonnor.donor_name = reader.GetString(1);
                                resultDonnor.donor_description = reader.IsDBNull(2) ? "": reader.GetString(2);
                                resultDonnor.donor_code = reader.GetString(3);
                                resultDonnor.guid_donor_category_id = reader.GetGuid(4);
                                resultDonnor.donor_category = reader.GetString(5);
                                resultDonnor.guid_navision_id = reader.IsDBNull(6) ? null : reader.GetGuid(6);
                                resultDonnor.donor_number = reader.IsDBNull(7) ? null : reader.GetString(7);

                            }
                        }
                        return Ok(resultDonnor);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost("GetDonnorContactByGuid")]
        public IActionResult GetDonnorContactByGuid(PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                var resultList = new List<DonorContact>();

                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("GMT.GetDonorContact", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@guid_donor_id", body.guid_donor_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new DonorContact
                                {
                                    guid_donor_contact_id =  reader.GetGuid(0),
                                    guid_donor_id = reader.GetGuid(1),
                                    contact_type = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    donor_contact = reader.IsDBNull(5) ? "" : reader.GetString(5)

                                });
                            }
                        }
                        return Ok(resultList);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }


        [HttpPost("GetDonnorCountryByGuid")]
        public IActionResult GetDonnorCountryByGuid(PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                var resultList = new List<DonorCountry>();

                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("GMT.GetDonorCountry", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@guid_donor_id", body.guid_donor_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new DonorCountry
                                {
                                    guid_donor_country_id = reader.GetGuid(0),
                                    guid_donor_id = reader.GetGuid(1),
                                    guid_JRS_region_country_id = reader.GetGuid(3),
                                    guid_country_id = reader.GetGuid(4),
                                    country_name = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    JRS_region_code = reader.IsDBNull(6) ? "" : reader.GetString(6)

                                });
                            }
                        }
                        return Ok(resultList);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }
        
        [HttpPost("GetDonnorAddressByGuid")]
        public IActionResult GetDonnorAddressByGuid(PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                var resultList = new List<DonorAddress>();

                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("GMT.GetDonorAddress", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@guid_donor_id", body.guid_donor_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new DonorAddress
                                {
                                    guid_donor_address_id = reader.GetGuid(0),
                                    guid_donor_id = reader.GetGuid(1),
                                    country_name = reader.GetString(3),
                                    country_code = reader.GetString(4),
                                    state_name = reader.GetString(5),
                                    city_name = reader.GetString(6),
                                    donor_address = reader.GetString(7),
                                });
                            }
                        }
                        return Ok(resultList);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost("Donor_Update")]
        public IActionResult Donor_Update(Donor body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string storedProcedureName = "GMT.Donor_Update";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_donor_id", body.guid_donor_id);
                    parameters.Add("@donor_name", body.donor_name);
                    parameters.Add("@donor_description", body.donor_description);
                    parameters.Add("@donor_code", body.donor_code);
                    parameters.Add("@guid_donor_category_id", body.guid_donor_category_id);
                    parameters.Add("@donor_number", body.donor_number);
                    parameters.Add("@guid_updated_by", body.userId);
                    
                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Data saved successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }

        [HttpPost("DonorContact_Insert")]
        public IActionResult DonorContact_Insert(DonorContact body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string storedProcedureName = "GMT.DonorContact_Insert";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_donor_id", body.guid_donor_id);
                    parameters.Add("@guid_contact_type_id", body.guid_contact_type_id);
                    parameters.Add("@donor_contact", body.donor_contact);
                    parameters.Add("@guid_created_by", body.userId);


                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Data saved successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }
        [HttpPost("DonorCountry_Insert")]
        public IActionResult DonorCountry_Insert(DonorCountry body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string storedProcedureName = "GMT.DonorCountry_Insert";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_donor_id", body.guid_donor_id);
                    parameters.Add("@guid_JRS_region_country_id", body.guid_JRS_region_country_id);
                    parameters.Add("@donor_country_start_date", body.donor_country_start_date);
                    parameters.Add("@donor_country_end_date", body.donor_country_end_date);
                    parameters.Add("@donor_country_description", body.donor_country_description);
                    parameters.Add("@guid_created_by", body.userId);

                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Data saved successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }

        [HttpPost("DonorAddress_Insert")]
        public IActionResult DonorAddress_Insert(DonorAddress body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string storedProcedureName = "GMT.DonorAddress_Insert";

                    var parameters = new DynamicParameters();

                    parameters.Add("@guid_donor_id", body.guid_donor_id);
                    parameters.Add("@guid_city_id", body.guid_city_id );
                    parameters.Add("@donor_address", body.donor_address );
                    parameters.Add("@guid_created_by", body.userId);

                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Data saved successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }


        [HttpPost("Donnor_Add")]
        public IActionResult Donnor_Add(Donor_Add_Body body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string storedProcedureName = "[GMT].[Donnor_Add]";

                    var parameters = new DynamicParameters();

                    parameters.Add("@donor_name", body.donor_name);
                    parameters.Add("@donor_description", body.donor_description);
                    parameters.Add("@donor_code", body.donor_code);
                    parameters.Add("@guid_donor_category_id", body.guid_donor_category_id);
                    parameters.Add("@donor_number", body.donor_number);
                    parameters.Add("@guid_contact_type_id", body.guid_contact_type_id);
                    parameters.Add("@donor_contact", body.donor_contact);
                    parameters.Add("@guid_JRS_region_country_id", body.guid_JRS_region_country_id);
                    parameters.Add("@donor_country_start_date", body.donor_country_start_date);
                    parameters.Add("@donor_country_end_date", body.donor_country_end_date);
                    parameters.Add("@donor_country_description", body.donor_country_description);
                    parameters.Add("@guid_city_id", body.guid_city_id);
                    parameters.Add("@donor_address", body.donor_address);
                    parameters.Add("@guid_created_by", body.userId);


                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Data saved successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }



        [HttpPost("DonorContact_Delete")]
        public IActionResult DonorContact_Delete(DonorContact body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string storedProcedureName = "GMT.DonorContact_Delete";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_donor_contact_id", body.guid_donor_contact_id);

                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Data deleted successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }
        [HttpPost("DonorCountry_Delete")]
        public IActionResult DonorCountry_Delete(DonorCountry body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string storedProcedureName = "GMT.DonorCountry_Delete";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_donor_country_id", body.guid_donor_country_id);


                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Data deleted successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }

        [HttpPost("DonorAddress_Delete")]
        public IActionResult DonorAddress_Delete(DonorAddress body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string storedProcedureName = "GMT.DonorAddress_Delete";

                    var parameters = new DynamicParameters();

                    parameters.Add("@guid_donor_address_id", body.guid_donor_address_id);


                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Data deleted successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }

        [HttpPost("Donor_Delete")]
        public IActionResult Donor_Delete(Donor body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string storedProcedureName = "GMT.Donor_Delete";

                    var parameters = new DynamicParameters();

                    parameters.Add("@guid_donor_id", body.guid_donor_id);


                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Data deleted successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }


        [HttpPost("GetStatistics")]
        public IActionResult GetStatistics(PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                sqlConnectionBuilder.MultipleActiveResultSets = false;


                var donorStatistics_Category = new List<DonorStatistics_Category>();
                var donorStatistics_Country = new List<DonorStatistics_Country>();
                
                var donor_Statistics = new Donor_Statistics();

                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("GMT.GetDonorStatistics_Category", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@office_id", body.office_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                donorStatistics_Category.Add(new DonorStatistics_Category
                                {
                                    donnorsCount = reader.GetInt32(0),
                                    donor_category = reader.GetString(1)
                                });


                            }
                        }
                    }
                    using (SqlCommand command = new SqlCommand("GMT.GetDonorStatistics_Country", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@office_id", body.office_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                donorStatistics_Country.Add(new DonorStatistics_Country
                                {
                                    country_name = reader.GetString(0),
                                    donnorsCount = reader.GetInt32(1)
                                });
                            }
                        }
                    }
                   
                    donor_Statistics.donorStatistics_Category = donorStatistics_Category;
                    donor_Statistics.donorStatistics_Country = donorStatistics_Country;


                    return Ok(donor_Statistics);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }




    }
}
