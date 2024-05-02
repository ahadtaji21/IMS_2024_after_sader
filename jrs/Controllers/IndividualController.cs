using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jrs.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Threading.Tasks;

namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class IndividualController : ControllerBase
    {
        private readonly string _connectionString;
        private SqlConnectionStringBuilder sqlConnectionBuilder;
        private IConfiguration _configuration;

        public IndividualController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("jrsdb");
            sqlConnectionBuilder = new SqlConnectionStringBuilder(_connectionString);

        }
        //PME.VI_IndividualList 

        [HttpPost("GetListPagination")]
        public IActionResult GetListPagination(Individual_PaginationBody body)
        {

            try
            {
                if (body.page <= 0) body.page = 1;
                if (body.limit <= 0) body.page = 10;

                string connectionString = _connectionString;
                var resultList = new List<IndividualWizardClass>();
                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "SELECT [guid_individual_id], [individual_first_name],  [individual_last_name]" +
                        " ,   [date_of_birth]" +
                        " , [is_active], guid_data_section_id, JRS_individual_identifier, gender, nationality, document_number " +
                        " FROM [JRSDB].PME.VI_Individual_GetList_DataSection  " +
                        " WHERE guid_data_section_id = @guid_data_section_id " +
                        " ORDER BY created_date desc " +
                        " OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@Offset", (body.page - 1) * body.limit);
                        command.Parameters.AddWithValue("@Limit", body.limit);
                        command.Parameters.AddWithValue("@guid_data_section_id", body.guid_data_section_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new IndividualWizardClass
                                {
                                    guid_individual_id = reader.GetGuid(0),
                                    individual_first_name = reader.GetString(1),
                                    individual_last_name =  reader.GetString(2),
                                    date_of_birth = reader.GetDateTime(3),
                                    is_active = reader.GetBoolean(4),
                                    guid_data_section_id = reader.GetGuid(5),
                                    JRS_individual_identifier = reader.GetString(6),
                                    gender = reader.GetString(7),
                                    nationality = reader.GetString(8),
                                    document_number = reader.GetString(9)

                                });;
                            }
                        }
                        int totalCount = 0;
                        using (SqlCommand cmd = new SqlCommand("SELECT count(distinct guid_individual_id) [count] FROM JRSDB.PME.VI_Individual_GetList_DataSection where guid_data_section_id = '"+body.guid_data_section_id+"'", connection))
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
        public IActionResult GetListSearch(Individual_PaginationBody body)
        {
            try
            {
                string where_phrase = body.searchBody.Length > 0 ? " " : "";

                for (var i = 0; i < body.searchBody.Length; i++)
                {
                    where_phrase += body.searchBody[i].search_key + " LIKE '%" + body.searchBody[i].search_query.Trim() + "%'";
                    if (i + 1 < body.searchBody.Length)
                        where_phrase += " and ";
                }


                string connectionString = _connectionString;
                var resultList = new List<IndividualWizardClass>();
                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                               "SELECT [guid_individual_id], [individual_first_name],  [individual_last_name]" +
                        " ,   [date_of_birth]" +
                        " , [is_active], guid_data_section_id, JRS_individual_identifier, gender, nationality , document_number " +
                        " FROM [JRSDB].PME.VI_Individual_GetList_DataSection  " +
                        " WHERE guid_data_section_id = @guid_data_section_id " +
                        " and " + where_phrase +

                        " ORDER BY created_date desc " +
                        " ", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@guid_data_section_id", body.guid_data_section_id);


                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new IndividualWizardClass
                                {
                                    guid_individual_id = reader.GetGuid(0),
                                    individual_first_name = reader.GetString(1),
                                    individual_last_name = reader.GetString(2),
                                    date_of_birth = reader.GetDateTime(3),
                                    is_active = reader.GetBoolean(4),
                                    guid_data_section_id = reader.GetGuid(5),
                                    JRS_individual_identifier = reader.GetString(6),
                                    gender = reader.GetString(7),
                                    nationality = reader.GetString(8),
                                    document_number = reader.GetString(9)


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









        [HttpPost("Individual_Add")]
        public IActionResult Individual_Add(IndividualWizardClass body)
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
                    string storedProcedureName = "PMS.Individual_Add";
                    var parameters = new DynamicParameters();

                    parameters.Add("@guid_data_section_id", body.guid_data_section_id);
                    parameters.Add("@individual_first_name", body.individual_first_name);
                    parameters.Add("@individual_first_name_native_lang", body.individual_first_name_native_lang);
                    parameters.Add("@individual_last_name", body.individual_last_name);
                    parameters.Add("@individual_last_name_native_lang", body.individual_last_name_native_lang);
                    parameters.Add("@gender_id", body.gender_id);
                    parameters.Add("@individual_father_name", body.individual_father_name);
                    parameters.Add("@individual_mother_name", body.individual_mother_name);
                    parameters.Add("@guid_native_language_id", body.guid_native_language_id);
                    parameters.Add("@date_of_birth", body.date_of_birth);
                    parameters.Add("@place_of_birth", body.place_of_birth);
                    parameters.Add("@place_of_birth_native_lang", body.place_of_birth_native_lang);
                    parameters.Add("@guid_native_nationality_id", body.guid_native_nationality_id);
                    parameters.Add("@IndividualDocumentTypesJson", body.individualDocumentType);
                    parameters.Add("@HouseholdDocumentTypesJson", body.householdDocumentType);
                    parameters.Add("@guid_educational_level_id", body.guid_educational_level_id);
                    parameters.Add("@education_description", body.education_description);
                    parameters.Add("@guid_marital_status_id", body.guid_marital_status_id);
                    parameters.Add("@mobileNumber", body.mobileNumber);
                    parameters.Add("@mobileNumber2", body.mobileNumber2);
                    parameters.Add("@guid_special_need_type_id", body.guid_special_need_type_id);
                    parameters.Add("@email", body.email);
                    parameters.Add("@relationships_type_id", body.relationships_type_id);
                    parameters.Add("@guid_neighborhood_id", body.guid_neighborhood_id);
                    parameters.Add("@rent_value", body.rent_value);


                    parameters.Add("@guid_address_property_type_id", body.guid_address_property_type_id);
                    parameters.Add("@guid_address_status_type_id", body.guid_address_status_type_id);
                    parameters.Add("@is_pregnant", body.is_pregnant);
                    parameters.Add("@total_rooms", body.totalRooms);
                    parameters.Add("@household_address", body.household_address);
                    parameters.Add("@is_chronic", body.is_chronic);
                    parameters.Add("@guid_city_id", body.guid_city_id);
                    parameters.Add("@guid_household_status_type_id ", body.guid_household_status_type_id);

                    parameters.Add("@address_notes ", body.address_notes);
                    parameters.Add("@address_housemates ", body.address_housemates);
                    
                    parameters.Add("@household_provider", body.household_provider);
                    parameters.Add("@household_provider_gender_id", body.household_provider_gender_id);
                    parameters.Add("@household_provider_relation_type_id", body.household_provider_relation_type_id);
                    parameters.Add("@owner_same_as_provider", body.owner_same_as_provider);
                    

                    parameters.Add("@ResultOut", dbType: DbType.String, direction: ParameterDirection.Output, size: 100);

                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    string outputValue = parameters.Get<string>("@ResultOut");


                    return Ok(new { Message = outputValue });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }

       



       
        [HttpPost("Individual_Delete")]
        public IActionResult Individual_Delete(Individual body)
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

                    string storedProcedureName = "PME.Individual_Delete";

                    var parameters = new DynamicParameters();

                    parameters.Add("@guid_individual_id", body.guid_individual_id);


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
        public IActionResult GetStatistics(Individual_PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                sqlConnectionBuilder.MultipleActiveResultSets = false;


                var individualStatistics_Gender = new List<IndividualStatistics_Gender>();

                var individual_Statistics = new Individual_Statistics();

                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PME.GetIndividualsStatistics_Gender", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@office_id", body.office_id);
                        command.Parameters.AddWithValue("@guid_data_section_id", body.guid_data_section_id);
                        

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                individualStatistics_Gender.Add(new IndividualStatistics_Gender
                                {
                                    individualsCount = reader.GetInt32(0),
                                    gender = reader.GetString(1)
                                });
                            }
                        }
                    }

                    individual_Statistics.individualStatistics_Gender = individualStatistics_Gender;
                    

                    return Ok(individual_Statistics);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }
        [HttpPost("GetIndividualByGuid")]
        public IActionResult GetIndividualByGuid(Individual_PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                IndividualWizardClass resultIndividual = new IndividualWizardClass();
                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("[PME].[GetIndividual]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@guid_individual_id", body.guid_individual_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                resultIndividual.guid_individual_id = reader.GetGuid(0);
                                resultIndividual.guid_data_section_id = reader.GetGuid(1);
                                resultIndividual.individual_first_name = reader.GetString(2);
                                resultIndividual.individual_first_name_native_lang = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                resultIndividual.individual_last_name = reader.GetString(4);
                                resultIndividual.individual_last_name_native_lang = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                resultIndividual.individual_mother_name = reader.IsDBNull(6) ? "" : reader.GetString(6);
                                resultIndividual.individual_father_name = reader.IsDBNull(7) ? "" : reader.GetString(7);
                                resultIndividual.gender_id = reader.GetInt32(8);
                                resultIndividual.guid_native_language_id = reader.IsDBNull(9) ? Guid.Empty : reader.GetGuid(9);
                                resultIndividual.date_of_birth = reader.IsDBNull(10) ? new DateTime(1900,1,1) : reader.GetDateTime(10);
                                resultIndividual.place_of_birth = reader.IsDBNull(11) ? "" :  reader.GetString(11);
                                resultIndividual.place_of_birth_native_lang = reader.IsDBNull(12) ? "" : reader.GetString(12);
                            }
                        }
                        return Ok(resultIndividual);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }


        [HttpPost("GetIndividualEducationByGuid")]
        public IActionResult GetIndividualEducationByGuid(Individual_PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                var resultList = new List<IndividualEducationalLevel>();

                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PME.GetIndividualEducationByGuid", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@guid_individual_id", body.guid_individual_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new IndividualEducationalLevel
                                {
                                    guid_educational_level_id = reader.GetGuid(0),
                                    guid_individual_id = reader.GetGuid(1),
                                    description = reader.GetString(2),
                                    is_active = reader.GetBoolean(3),
                                    educational_level = reader.GetString(4)
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

        [HttpPost("GetIndividualSpecialNeedByGuid")]
        public IActionResult GetIndividualSpecialNeedByGuid(Individual_PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                var resultList = new List<SpecialNeed>();

                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PME.GetIndividualSpecialNeedByGuid", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@guid_individual_id", body.guid_individual_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new SpecialNeed
                                {
                                    guid_special_need_id = reader.GetGuid(0),
                                    guid_individual_id = reader.GetGuid(1),
                                    special_need_type = reader.GetString(2),
                                    is_active = reader.GetBoolean(3)
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

        [HttpPost("GetIndividualPregnancyByGuid")]
        public IActionResult GetIndividualPregnancyByGuid(Individual_PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                var resultList = new List<IndividualPregnancy>();

                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PME.GetIndividualPregnancyByGuid", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@guid_individual_id", body.guid_individual_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new IndividualPregnancy
                                {
                                    guid_pregnant_id = reader.GetGuid(0),
                                    guid_individual_id = reader.GetGuid(1),
                                    pregnancy_start_date = reader.GetDateTime(2),
                                    is_active = reader.GetBoolean(3)
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


        [HttpPost("GetIndividualContactByGuid")]
        public IActionResult GetIndividualContactByGuid(Individual_PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                var resultList = new List<IndividualContact>();

                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PME.GetIndividualContactByGuid", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@guid_individual_id", body.guid_individual_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new IndividualContact
                                {
                                    guid_individual_contact_id = reader.GetGuid(0),
                                    guid_individual_id = reader.GetGuid(1),
                                    contact_type = reader.GetString(2),
                                    contact_value = reader.GetString(3),
                                    is_active = reader.GetBoolean(4)
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

        [HttpPost("GetIndividualDocumentsByGuid")]
        public IActionResult GetIndividualDocumentsByGuid(Individual_PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                var resultList = new List<IndividualDocumentType>();

                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PME.GetIndividualDocumentsByGuid", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@guid_individual_id", body.guid_individual_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new IndividualDocumentType
                                {
                                    guid_individual_document_type_id = reader.GetGuid(0),
                                    guid_individual_id = reader.GetGuid(1),
                                    document_type = reader.GetString(2),
                                    document_number = reader.GetString(3),
                                    is_active = reader.GetBoolean(4)
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

        [HttpPost("GetIndividualHouseholdDocumentsByGuid")]
        public IActionResult GetIndividualHouseholdDocumentsByGuid(Individual_PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                var resultList = new List<IndividualDocumentType>();

                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PME.GetIndividualHouseholdDocumentsByGuid", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@guid_individual_id", body.guid_individual_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new IndividualDocumentType
                                {
                                    document_type = reader.GetString(0),
                                    document_number = reader.GetString(1),
                                    document_parent = reader.GetString(2),
                                    is_active = reader.GetBoolean(3),

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

        [HttpPost("GetIndividualNationalityByGuid")]
        public IActionResult GetIndividualNationalityByGuid(Individual_PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                var resultList = new List<IndividualNationality>();

                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PME.GetIndividualNationalityByGuid", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@guid_individual_id", body.guid_individual_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new IndividualNationality
                                {
                                    guid_individual_nationality_id = reader.GetGuid(0),
                                    guid_individual_id = reader.GetGuid(1),
                                    native_nationality = reader.GetString(2),
                                    is_active = reader.GetBoolean(3),

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

        [HttpPost("GetIndividualMaritalStatusByGuid")]
        public IActionResult GetIndividualMaritalStatusByGuid(Individual_PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                var resultList = new List<IndividualMaritalStatus>();

                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PME.GetIndividualMaritalStatusByGuid", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@guid_individual_id", body.guid_individual_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new IndividualMaritalStatus
                                {
                                    guid_individual_marital_status_id = reader.GetGuid(0),
                                    guid_individual_id = reader.GetGuid(1),
                                    marital_status = reader.GetString(2),
                                    is_active = reader.GetBoolean(3),

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






        [HttpPost("GetIndividualHouseholdByGuid")]
        public IActionResult GetIndividualHouseholdByGuid(Individual_PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                var resultList = new List<HouseholdMember>();

                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PME.GetIndividualHouseholdByGuid", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@guid_individual_id", body.guid_individual_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new HouseholdMember
                                {
                                    jrs_household_identifier = reader.GetString(0),
                                    household_provider_relation_type_id = reader.GetInt32(1),
                                    household_provider = reader.GetString(2),
                                    household_provider_gender_id= reader.GetInt32(3),
                                    file_open_date = reader.GetDateTime(4),

                                    relationships_type_id = reader.GetInt32(5),
                                    is_owner = reader.GetBoolean(6),
                                    is_orphan = reader.IsDBNull(7) ? false: reader.GetBoolean(7),
                                    is_active = reader.GetBoolean(8),
                                    owner_same_as_provider = reader.GetBoolean(9)

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
        
        [HttpPost("GetIndividualAddressByGuid")]
        public IActionResult GetIndividualAddressByGuid(Individual_PaginationBody body)
        {
            try
            {
                string connectionString = _connectionString;
                var resultList = new List<HouseholdAddress>();

                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PME.GetIndividualAddressByGuid", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@guid_individual_id", body.guid_individual_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new HouseholdAddress
                                {

                                   guid_country_id = reader.GetGuid(0),
                                    guid_state_id = reader.GetGuid(1),
                                    guid_city_id = reader.GetGuid(2),
                                    guid_neighborhood_id = reader.GetGuid(3),
                                    rent_value = reader.GetDecimal(4),
                                    household_address = reader.GetString(5),
                                    guid_address_property_type_id = reader.GetGuid(6),
                                    guid_address_status_type_id = reader.GetGuid(7),
                                    is_native_address = reader.GetBoolean(8),   



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







        [HttpPost("Individual_Update")]
        public IActionResult Individual_Update(IndividualWizardClass body)
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

                    string storedProcedureName = "PME.Individual_update";

                    var parameters = new DynamicParameters();

                    parameters.Add("@guid_individual_id ", body.guid_individual_id);

                    parameters.Add("@individual_first_name", body.individual_first_name);
                    parameters.Add("@individual_first_name_native_lang", body.individual_first_name_native_lang);
                    parameters.Add("@individual_last_name", body.individual_last_name);
                    parameters.Add("@individual_last_name_native_lang", body.individual_last_name_native_lang);
                    parameters.Add("@individual_mother_name", body.individual_mother_name);
                    parameters.Add("@individual_father_name", body.individual_father_name);
                    parameters.Add("@gender_id", body.gender_id);
                    parameters.Add("@guid_native_language_id ", body.guid_native_language_id);
                    parameters.Add("@date_of_birth", body.date_of_birth);
                    parameters.Add("@place_of_birth", body.place_of_birth);
                    parameters.Add("@place_of_birth_native_lang", body.place_of_birth_native_lang);
                    parameters.Add("@guid_data_section_id ", body.guid_data_section_id);
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


        [HttpPost("IndividualHouseholdAddress_Update")]
        public IActionResult IndividualHouseholdAddress_Update(HouseholdAddress body)
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

                    string storedProcedureName = "PME.IndividualHouseholdAddress_Update";

                    var parameters = new DynamicParameters();

                    parameters.Add("@guid_neighborhoood_id ", body.guid_neighborhood_id);
                    parameters.Add("@rent_value", body.rent_value);
                    parameters.Add("@household_address", body.household_address);
                    parameters.Add("@guid_address_property_type_id", body.guid_address_property_type_id);
                    parameters.Add("@guid_address_status_type_id", body.guid_address_status_type_id);
                    parameters.Add("@is_native_address", body.is_native_address);

                    parameters.Add("@guid_individual_id ", body.guid_individual_id);
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


        [HttpPost("IndividualEducation_Insert")]
        public IActionResult IndividualEducation_Insert(IndividualEducationalLevel body)
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

                    string storedProcedureName = "PME.IndividualEducation_Insert";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_educational_level_id", body.guid_educational_level_id);
                    parameters.Add("@education_description", body.education_level_description);
                    parameters.Add("@guid_individual_id", body.guid_individual_id);
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

        [HttpPost("IndividualSpecialNeed_Insert")]
        public IActionResult IndividualSpecialNeed_Insert(SpecialNeed body)
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

                    string storedProcedureName = "PME.IndividualSpecialNeed_Insert";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_special_need_type_ids", body.guid_special_need_type_ids);

                    parameters.Add("@guid_individual_id", body.guid_individual_id);
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

        [HttpPost("IndividualDocument_Insert")]
        public IActionResult IndividualDocument_Insert(IndividualHouseholdDocumentType body)
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

                    string storedProcedureName = "PME.IndividualDocument_Insert";

                    var parameters = new DynamicParameters();
                    parameters.Add("@IndividualDocumentTypesJson", body.individualDocumentType);
                    parameters.Add("@HouseholdDocumentTypesJson", body.householdDocumentType);
                    parameters.Add("@guid_individual_id", body.guid_individual_id);
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

        [HttpPost("IndividualContact_Insert")]
        public IActionResult IndividualContact_Insert(IndividualContact body)
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

                    string storedProcedureName = "PME.IndividualContact_Insert";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_contact_type_id", body.guid_contact_type_id);
                    parameters.Add("@contact_value", body.contact_value);

                    parameters.Add("@guid_individual_id", body.guid_individual_id);
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
        
        [HttpPost("IndividualNationality_Insert")]
        public IActionResult IndividualNationality_Insert(IndividualNationality body)
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

                    string storedProcedureName = "PME.IndividualNationality_Insert";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_country_id", body.guid_country_id);

                    parameters.Add("@guid_individual_id", body.guid_individual_id);
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
        
        [HttpPost("IndividualPregnancy_Insert")]
        public IActionResult IndividualPregnancy_Insert(IndividualPregnancy body)
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

                    string storedProcedureName = "PME.IndividualPregnancy_Insert";

                    var parameters = new DynamicParameters();
                    parameters.Add("@pregnancy_start_date", body.pregnancy_start_date);

                    parameters.Add("@guid_individual_id", body.guid_individual_id);
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
        [HttpPost("IndividualMaritalStatus_Insert")]
        public IActionResult IndividualMaritalStatus_Insert(IndividualMaritalStatus body)
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

                    string storedProcedureName = "PME.IndividualMaritalStatus_Insert";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_marital_status_id", body.guid_marital_status_id);

                    parameters.Add("@guid_individual_id", body.guid_individual_id);
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



        [HttpPost("IndividualHousehold_Update")]
        public IActionResult IndividualHousehold_Update(HouseholdMember body)
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

                    string storedProcedureName = "PME.IndividualHousehold_Update";

                    var parameters = new DynamicParameters();
                    parameters.Add("@owner_same_as_provider", body.owner_same_as_provider);
                    parameters.Add("@household_provider", body.household_provider);
                    parameters.Add("@household_provider_gender_id", body.household_provider_gender_id);
                    parameters.Add("@household_provider_relation_type_id", body.household_provider_relation_type_id);
                    parameters.Add("@relationships_type_id", body.relationships_type_id);
                    parameters.Add("@file_open_date", body.file_open_date);
                    parameters.Add("@is_orphan", body.is_orphan);
                    parameters.Add("@is_active", body.is_active);
                    parameters.Add("@guid_individual_id", body.guid_individual_id);
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





        [HttpPost("IndividualPregnancy_Delete")]
        public IActionResult IndividualPregnancy_Delete(IndividualPregnancy body)
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

                    string storedProcedureName = "PME.IndividualPregnancy_Delete";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_pregnant_id", body.guid_pregnant_id);


                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Data deleted successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }

        [HttpPost("IndividualSpecialNeed_Delete")]
        public IActionResult IndividualSpecialNeed_Delete(SpecialNeed body)
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

                    string storedProcedureName = "PME.IndividualSpecialNeed_Delete";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_special_need_id", body.guid_special_need_id);


                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Data deleted successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }

        [HttpPost("IndividualContact_Delete")]
        public IActionResult IndividualContact_Delete(IndividualContact body)
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

                    string storedProcedureName = "PME.IndividualContact_Delete";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_individual_contact_id", body.guid_individual_contact_id);


                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Data deleted successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }


        [HttpPost("IndividualNationality_Delete")]
        public IActionResult IndividualNationality_Delete(IndividualNationality body)
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
                    string storedProcedureName = "PME.IndividualNationality_Delete";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_individual_nationality_id", body.guid_individual_nationality_id);

                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Data deleted successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost("IndividualMaritalStatus_Delete")]
        public IActionResult IndividualMaritalStatus_Delete(IndividualMaritalStatus body)
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
                    string storedProcedureName = "PME.IndividualMaritalStatus_Delete";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_individual_marital_status_id", body.guid_individual_marital_status_id);

                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Data deleted successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }


        [HttpPost("IndividualSpecialNeed_ChangeStatus")]
        public IActionResult IndividualSpecialNeed_ChangeStatus(SpecialNeed body)
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

                    string storedProcedureName = "PME.IndividualSpecialNeed_ChangeStatus";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_special_need_id", body.guid_special_need_id);


                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Changed status successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }

        [HttpPost("IndividualPregnancy_ChangeStatus")]
        public IActionResult IndividualPregnancy_ChangeStatus(IndividualPregnancy body)
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

                    string storedProcedureName = "PME.IndividualPregnancy_ChangeStatus";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_pregnant_id", body.guid_pregnant_id);


                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Changed status successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }


        
        
            
        [HttpPost("IndividualContact_ChangeStatus")]
        public IActionResult IndividualContact_ChangeStatus(IndividualContact body)
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

                    string storedProcedureName = "PME.IndividualContact_ChangeStatus";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_individual_contact_id", body.guid_individual_contact_id);


                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Changed status successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }



        
            
            

        [HttpPost("IndividualNationality_ChangeStatus")]
        public IActionResult IndividualNationality_ChangeStatus(IndividualNationality body)
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

                    string storedProcedureName = "PME.IndividualNationality_ChangeStatus";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_individual_nationality_id", body.guid_individual_nationality_id);


                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Changed status successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }
        
        [HttpPost("IndividualMaritalStatus_ChangeStatus")]
        public IActionResult IndividualMaritalStatus_ChangeStatus(IndividualMaritalStatus body)
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

                    string storedProcedureName = "PME.IndividualMaritalStatus_ChangeStatus";

                    var parameters = new DynamicParameters();
                    parameters.Add("@guid_individual_marital_status_id", body.guid_individual_marital_status_id);


                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                    return Ok(new { Message = "Changed status successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);

            }
        }




        [HttpGet("IndividualUpdate_GetConfigurations")]
        public IActionResult IndividualUpdate_GetConfigurations()
        {
            try
            {

                IndividualDocumentTypeController individualDocumentType = new IndividualDocumentTypeController(_configuration);
                var idt = individualDocumentType.Getlist() as OkObjectResult;

                GenderController gender= new GenderController(_configuration);
                var gen = gender.Getlist() as OkObjectResult;

                var result = new { GenderList = gen, IndividualDocumentTypeList = idt };

                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }


        [HttpGet("GetConfigurations")]
        public IActionResult GetConfigurations()
        {

            List<CombinedData> combinedData = new List<CombinedData>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                combinedData.Add(FetchDataFromView(connection, "IMS.VI_IndividualDocumentTypeList", "IndividualDocumentType"));
                combinedData.Add(FetchDataFromView(connection, "IMS.VI_GenderList", "Gender"));
                combinedData.Add(FetchDataFromView(connection, "IMS.VI_LanguageList", "Language", " order by language_name"));
                combinedData.Add(FetchDataFromView(connection, "IMS.VI_MaritalStatusList", "MaritalStatus"));
                combinedData.Add(FetchDataFromView(connection, "IMS.VI_EducationalLevelList", "EducationalLevel"));
                combinedData.Add(FetchDataFromView(connection, "IMS.VI_SpecialNeedList", "SpecialNeed"));
                combinedData.Add(FetchDataFromView(connection, "PME.VI_HouseholdStatusTypeList", "HouseholdStatusType"));
                combinedData.Add(FetchDataFromView(connection, "[IMS].[VI_HouseholdDocumentTypeList]", "HouseholdDocumentType"));
                combinedData.Add(FetchDataFromView(connection, "IMS.VI_CountryList", "Country", " order by country_name"));
                combinedData.Add(FetchDataFromView(connection, "IMS.VI_NationalityList", "Country/getNationalities", "", "order by nationality"));
                combinedData.Add(FetchDataFromView(connection, "PME.VI_RelationshipsTypeList", "RelationshipsType"));
                combinedData.Add(FetchDataFromView(connection, "PME.VI_AddressStatusTypeList", "AddressStatusType"));
                combinedData.Add(FetchDataFromView(connection, "PME.VI_AddressPropertyTypeList", "AddressPropertyType"));
                combinedData.Add(FetchDataFromView(connection, "PME.VI_DataSectionList", "DataSection/Enterable", "where is_enterable = 1"));
            }


            return Ok(combinedData);


        }

        private CombinedData FetchDataFromView(SqlConnection connection, string viewName, string tableName, string condition = "", string orderBy = "")
        {
            using (SqlCommand command = new SqlCommand($"SELECT * FROM {viewName} {condition} {orderBy}", connection))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        var rowData = new Dictionary<string, object>();
                        foreach (DataColumn column in dataSet.Tables[0].Columns)
                        {
                            rowData[column.ColumnName] = row[column];
                        }
                        data.Add(rowData);
                    }

                    // Create a CombinedData object for this view
                    return new CombinedData
                    {
                        TableName = tableName,
                        Data = data
                    };
                }
            }
        }


        [HttpPost("GetGenderByFirstName")]
        public IActionResult GetGenderByFirstName([FromBody] string first_name)
        {
            try
            {
                string connectionString = _connectionString;
                int result = 0;
                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("[PME].[GenderByFirstName]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@first_name", first_name);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result = reader.GetInt32(0);
                            }
                        }
                        return Ok(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }


        [HttpPost("CheckPersonExist")]
        public IActionResult CheckPersonExist(IndividualWizardClass body)
        {
            string result = null;
            try
            {
                sqlConnectionBuilder.MultipleActiveResultSets = false;
            using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
            {
                var sql = "Select top 1  [PME].[CheckPersonExist] ('"+ body.individual_first_name + "','"+ body.individual_last_name + "','"+ body.gender_id+ "','"+ body.date_of_birth + "','"+ body.guid_native_nationality_id + "')";

                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = reader.IsDBNull(0) ? "not exists" : reader.GetString(0);
                        }
                        return Ok(result);
                    }
                }
            }
              
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }


        [HttpPost("Individual_Household_checkDocuments")]
        public async Task<IActionResult> Individual_Household_checkDocuments([FromBody] IndividualWizardClass body)
        {
            string result1 = "not exists";
            string result2 = "not exists";

            try
            {
                string connectionString = _connectionString;

                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PME.Individual_Household_checkDocuments", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@individualDocumentType", body.individualDocumentType);
                        command.Parameters.AddWithValue("@householdDocumentType", body.householdDocumentType);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result1 = reader.GetString(0);
                                result2 = reader.GetString(1);

                            }
                        }
                        return Ok(new {result1, result2 });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }

        }




    }
}
