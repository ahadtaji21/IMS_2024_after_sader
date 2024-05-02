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
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;

namespace jrs.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Consumes("application/json")]
	public class StaffController : ControllerBase
	{
		private readonly string _connectionString;
		public Guid isPrimaryId = new Guid();
		public string action="";// for action of db like insert or upadate
		private SqlConnectionStringBuilder sqlConnectionBuilder;
		public StaffController(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("jrsdb");
			sqlConnectionBuilder = new SqlConnectionStringBuilder(_connectionString);
		}


		#region Get list data
		
		[HttpPost("GetStatistics")]
		public IActionResult GetStatistics(StaffPaginationBody body)
		{
			try
			{
				string connectionString = _connectionString;
				sqlConnectionBuilder.MultipleActiveResultSets = false;
				var genderStatistics = new List<GenderStatistics>();
				var staffStatistics_Country = new List<StaffStatistics_Country>();
				var staff_Statistics = new Staff_Statistics();
				using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
				{
					connection.Open();

					using (SqlCommand command = new SqlCommand(
						"SELECT count(gender) [genderCount],gender " +
						"FROM JRSDB.HRM.StaffView " +
						"WHERE guid_data_section_id = '" + body.guid_data_section_id + "'" +
						"GROUP BY gender"
						, connection))
					{
						command.CommandType = CommandType.Text;

						//string gender = command.ExecuteScalar().ToString();
						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{

								genderStatistics.Add(new GenderStatistics
								{
									genderCount = reader.GetInt32(0),
									//genderCount = 1,
									gender_category = reader.GetString(1)
								});

							}
						}
					}
					using (SqlCommand command = new SqlCommand("GMT.GetDonorStatistics_Country", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@office_id", body.office_id);
						//command.Parameters.AddWithValue("@guid_data_section_id", body.guid_data_section_id);

						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								staffStatistics_Country.Add(new StaffStatistics_Country
								{
									staffcountry_name = reader.GetString(0),
									staffCount = reader.GetInt32(1)
								});
							}
						}
					}

					staff_Statistics.genderStatistics = genderStatistics;
					staff_Statistics.staffStatistics_Country = staffStatistics_Country;
					return Ok(staff_Statistics);
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);
			}
		}
		[HttpPost("GetListPagination")]
		public IActionResult GetListPagination(Staff body)
		{
			try
			{
				if (body.page <= 0) body.page = 1;
				if (body.limit <= 0) body.page = 10;
				//body.guid_data_section_id = Guid.Parse("d59496bb-04b0-4eec-8259-69ea0c69af37");
				string connectionString = _connectionString;
				var resultList = new List<Staff>();
				sqlConnectionBuilder.MultipleActiveResultSets = false;
				using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
				{

					connection.Open();
					using (SqlCommand command = new SqlCommand(
						"SELECT guid_staff_id, staff_first_name, gender,staff_date_of_birth,staff_last_name, staff_code, jrs_entry_date, city_name, objective_code, created_date, Age" +
						" " +
						"FROM JRSDB.HRM.StaffView " +
						"WHERE guid_data_section_id = '" + body.guid_data_section_id + "'" +
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
								resultList.Add(new Staff
								{
									guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
									staff_code = reader["staff_code"].ToString(),
									staff_first_name = (reader["staff_first_name"].ToString()),
									staff_last_name = (reader["staff_last_name"].ToString()),
									gender = reader["gender"].ToString(),
									city_name = reader["city_name"].ToString(),
									objective_code = reader["objective_code"].ToString(),
									jrs_entry_date = (reader["jrs_entry_date"]).ToString(),
									staff_date_of_birth = (reader["staff_date_of_birth"]).ToString(),
                                    created_date = (reader["created_date"]).ToString(),
                                    Age = (reader["Age"].ToString()),
                                    /*
                                        guid_staff_id = reader.GetGuid(0),
                                        staff_code = reader.GetString(1),
                                        jrs_entry_date = reader.GetDateTime(2),
                                        data_section = reader.GetString(2),
                                        staff_first_name = reader.GetString(3),
                                        staff_last_name = reader.GetString(4),
                                        city_name = reader.GetString(18),
                                        objective_code = reader.GetString(23),
                                    */
                                });
							}
						}
						int totalCount = 0;
						using (SqlCommand cmd = new SqlCommand("SELECT count( guid_staff_id) [count] " +
																	"FROM JRSDB.HRM.StaffView " +
																	"WHERE guid_data_section_id = '" + body.guid_data_section_id + "'" 
																	, connection))
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

		[HttpPost("GetNationalityListPagination")]
		public IActionResult GetNationalityListPagination(StaffGetNationality body)
		{
			try
			{
				if (body.page <= 0) body.page = 1;
				if (body.limit <= 0) body.page = 10;
				string connectionString = _connectionString;
				var resultList = new List<Staff>();
				sqlConnectionBuilder.MultipleActiveResultSets = false;
				using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
				{

					connection.Open();
					using (SqlCommand command = new SqlCommand(
						"SELECT guid_staff_id, guid_staff_nationality_id,guid_country_id,nationality,is_active, is_native, created_date " +
						"FROM JRSDB.HRM.StaffNationality_getList " +
						"WHERE guid_staff_id = '" + body.guid_staff_id + "'" +
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
								resultList.Add(new Staff
								{
									guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
									guid_country_id = Guid.Parse(reader["guid_country_id"].ToString()),
									guid_staff_nationality_id = Guid.Parse(reader["guid_staff_nationality_id"].ToString()),
									created_date = reader["created_date"].ToString(),
									//created_date = DateTime.ParseExact(reader["created_date"].ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture),
									nationality_is_active = Convert.ToBoolean((reader["is_active"]).ToString()),
									is_native = Convert.ToBoolean((reader["is_native"]).ToString()),
									nationality = (reader["nationality"]).ToString(),
								});
							}
						}
						int totalCount = 0;
						using (SqlCommand cmd = new SqlCommand("SELECT count( guid_staff_id) [count] " +
																	"FROM JRSDB.HRM.StaffNationality_getList " +
																	"WHERE guid_staff_id = '" + body.guid_staff_id + "'"
																	, connection))
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
		
		[HttpPost("GetAddressListPagination")]
		public IActionResult GetAddressListPagination(StaffGetAddress body)
		{
			try
			{
				if (body.page <= 0) body.page = 1;
				if (body.limit <= 0) body.page = 10;
				string connectionString = _connectionString;
				var resultList = new List<StaffGetAddress>();
				sqlConnectionBuilder.MultipleActiveResultSets = false;
				using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
				{

					connection.Open();
					using (SqlCommand command = new SqlCommand(
						"SELECT guid_staff_id, guid_staff_address_id,guid_country_id,guid_country_id,guid_city_id, guid_state_id, is_active," +
						"guid_neighborhood_id,country_name,state_name,city_name,neighborhood_name, staff_address_details,created_date " +
						"FROM JRSDB.HRM.StaffAddress_getList " +
						"WHERE guid_staff_id = '" + body.guid_staff_id + "'" +
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
								resultList.Add(new StaffGetAddress
								{
									guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
									guid_staff_address_id = Guid.Parse(reader["guid_staff_address_id"].ToString()),
									guid_country_id = Guid.Parse(reader["guid_country_id"].ToString()),
									guid_state_id = Guid.Parse(reader["guid_state_id"].ToString()),
									guid_city_id = Guid.Parse(reader["guid_city_id"].ToString()),
									guid_neighborhood_id = Guid.Parse(reader["guid_neighborhood_id"].ToString()),
									created_date = reader["created_date"].ToString(),
									//created_date = DateTime.ParseExact(reader["created_date"].ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture),
									address_is_active = Convert.ToBoolean((reader["is_active"]).ToString()),
									country_name = (reader["country_name"]).ToString(),
									state_name = (reader["state_name"]).ToString(),
									city_name = (reader["city_name"]).ToString(),
									neighborhood_name = (reader["neighborhood_name"]).ToString(),
									staff_address_details = (reader["staff_address_details"]).ToString(),
								});
							}
						}
						int totalCount = 0;
						using (SqlCommand cmd = new SqlCommand("SELECT count( guid_staff_id) [count] " +
																	"FROM JRSDB.HRM.StaffAddress_getList " +
																	"WHERE guid_staff_id = '" + body.guid_staff_id + "'"
																	, connection))
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
		[HttpPost("GetMaritalListPagination")]
		public IActionResult GetMaritalListPagination(StaffGetMaritalStatus body)
		{
			try
			{
				if (body.page <= 0) body.page = 1;
				if (body.limit <= 0) body.page = 10;
				string connectionString = _connectionString;
				var resultList = new List<StaffGetMaritalStatus>();
				sqlConnectionBuilder.MultipleActiveResultSets = false;
				using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
				{

					connection.Open();
					using (SqlCommand command = new SqlCommand(
						"SELECT guid_staff_id, guid_staff_marital_status_id,guid_marital_status_id,is_active, created_date,marital_status " +
						"FROM JRSDB.HRM.StaffMaritalStatus_getList " +
						"WHERE guid_staff_id = '" + body.guid_staff_id + "'" +
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
								resultList.Add(new StaffGetMaritalStatus
								{
									guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
									guid_staff_marital_status_id = Guid.Parse(reader["guid_staff_marital_status_id"].ToString()),
									guid_marital_status_id = Guid.Parse(reader["guid_marital_status_id"].ToString()),
									created_date = reader["created_date"].ToString(),
									//created_date = DateTime.ParseExact(reader["created_date"].ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture),
									marital_is_active = Convert.ToBoolean((reader["is_active"]).ToString()),
									marital_status = (reader["marital_status"]).ToString(),
								});
							}
						}
						int totalCount = 0;
						using (SqlCommand cmd = new SqlCommand("SELECT count( guid_staff_id) [count] " +
																	"FROM JRSDB.HRM.StaffMaritalStatus_getList " +
																	"WHERE guid_staff_id = '" + body.guid_staff_id + "'"
																	, connection))
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
		[HttpPost("GetEducationListPagination")]
		public IActionResult GetEducationListPagination(StaffGetEducationalLevel body)
		{
			try
			{
				if (body.page <= 0) body.page = 1;
				if (body.limit <= 0) body.page = 10;
				string connectionString = _connectionString;
				var resultList = new List<StaffGetEducationalLevel>();
				sqlConnectionBuilder.MultipleActiveResultSets = false;
				using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
				{

					connection.Open();
					using (SqlCommand command = new SqlCommand(
						"SELECT guid_staff_id, guid_staff_educational_level_id,guid_educational_level_id,educational_level,is_active, created_date " +
						"FROM JRSDB.HRM.StaffEducationalLevel_getList " +
						"WHERE guid_staff_id = '" + body.guid_staff_id + "'" +
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
								resultList.Add(new StaffGetEducationalLevel
								{
									guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
									guid_staff_educational_level_id = Guid.Parse(reader["guid_staff_educational_level_id"].ToString()),
									guid_educational_level_id = Guid.Parse(reader["guid_educational_level_id"].ToString()),
									created_date = reader["created_date"].ToString(),
									//created_date = DateTime.ParseExact(reader["created_date"].ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture),
									education_is_active = Convert.ToBoolean((reader["is_active"]).ToString()),
									educational_level = (reader["educational_level"]).ToString(),
								});
							}
						}
						int totalCount = 0;
						using (SqlCommand cmd = new SqlCommand("SELECT count( guid_staff_id) [count] " +
																	"FROM JRSDB.HRM.StaffEducationalLevel_getList " +
																	"WHERE guid_staff_id = '" + body.guid_staff_id + "'"
																	, connection))
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
		[HttpPost("GetDocumentListPagination")]
		public IActionResult GetDocumentListPagination(StaffGetDocument body)
		{
			try
			{
				if (body.page <= 0) body.page = 1;
				if (body.limit <= 0) body.page = 10;
				string connectionString = _connectionString;
				var resultList = new List<StaffGetDocument>();
				sqlConnectionBuilder.MultipleActiveResultSets = false;
				using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
				{

					connection.Open();
					using (SqlCommand command = new SqlCommand(
						"SELECT guid_staff_id, guid_staff_document_id,guid_document_type_id,document_type,is_active, " +
						"created_date,document_notes,document_number,start_date,end_date " +
						"FROM JRSDB.HRM.StaffDocument_getList " +
						"WHERE guid_staff_id = '" + body.guid_staff_id + "'" +
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
								resultList.Add(new StaffGetDocument
								{
									guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
									guid_staff_document_id = Guid.Parse(reader["guid_staff_document_id"].ToString()),
									guid_document_type_id = Guid.Parse(reader["guid_document_type_id"].ToString()),
									created_date = reader["created_date"].ToString(),
									//created_date = DateTime.ParseExact(reader["created_date"].ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture),
									document_is_active = Convert.ToBoolean((reader["is_active"]).ToString()),
									document_type = (reader["document_type"]).ToString(),
									document_notes = (reader["document_notes"]).ToString(),
									document_number = (reader["document_number"]).ToString(),
									document_start_date = (reader["start_date"]).ToString(),
									document_end_date = (reader["end_date"]).ToString(),
								});
							}
						}
						int totalCount = 0;
						using (SqlCommand cmd = new SqlCommand("SELECT count( guid_staff_id) [count] " +
																	"FROM JRSDB.HRM.StaffDocument_getList " +
																	"WHERE guid_staff_id = '" + body.guid_staff_id + "'"
																	, connection))
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
		[HttpPost("GetDataSectionListPagination")]
		public IActionResult GetDataSectionListPagination(StaffGetDataSection body)
		{
			try
			{
				if (body.page <= 0) body.page = 1;
				if (body.limit <= 0) body.page = 10;
				string connectionString = _connectionString;
				var resultList = new List<StaffGetDataSection>();
				sqlConnectionBuilder.MultipleActiveResultSets = false;
				using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
				{

					connection.Open();
					using (SqlCommand command = new SqlCommand(
						"SELECT guid_staff_id, guid_staff_datasection_id,guid_data_section_id,data_section,is_active, objective_code, " +
						"created_date,start_date,end_date " +
						"FROM JRSDB.HRM.StaffDataSection_getList " +
						"WHERE guid_staff_id = '" + body.guid_staff_id + "'" +
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
								resultList.Add(new StaffGetDataSection
								{
									guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
									guid_staff_datasection_id = Guid.Parse(reader["guid_staff_datasection_id"].ToString()),
									guid_data_section_id = Guid.Parse(reader["guid_data_section_id"].ToString()),
									created_date = reader["created_date"].ToString(),
									//created_date = DateTime.ParseExact(reader["created_date"].ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture),
									datasection_is_active = Convert.ToBoolean((reader["is_active"]).ToString()),
									data_section = (reader["data_section"]).ToString(),
									objective_code = (reader["objective_code"]).ToString(),
									datasection_start_date = (reader["start_date"]).ToString(),
									datasection_end_date = (reader["end_date"]).ToString(),
								});
							}
						}
						int totalCount = 0;
						using (SqlCommand cmd = new SqlCommand("SELECT count( guid_staff_id) [count] " +
																	"FROM JRSDB.HRM.StaffDataSection_getList " +
																	"WHERE guid_staff_id = '" + body.guid_staff_id + "'"
																	, connection))
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
		[HttpPost("GetContactListPagination")]
		public IActionResult GetContactListPagination(StaffGetContactType body)
		{
			try
			{
				if (body.page <= 0) body.page = 1;
				if (body.limit <= 0) body.page = 10;
				string connectionString = _connectionString;
				var resultList = new List<StaffGetContactType>();
				sqlConnectionBuilder.MultipleActiveResultSets = false;
				using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
				{

					connection.Open();
					using (SqlCommand command = new SqlCommand(
						"SELECT guid_staff_id, guid_staff_contact_id,guid_contact_type_id,contact_type,is_active, staff_contact_values, created_date " +
						"FROM JRSDB.HRM.StaffContact_getList " +
						"WHERE guid_staff_id = '" + body.guid_staff_id + "'" +
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
								resultList.Add(new StaffGetContactType
								{
									guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
									guid_staff_contact_id = Guid.Parse(reader["guid_staff_contact_id"].ToString()),
									guid_contact_type_id = Guid.Parse(reader["guid_contact_type_id"].ToString()),
									created_date = reader["created_date"].ToString(),
									//created_date = DateTime.ParseExact(reader["created_date"].ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture),
									contact_is_active = Convert.ToBoolean((reader["is_active"]).ToString()),
									staff_contact_values = (reader["staff_contact_values"]).ToString(),
									contact_type = (reader["contact_type"]).ToString(),
								});
							}
						}
						int totalCount = 0;
						using (SqlCommand cmd = new SqlCommand("SELECT count( guid_staff_id) [count] " +
																	"FROM JRSDB.HRM.StaffContact_getList " +
																	"WHERE guid_staff_id = '" + body.guid_staff_id + "'"
																	, connection))
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
		[HttpPost("GetStaffListPagination")]
		public async Task<IActionResult> GetStaffListPagination(StaffPaginationBody body)
		{
			try
			{
			
				string connectionString = _connectionString;
				sqlConnectionBuilder.MultipleActiveResultSets = false;
				using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
				{

					connection.Open();
					using (SqlCommand command = new SqlCommand(
						"SELECT guid_staff_id, staff_first_name,staff_last_name,staff_first_name_native,staff_last_name_native, staff_father_name," +
						"staff_mother_name, jrs_entry_date,staff_date_of_birth,is_religious,staff_place_of_birth,staff_code,is_active,guid_native_language_id," +
						"staff_gender_id,blood_type_id FROM JRSDB.HRM.Staff_getList WHERE guid_staff_id = '" + body.guid_staff_id + "'", connection))
					{
						command.CommandType = CommandType.Text;
						StaffGetData vm = new();
						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (await reader.ReadAsync())
							{
								#region Personal Information
								vm.guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString());
								vm.staff_first_name = reader["staff_first_name"].ToString();
								vm.staff_last_name = reader["staff_last_name"].ToString();
								vm.staff_first_name_native = reader["staff_first_name_native"].ToString();
								vm.staff_last_name_native = reader["staff_last_name_native"].ToString();
								vm.staff_father_name = reader["staff_father_name"].ToString();
								vm.staff_mother_name = reader["staff_mother_name"].ToString();
								vm.jrs_entry_date = (reader["jrs_entry_date"]).ToString();
								//vm.jrs_entry_date = DateTime.ParseExact(reader["jrs_entry_date"].ToString(), "MM/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
								vm.staff_date_of_birth = reader["staff_date_of_birth"].ToString();
								vm.is_religious = Convert.ToBoolean((reader["is_religious"]));
								vm.staff_place_of_birth = reader["staff_place_of_birth"].ToString();
								vm.staff_code = reader["staff_code"].ToString();
								vm.staff_is_active = Convert.ToBoolean(reader["is_active"].ToString());
								vm.guid_native_language_id = Guid.Parse(reader["guid_native_language_id"].ToString());
								vm.staff_gender_id = Convert.ToInt32((reader["staff_gender_id"]).ToString());
								vm.blood_type_id = Convert.ToInt32((reader["blood_type_id"]).ToString());
								#endregion
							}
						}
						return Ok(vm);
					}
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);
			}
		}
		[HttpPost("GetListSearch")]
		public IActionResult GetListSearch(StaffPaginationBody body)
		{
			try
			{
				string where_phrase = body.searchBody.Length > 0 ? "where " : "";
				for (var i = 0; i < body.searchBody.Length; i++)
				{
					where_phrase += body.searchBody[i].search_key + " LIKE '%" + body.searchBody[i].search_query + "%'";
					if (i + 1 < body.searchBody.Length)
						where_phrase += " and ";
				}
				// Adding the new WHERE clause
				if (!string.IsNullOrEmpty(where_phrase))
					where_phrase += " AND ";
				where_phrase += "guid_data_section_id = '" + body.guid_data_section_id + "'";
				string connectionString = _connectionString;
				var resultList = new List<Staff>();
				sqlConnectionBuilder.MultipleActiveResultSets = false;
				using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
				{
					connection.Open();
					using (SqlCommand command = new SqlCommand(
						"SELECT guid_staff_id, staff_first_name, staff_last_name, staff_code, jrs_entry_date, " +
						"city_name, gender, staff_date_of_birth, Age, created_date, objective_code " +
						"FROM JRSDB.HRM.STaffView " +
						where_phrase +
						" ORDER BY created_date desc ", connection))
					{
						command.CommandType = CommandType.Text;
						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								resultList.Add(new Staff
								{
                                    guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
                                    staff_code = reader["staff_code"].ToString(),
                                    staff_first_name = (reader["staff_first_name"].ToString()),
                                    staff_last_name = (reader["staff_last_name"].ToString()),
                                    gender = reader["gender"].ToString(),
                                    city_name = reader["city_name"].ToString(),
                                    objective_code = reader["objective_code"].ToString(),
                                    jrs_entry_date = (reader["jrs_entry_date"]).ToString(),
                                    staff_date_of_birth = (reader["staff_date_of_birth"]).ToString(),
                                    created_date = (reader["created_date"]).ToString(),
                                    Age = (reader["Age"].ToString()),

         //                           guid_staff_id = reader.GetGuid(0),
									//staff_first_name = reader.GetString(1),
									//staff_last_name = reader.GetString(2),
									//staff_code = reader.GetString(3),
									////jrs_entry_date = (string?)reader.GetString(4),
									//city_name = reader.GetString(5),
									//objective_code = reader.GetString(6),
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
		[HttpPost("GetStaffByGuid")]
		public async Task<IActionResult> GetStaffByGuid(StaffPaginationBody body)
		{
			try
			{
				#region New Implementation by Ahad

				if (body.page <= 0) body.page = 1;
				if (body.limit <= 0) body.page = 10;
				using (SqlConnection sql = new SqlConnection(_connectionString))
				{
					using (SqlCommand cmd = new SqlCommand("HRM.Staff_edit_get", sql))
					{
						cmd.CommandType = System.Data.CommandType.StoredProcedure;
						cmd.Parameters.Add(new SqlParameter("@guid_staff_id", body.guid_staff_id));
						StaffGetData vm = new();
						await sql.OpenAsync();
						cmd.CommandTimeout = 500;
						using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
						{
							try
							{
								while (await reader.ReadAsync())
								{
									#region Personal Information
									vm.guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString());
									vm.staff_first_name = reader["staff_first_name"].ToString();
									vm.staff_last_name = reader["staff_last_name"].ToString();
									vm.staff_first_name_native = reader["staff_first_name_native"].ToString();
									vm.staff_last_name_native = reader["staff_last_name_native"].ToString();
									vm.staff_father_name = reader["staff_father_name"].ToString();
									vm.staff_mother_name = reader["staff_mother_name"].ToString();
									vm.jrs_entry_date = (reader["jrs_entry_date"]).ToString();
									//vm.jrs_entry_date = DateTime.ParseExact(reader["jrs_entry_date"].ToString(), "MM/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
									
									vm.staff_date_of_birth = reader["staff_date_of_birth"].ToString();
									vm.is_religious = Convert.ToBoolean((reader["is_religious"]));
									vm.staff_place_of_birth = reader["staff_place_of_birth"].ToString();
									vm.staff_code = reader["staff_code"].ToString();
									vm.staff_is_active = Convert.ToBoolean(reader["is_active"].ToString());
									vm.guid_native_language_id = Guid.Parse(reader["guid_native_language_id"].ToString());
									vm.staff_gender_id = Convert.ToInt32((reader["staff_gender_id"]).ToString());
									vm.blood_type_id = Convert.ToInt32((reader["blood_type_id"]).ToString());
                                    #endregion
                                }
								reader.NextResult();
								var staffnationality = new List<StaffGetNationality>();
								while (await reader.ReadAsync())
								{
									var data = new StaffGetNationality()
									{
										guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
										guid_staff_nationality_id = Guid.Parse(reader["guid_staff_nationality_id"].ToString()),
										guid_country_id = Guid.Parse(reader["guid_country_id"].ToString()),
										nationality_is_active = Convert.ToBoolean(reader["is_active"].ToString()),
										is_native = Convert.ToBoolean(reader["is_native"].ToString()),
										nationality = reader["nationality"].ToString(),
										created_date = (reader["created_date"]).ToString()
									};
									staffnationality.Add(data);
								}
								vm.StaffGetNationality = staffnationality;
								reader.NextResult();
								var staffContactType = new List<StaffGetContactType>();
								while (await reader.ReadAsync())
								{
									var data = new StaffGetContactType()
									{
										guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
										guid_staff_contact_id = Guid.Parse(reader["guid_staff_contact_id"].ToString()),
										guid_contact_type_id = Guid.Parse(reader["guid_contact_type_id"].ToString()),
										contact_is_active = Convert.ToBoolean(reader["is_active"].ToString()),
										//staff_contact_values = reader["staff_contact_values"].ToString(),
										contact_type = reader["contact_type"].ToString(),
										created_date = (reader["created_date"]).ToString(),
									};
									staffContactType.Add(data);
								}
								vm.StaffGetContactType = staffContactType;
								reader.NextResult();
								var staffMaritalStatus = new List<StaffGetMaritalStatus>();
								while (await reader.ReadAsync())
								{
									var data = new StaffGetMaritalStatus()
									{
										guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
										guid_marital_status_id = Guid.Parse(reader["guid_marital_status_id"].ToString()),
										marital_is_active = Convert.ToBoolean(reader["is_active"].ToString()),
										marital_status = reader["marital_status"].ToString(),
										created_date = (reader["created_date"]).ToString()
									};
									staffMaritalStatus.Add(data);
								}
								vm.StaffGetMaritalStatus = staffMaritalStatus;
								reader.NextResult();
								var staffAddress = new List<StaffGetAddress>();
								while (await reader.ReadAsync())
								{
									var data = new StaffGetAddress()
									{
										guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
										guid_staff_address_id = Guid.Parse(reader["guid_staff_address_id"].ToString()),
										guid_city_id = Guid.Parse(reader["guid_city_id"].ToString()),
										//guid_state_id = Guid.Parse(reader["guid_state_id"].ToString()),
										//guid_country_id = Guid.Parse(reader["guid_country_id"].ToString()),
										//guid_neighborhood_id = Guid.Parse(reader["guid_neighborhood_id"].ToString()),
										//is_active = Convert.ToBoolean(reader["is_active"].ToString()),
										city_name = reader["city_name"].ToString(),
										state_name = reader["state_name"].ToString(),
										country_name = reader["country_name"].ToString(),
										neighborhood_name = reader["neighborhood_name"].ToString(),
										created_date = (reader["created_date"]).ToString()
									};
									staffAddress.Add(data);
								}
								vm.StaffGetAddress = staffAddress;
								reader.NextResult();
								var staffEducationalLevel = new List<StaffGetEducationalLevel>();
								while (await reader.ReadAsync())
								{
									var data = new StaffGetEducationalLevel()
									{
										guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
										guid_staff_educational_level_id = Guid.Parse(reader["guid_staff_educational_level_id"].ToString()),
										guid_educational_level_id = Guid.Parse(reader["guid_educational_level_id"].ToString()),
										education_is_active = Convert.ToBoolean(reader["is_active"].ToString()),
										educational_level = reader["educational_level"].ToString(),
										created_date = (reader["created_date"]).ToString()
									};
									staffEducationalLevel.Add(data);
								}
								vm.StaffGetEducationalLevel = staffEducationalLevel;
								reader.NextResult();
								var staffDocument = new List<StaffGetDocument>();
								while (await reader.ReadAsync())
								{
									var data = new StaffGetDocument()
									{
										guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
										guid_staff_document_id = Guid.Parse(reader["guid_staff_document_id"].ToString()),
										guid_document_type_id = Guid.Parse(reader["guid_document_type_id"].ToString()),
										document_is_active = Convert.ToBoolean(reader["is_active"].ToString()),
										document_type = reader["document_type"].ToString(),
										document_file = reader["document_file"].ToString(),
										document_notes = reader["document_notes"].ToString(),
										document_number = reader["document_number"].ToString(),
										document_start_date = reader["start_date"].ToString(),
										document_end_date = reader["end_date"].ToString(),
										created_date = (reader["created_date"]).ToString()
									};
									staffDocument.Add(data);
								}
								vm.StaffGetDocument = staffDocument;
								reader.NextResult();
								var staffDataSection = new List<StaffGetDataSection>();
								while (await reader.ReadAsync())
								{
									var data = new StaffGetDataSection()
									{
										guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
										guid_staff_datasection_id = Guid.Parse(reader["guid_staff_datasection_id"].ToString()),
										guid_data_section_id = Guid.Parse(reader["guid_data_section_id"].ToString()),
										datasection_is_active = Convert.ToBoolean(reader["is_active"].ToString()),
										data_section = reader["data_section"].ToString(),
										objective_code = reader["objective_code"].ToString(),
										datasection_start_date = reader["start_date"].ToString(),
										datasection_end_date = reader["end_date"].ToString(),
										created_date = (reader["created_date"]).ToString()
									};
									staffDataSection.Add(data);
								}
								vm.StaffGetDataSection = staffDataSection;
								reader.NextResult();
							}
							catch (Exception ex)
							{
								System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
								System.Diagnostics.StackFrame sf = new System.Diagnostics.StackFrame();
								string LogString = "Message: " + ex.Message + " StackFrame: " + sf.GetMethod().Name + " FullName: " + ex.GetType().FullName + " MethodName: " + st.GetFrame(1).GetMethod().Name + " StackTrace: " + ex.StackTrace;
								Console.Write(LogString);
								throw ex;
							}
						}
						return Ok(vm);
					}
				}
				#endregion
				#region old implementation
				/*
				string connectionString = _connectionString;
				//var resultList = new List<Staff>();
				Staff resultList = new Staff();
				sqlConnectionBuilder.MultipleActiveResultSets = false;
				using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
				{
					connection.Open();
					using (SqlCommand command = new SqlCommand(
						"SELECT * FROM JRSDB.HRM.STaffView  WHERE guid_staff_id = '" + body.guid_staff_id + "'", connection))
					{
						command.CommandType = CommandType.Text;
						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
										resultList.guid_staff_id           = reader.GetGuid(0);
										resultList.staff_code				= reader.IsDBNull(1) ? null : reader.GetString(1);
										resultList.jrs_entry_date			= reader.IsDBNull(2) ? null : reader.GetString(2);
										resultList.staff_first_name			= reader.IsDBNull(3) ? null : reader.GetString(3);
										resultList.staff_last_name			= reader.IsDBNull(4) ? null : reader.GetString(4);
										resultList.staff_first_name_native	= reader.IsDBNull(5) ? null : reader.GetString(5);
										resultList.staff_last_name_native	= reader.IsDBNull(6) ? null : reader.GetString(6);
										resultList.guid_native_language_id	= reader.IsDBNull(7) ? null : reader.GetGuid(7);
										resultList.staff_father_name		= reader.IsDBNull(8) ? null : reader.GetString(8);
										resultList.staff_mother_name		= reader.IsDBNull(9) ? null : reader.GetString(9);
										resultList.staff_date_of_birth		= reader.IsDBNull(10) ? null : reader.GetDateTime(10);
										resultList.staff_place_of_birth	= reader.IsDBNull(11) ? null : reader.GetString(11);
										resultList.staff_gender_id				= reader.IsDBNull(12) ? 0 : reader.GetInt32(12);
										resultList.is_religious =			reader.GetBoolean(13);
										//staff_photo				= reader.GetByte(14);
										resultList.blood_type_id			= reader.IsDBNull(15) ? null : reader.GetInt32(15);
										resultList.guid_created_by			= reader.IsDBNull(16) ? null : reader.GetGuid(16);
										resultList.created_date			= reader.IsDBNull(17) ? null : reader.GetDateTime(17);
										resultList.guid_updated_by			= reader.IsDBNull(18) ? null : reader.GetGuid(18);
										resultList.updated_date			= reader.IsDBNull(19) ? null : reader.GetDateTime(19);
										resultList.is_active   =			reader.GetBoolean(20) ;
											
							}
						}
						return Ok(resultList);
					}
				}
				*/


				#endregion
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);
			}
		}
		#endregion

		#region staff update
		[HttpPost("Staff_Update")]
		public IActionResult Staff_Update(Staff body)
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
					string storedProcedureName = "HRM.Staff_update";
					var parameters = new DynamicParameters();
					parameters.Add("@guid_staff_id", body.guid_staff_id);
					parameters.Add("@jrs_entry_date", body.jrs_entry_date);
					parameters.Add("@staff_first_name", body.staff_first_name);
					parameters.Add("@staff_last_name", body.staff_last_name);
					parameters.Add("@staff_first_name_native", body.staff_first_name_native);
					parameters.Add("@staff_last_name_native", body.staff_last_name_native);
					parameters.Add("@guid_native_language_id", body.guid_native_language_id);
					parameters.Add("@staff_father_name", body.staff_father_name);
					parameters.Add("@staff_mother_name", body.staff_mother_name);
					parameters.Add("@staff_date_of_birth", body.staff_date_of_birth);
					parameters.Add("@staff_place_of_birth", body.staff_place_of_birth);
					parameters.Add("@staff_gender_id", body.staff_gender_id);
					parameters.Add("@is_religious", body.is_religious);
                    //parameters.Add("@staff_photo", body.staff_photo);
                    parameters.Add("@blood_type_id", body.blood_type_id);
					parameters.Add("@guid_updated_by", body.userId);
					parameters.Add("@updated_date", DateTime.Now);
					parameters.Add("@is_active", body.staff_is_active);
					//parameters.Add("@staff_code", body.staff_code);
					connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
					return Ok(new { Message = "Data saved successfully." });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);
			}
		}
		#endregion

		#region Staff InsertUpdate Section
		[HttpPost("StaffNationality_insert")]
		public IActionResult StaffNationality_insert_update_delete(StaffGetNationality body)
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
                    if (body.IsActiveTrue == 1)
                    {
                        string storedProcedureName = "HRM.Update_Active_ChangeStatus";
                        var parameters = new DynamicParameters();
                        parameters.Add("@tableSchema", "HRM");
                        parameters.Add("@tableName", "StaffNationality");
                        parameters.Add("@guid_staff_child_primary_id_name", "guid_staff_nationality_id");
                        //parameters.Add("@guid_staff_id", body.guid_staff_id);
                        parameters.Add("@guid_staff_child_primary_id", body.guid_staff_nationality_id);
                        parameters.Add("@updated_date", DateTime.Now);
                        parameters.Add("@guid_updated_by", body.userId);
                        parameters.Add("@is_active", body.nationality_is_active);
                        connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                        return Ok(new { Message = "Status changed successfully." });
                    }
					else //Validation
					{
                        // Check if the data already exists
                        using (SqlCommand checkCommand = new SqlCommand(
                            "SELECT COUNT(*) FROM HRM.StaffNationality " +
							"WHERE guid_staff_id = @guid_staff_id " +
							"AND guid_country_id = @guid_country_id",
                            connection))
                        {
                            checkCommand.Parameters.AddWithValue("@guid_staff_id", body.guid_staff_id);
                            checkCommand.Parameters.AddWithValue("@guid_country_id", body.guid_country_id);

                            int existingCount = (int)checkCommand.ExecuteScalar();
                            if (existingCount > 0)
                            {
                                // Data already exists, return a message
                                return BadRequest( "Data already exists");
                            }
                        }
                        // End - Check if the data already exists

                        //--- Check if primary id is null then insert else update
                        if (body.guid_staff_nationality_id == isPrimaryId || body.guid_staff_nationality_id == null)
                        {
                            action = "insert";
                        }
                        else
                        {
                            action = "update";
                        }
                        // Continue with the insertion/updation if data doesn't exist
                        string storedProcedureName = "HRM.StaffNationality_insert_update_delete";
                        var parameters = new DynamicParameters();
                        parameters.Add("@@guid_staff_nationality_id", body.guid_staff_nationality_id);
                        parameters.Add("@action_perform", action);
                        parameters.Add("@guid_staff_id", body.guid_staff_id);
                        parameters.Add("@guid_country_id", body.guid_country_id);
                        parameters.Add("@is_native", body.is_native);
                        parameters.Add("@created_date", DateTime.Now);
                        parameters.Add("@guid_created_by", body.userId);
                        parameters.Add("@updated_date", DateTime.Now);
                        parameters.Add("@guid_updated_by", body.userId);
                        parameters.Add("@is_active", body.nationality_is_active);
                        connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                        return Ok(new { Message = "Data saved successfully." });
                    }
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);

			}
		}
		
		[HttpPost("StaffMaritalStatus_insert")]
		public IActionResult StaffMaritalStatus_insert(StaffGetMaritalStatus body)
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
                    if (body.IsActiveTrue == 1)
                    {
                        string storedProcedureName = "HRM.Update_Active_ChangeStatus";
                        var parameters = new DynamicParameters();
                        parameters.Add("@tableSchema", "HRM");
                        parameters.Add("@tableName", "StaffMaritalStatus");
                        parameters.Add("@guid_staff_child_primary_id_name", "guid_staff_marital_status_id");
                        //parameters.Add("@guid_staff_id", body.guid_staff_id);
                        parameters.Add("@guid_staff_child_primary_id", body.guid_staff_marital_status_id);
                        parameters.Add("@updated_date", DateTime.Now);
                        parameters.Add("@guid_updated_by", body.userId);
                        parameters.Add("@is_active", body.marital_is_active);
                        connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                        return Ok(new { Message = "Status changed successfully." });

                    }
                    
					
					else //Validation
					{
                        // Check if the data already exists
                        using (SqlCommand checkCommand = new SqlCommand(
                            "SELECT COUNT(*) FROM HRM.StaffMaritalStatus " +
							"WHERE guid_staff_id = @guid_staff_id " +
							"AND guid_marital_status_id = @guid_marital_status_id",
                            connection))
                        {
                            checkCommand.Parameters.AddWithValue("@guid_staff_id", body.guid_staff_id);
                            checkCommand.Parameters.AddWithValue("@guid_marital_status_id", body.guid_marital_status_id);
                            int existingCount = (int)checkCommand.ExecuteScalar();
                            if (existingCount > 0)
                            {
                                // Data already exists, return a message
                                return BadRequest(new { Message = "Data already exists" });
                            }
                        }
                        // Check if the data already exists
                        if (body.guid_staff_marital_status_id == isPrimaryId || body.guid_staff_marital_status_id == null)
                        {
                            action = "insert";
                        }
                        else
                        {
                            action = "update";
                        }
                        // Continue with the insertion/updation if data doesn't exist
                        string storedProcedureName = "HRM.StaffMaritalStatus_insert_update_delete";
                        var parameters = new DynamicParameters();
                        parameters.Add("@guid_staff_marital_status_id", body.guid_staff_marital_status_id);
                        parameters.Add("@action_perform", action);
                        parameters.Add("@guid_staff_id", body.guid_staff_id);
                        parameters.Add("@guid_marital_status_id", body.guid_marital_status_id);
                        parameters.Add("@created_date", DateTime.Now);
                        parameters.Add("@guid_created_by", body.userId);
                        parameters.Add("@updated_date", DateTime.Now);
                        parameters.Add("@guid_updated_by", body.userId);
                        parameters.Add("@is_active", body.marital_is_active);
                        connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                        return Ok(new { Message = "Data saved successfully." });
                    }
					
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);

			}
		}
		
		[HttpPost("StaffEducationalLevel_insert")]
		public IActionResult StaffEducationalLevel_insert(StaffGetEducationalLevel body)
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
					if (body.IsActiveTrue == 1)
					{
						string storedProcedureName = "HRM.Update_Active_ChangeStatus";
						var parameters = new DynamicParameters();
						parameters.Add("@tableSchema", "HRM");
						parameters.Add("@tableName", "StaffEducationalLevel");
						parameters.Add("@guid_staff_child_primary_id_name", "guid_staff_educational_level_id");
						parameters.Add("@guid_staff_child_primary_id", body.guid_staff_educational_level_id);
						parameters.Add("@updated_date", DateTime.Now);
						parameters.Add("@guid_updated_by", body.userId);
						parameters.Add("@is_active", body.education_is_active);
						connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
						return Ok(new { Message = "Status changed successfully." });
					}
					else //Validation
					{
						// Check if the data already exists
						using (SqlCommand checkCommand = new SqlCommand(
                        "SELECT COUNT(*) FROM HRM.StaffEducationalLevel " +
						"WHERE guid_staff_id = @guid_staff_id " +
						"AND guid_educational_level_id = @guid_educational_level_id ",
						connection))
						//"WHERE guid_staff_id = '" + body.guid_staff_id + "' AND guid_marital_status_id = '" + body.guid_marital_status_id + "'" , connection))

						{
							checkCommand.Parameters.AddWithValue("@guid_staff_id", body.guid_staff_id);
							checkCommand.Parameters.AddWithValue("@guid_educational_level_id", body.guid_educational_level_id);

							int existingCount = (int)checkCommand.ExecuteScalar();
							if (existingCount > 0)
							{
								// Data already exists, return a message
								return BadRequest("Data already exists" );
							}
						}
						// End - Check if the data already exists

						//--- Check if primary id is null then insert else update
						if (body.guid_staff_educational_level_id == isPrimaryId || body.guid_staff_educational_level_id == null)
						{
							action = "insert";
						}
						else
						{
							action = "update";
						}
						// Continue with the insertion/updation if data doesn't exist

						string storedProcedureName = "HRM.StaffEducationalLevel_insert_update_delete";
						var parameters = new DynamicParameters();
						parameters.Add("@guid_staff_educational_level_id", body.guid_staff_educational_level_id);
						parameters.Add("@action_perform", action);
						parameters.Add("@guid_staff_id", body.guid_staff_id);
						parameters.Add("@guid_educational_level_id", body.guid_educational_level_id);
						parameters.Add("@created_date", DateTime.Now);
						parameters.Add("@guid_created_by", body.userId);
						parameters.Add("@updated_date", DateTime.Now);
						parameters.Add("@guid_updated_by", body.userId);
						parameters.Add("@is_active", body.education_is_active);

						connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
						return Ok(new { Message = "Data saved successfully." });
					}
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);

			}
		}
		
		[HttpPost("StaffContact_insert")]
		public IActionResult StaffContact_insert(StaffGetContactType body)
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
					if (body.IsActiveTrue == 1)
					{
						string storedProcedureName = "HRM.Update_Active_ChangeStatus";
						var parameters = new DynamicParameters();
						parameters.Add("@tableSchema", "HRM");
						parameters.Add("@tableName", "StaffContact");
						parameters.Add("@guid_staff_child_primary_id_name", "guid_staff_contact_id");
						parameters.Add("@guid_staff_child_primary_id", body.guid_staff_contact_id);
						parameters.Add("@updated_date", DateTime.Now);
						parameters.Add("@guid_updated_by", body.userId);
						parameters.Add("@is_active", body.contact_is_active);
						connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
						return Ok(new { Message = "Status changed successfully." });

					}
					else // Validation
					{
						// Check if the data already exists
						using (SqlCommand checkCommand = new SqlCommand(
                        "SELECT COUNT(*) FROM HRM.StaffContact " +
						"WHERE guid_staff_id = @guid_staff_id " +
						"AND guid_contact_type_id = @guid_contact_type_id " +
						"AND staff_contact_values = @staff_contact_values ",
						connection))
						{
							checkCommand.Parameters.AddWithValue("@guid_staff_id", body.guid_staff_id);
							checkCommand.Parameters.AddWithValue("@guid_contact_type_id", body.guid_contact_type_id);
							checkCommand.Parameters.AddWithValue("@staff_contact_values", body.staff_contact_values);

							int existingCount = (int)checkCommand.ExecuteScalar();
							if (existingCount > 0)
							{
								// Data already exists, return a message
								return BadRequest( "Data already exists");
							}
						}
						// End - Check if the data already exists

						//--- Check if primary id is null then insert else update
						if (body.guid_staff_contact_id == isPrimaryId || body.guid_staff_contact_id == null)
						{
							action = "insert";
						}
						else
						{
							action = "update";
						}

						// Continue with the insertion/updation if data doesn't exist
						string storedProcedureName = "HRM.StaffContact_insert_update_delete";
						var parameters = new DynamicParameters();
						parameters.Add("@guid_staff_contact_id", body.guid_staff_contact_id);
						parameters.Add("@action_perform", action);
						parameters.Add("@guid_staff_id", body.guid_staff_id);
						parameters.Add("@guid_contact_type_id", body.guid_contact_type_id);
						parameters.Add("@created_date", DateTime.Now);
						parameters.Add("@guid_created_by", body.userId);
						parameters.Add("@updated_date", DateTime.Now);
						parameters.Add("@guid_updated_by", body.userId);
						parameters.Add("@staff_contact_values", body.staff_contact_values);
						parameters.Add("@is_active", body.contact_is_active);

						connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
						return Ok(new { Message = "Data saved successfully." });
					}
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);

			}
		}

		[HttpPost("StaffDocument_insert")]
		public IActionResult StaffDocument_insert(StaffGetDocument body)
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
					if (body.IsActiveTrue == 1)
					{
						string storedProcedureName = "HRM.Update_Active_ChangeStatus";
						var parameters = new DynamicParameters();
						parameters.Add("@tableSchema", "HRM");
						parameters.Add("@tableName", "StaffDocument");
						parameters.Add("@guid_staff_child_primary_id_name", "guid_staff_document_id");
						parameters.Add("@guid_staff_child_primary_id", body.guid_staff_document_id);
						parameters.Add("@updated_date", DateTime.Now);
						parameters.Add("@guid_updated_by", body.userId);
						parameters.Add("@is_active", body.document_is_active);
						connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
						return Ok(new { Message = "Status changed successfully." });
					}
                    else //Validation
                    {
                        // Check if the data already exists
                        using (SqlCommand checkCommand = new SqlCommand(
                            "SELECT COUNT(*) FROM HRM.StaffDocument " +
							"WHERE guid_staff_id = @guid_staff_id " +
                            "AND guid_document_type_id = @guid_document_type_id " +
							"AND document_number = @document_number " +
							"AND start_date = @start_date " +
							"AND end_date = @end_date " +
							"AND is_active = @is_active ",
                            connection))
                        {
                            checkCommand.Parameters.AddWithValue("@guid_staff_id", body.guid_staff_id);
                            checkCommand.Parameters.AddWithValue("@guid_document_type_id", body.guid_document_type_id);
                            checkCommand.Parameters.AddWithValue("@document_number", body.document_number);
                            checkCommand.Parameters.AddWithValue("@start_date", body.document_start_date);
                            checkCommand.Parameters.AddWithValue("@end_date", body.document_end_date);
                            checkCommand.Parameters.AddWithValue("@is_active", true);
                            int existingCount = (int)checkCommand.ExecuteScalar();
                            if (existingCount > 0)
                            {
                                // Data already exists, return a message
                                return BadRequest( "Data already exists" );
                            }
                        }
                        // Check if the data already exists
                        if (body.guid_staff_document_id == isPrimaryId || body.guid_staff_document_id == null)
                        {
                            action = "insert";
                        }
                        else
                        {
                            action = "update";
                        }

                        string storedProcedureName = "HRM.StaffDocument_insert_update_delete";
						var parameters = new DynamicParameters();
						parameters.Add("@guid_staff_document_id", body.guid_staff_document_id);
						parameters.Add("@action_perform", action);
						parameters.Add("@guid_staff_id", body.guid_staff_id);
						parameters.Add("@guid_document_type_id", body.guid_document_type_id);
						parameters.Add("@document_number", body.document_number);
						parameters.Add("@start_date", body.document_start_date);
						parameters.Add("@end_date", body.document_end_date);
						parameters.Add("@document_notes", body.document_notes);
						parameters.Add("@document_file", body.document_file);
						parameters.Add("@created_date", DateTime.Now);
						parameters.Add("@guid_created_by", body.userId);
						parameters.Add("@updated_date", DateTime.Now);
						parameters.Add("@guid_updated_by", body.userId);
						parameters.Add("@is_active", body.document_is_active);

						connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
						return Ok(new { Message = "Data saved successfully." });
					}
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);

			}
		}

		[HttpPost("StaffAddress_insert")]
		public IActionResult StaffAddress_insert(StaffGetAddress body)
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
					if (body.IsActiveTrue == 1)
					{
						string storedProcedureName = "HRM.Update_Active_ChangeStatus";
						var parameters = new DynamicParameters();
						parameters.Add("@tableSchema", "HRM");
						parameters.Add("@tableName", "StaffAddress");
						parameters.Add("@guid_staff_child_primary_id_name", "guid_staff_address_id");
						parameters.Add("@guid_staff_child_primary_id", body.guid_staff_address_id);
						parameters.Add("@updated_date", DateTime.Now);
						parameters.Add("@guid_updated_by", body.userId);
						parameters.Add("@is_active", body.address_is_active);
						connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
						return Ok(new { Message = "Status changed successfully." });
					}
					else //Validation
					{
						// Check if the data already exists
						using (SqlCommand checkCommand = new SqlCommand(
                        "SELECT COUNT(*) FROM HRM.StaffAddress " +
						"WHERE guid_staff_id = @guid_staff_id " +
						"AND guid_city_id = @guid_city_id " +
						"AND staff_address_details = @staff_address_details",
						connection))
						{
							checkCommand.Parameters.AddWithValue("@guid_staff_id", body.guid_staff_id);
							checkCommand.Parameters.AddWithValue("@guid_city_id", body.guid_city_id);
							checkCommand.Parameters.AddWithValue("@staff_address_details", body.staff_address_details);
							//checkCommand.Parameters.AddWithValue("@guid_neighborhood_id", body.guid_neighborhood_id);
							int existingCount = (int)checkCommand.ExecuteScalar();
							if (existingCount > 0)
							{
								// Data already exists, return a message
								return BadRequest("Data already exists");
							}
						}
						// End - Check if the data already exists
						//--- Check if primary id is null then insert else update
						if (body.guid_staff_address_id == isPrimaryId || body.guid_staff_address_id == null)
						{
							action = "insert";
						}
						else
						{
							action = "update";
						}
						// Continue with the insertion/updation if data doesn't exist
						string storedProcedureName = "HRM.StaffAddress_insert_update_delete";
						var parameters = new DynamicParameters();
						parameters.Add("@guid_staff_address_id", body.guid_staff_address_id);
						parameters.Add("@action_perform", action);
						parameters.Add("@guid_staff_id", body.guid_staff_id);
						parameters.Add("@guid_city_id", body.guid_city_id);
						parameters.Add("@guid_neighborhood_id", body.guid_neighborhood_id);
						parameters.Add("@staff_address_details", body.staff_address_details);
						parameters.Add("@is_active", body.address_is_active);
						parameters.Add("@created_date", DateTime.Now);
						parameters.Add("@guid_created_by", body.userId);
						parameters.Add("@updated_date", DateTime.Now);
						parameters.Add("@guid_updated_by", body.userId);
						connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
						return Ok(new { Message = "Data saved successfully." });
					}
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);
			}
		}

		[HttpPost("StaffDataSection_insert")]
		public IActionResult StaffDataSection_insert(StaffGetDataSection body)
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
					if (body.IsActiveTrue == 1)
					{
						string storedProcedureName = "HRM.Update_Active_ChangeStatus";
						var parameters = new DynamicParameters();
						parameters.Add("@tableSchema", "HRM");
						parameters.Add("@tableName", "StaffDataSection");
						parameters.Add("@guid_staff_child_primary_id_name", "guid_staff_datasection_id");
						//parameters.Add("@guid_staff_id", body.guid_staff_id);
						parameters.Add("@guid_staff_child_primary_id", body.guid_staff_datasection_id);
						parameters.Add("@updated_date", DateTime.Now);
						parameters.Add("@guid_updated_by", body.userId);
						parameters.Add("@is_active", body.datasection_is_active);
						connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
						return Ok(new { Message = "Status changed successfully." });
					}
					else //Validation
					{
                        // Check if the data already exists
                        using (SqlCommand checkCommand = new SqlCommand(
                        "SELECT COUNT(*) FROM HRM.StaffDataSection " +
						"WHERE guid_staff_id = @guid_staff_id " +
                        "AND guid_data_section_id = @guid_data_section_id " +
						"AND start_date = @start_date " +
						"AND end_date = @end_date ",
                        connection))
                        {
                            checkCommand.Parameters.AddWithValue("@guid_staff_id", body.guid_staff_id);
                            checkCommand.Parameters.AddWithValue("@guid_data_section_id", body.guid_data_section_id);
                            checkCommand.Parameters.AddWithValue("@start_date", body.datasection_start_date);
                            checkCommand.Parameters.AddWithValue("@end_date", body.datasection_end_date);

                            int existingCount = (int)checkCommand.ExecuteScalar();
                            if (existingCount > 0)
                            {
                                // Data already exists, return a message
                                return BadRequest("Data already exists");
                            }
                        }
                        // End - Check if the data already exists

                        
                        //--- Check if primary id is null then insert else update
                        if (body.guid_staff_datasection_id == isPrimaryId || body.guid_staff_datasection_id == null)
						{
							action = "insert";
						}
						else
						{
							action = "update";
						}
						// Continue with the insertion/updation if data doesn't exist
						string storedProcedureName = "HRM.StaffDataSection_insert_update_delete";
						var parameters = new DynamicParameters();
						parameters.Add("@guid_staff_datasection_id", body.guid_staff_datasection_id);
						parameters.Add("@action_perform", action);
						parameters.Add("@guid_staff_id", body.guid_staff_id);
						parameters.Add("@guid_data_section_id", body.guid_data_section_id);
						parameters.Add("@start_date", body.datasection_start_date);
						parameters.Add("@end_date", body.datasection_end_date);
						parameters.Add("@created_date", DateTime.Now);
						parameters.Add("@guid_created_by", body.userId);
						parameters.Add("@updated_date", DateTime.Now);
						parameters.Add("@guid_updated_by", body.userId);
						parameters.Add("@is_active", body.datasection_is_active);
						connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
						return Ok(new { Message = "Data saved successfully." });
					}
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);
			}
		}

		#endregion

		#region  Staff delete section
		[HttpPost("DataSection_Delete")]
		public IActionResult DataSection_Delete(StaffGetDataSection body)
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
					string storedProcedureName = "HRM.StaffDataSection_insert_update_delete";
					var parameters = new DynamicParameters();
					parameters.Add("@guid_staff_datasection_id", body.guid_staff_datasection_id);
					parameters.Add("@action_perform", "delete");
					parameters.Add("@guid_staff_id", body.guid_staff_id);
					parameters.Add("@guid_data_section_id", body.guid_data_section_id);
					parameters.Add("@start_date", body.datasection_start_date);
					parameters.Add("@end_date", body.datasection_end_date);
					parameters.Add("@created_date", DateTime.Now);
					parameters.Add("@guid_created_by", body.userId);
					parameters.Add("@updated_date", DateTime.Now);
					parameters.Add("@guid_updated_by", body.userId);
					parameters.Add("@is_active", body.datasection_is_active);

					connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
					return Ok(new { Message = "Data saved successfully." });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);

			}
		}
		[HttpPost("Marital_Delete")]
		public IActionResult Marital_Delete(StaffGetMaritalStatus body)
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
					string storedProcedureName = "HRM.StaffMaritalStatus_insert_update_delete";
					var parameters = new DynamicParameters();
					parameters.Add("@@guid_staff_marital_status_id", body.guid_staff_marital_status_id);
					parameters.Add("@action_perform", "delete");
					parameters.Add("@guid_staff_id", body.guid_staff_id);
					parameters.Add("@guid_marital_status_id", body.guid_marital_status_id);
					parameters.Add("@created_date", DateTime.Now);
					parameters.Add("@guid_created_by", body.userId);
					parameters.Add("@updated_date", DateTime.Now);
					parameters.Add("@guid_updated_by", body.userId);
					parameters.Add("@is_active", body.marital_is_active);

					connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
					return Ok(new { Message = "Data saved successfully." });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);

			}
		}
		[HttpPost("Education_Delete")]
		public IActionResult Education_Delete(StaffGetEducationalLevel body)
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
					string storedProcedureName = "HRM.StaffEducationalLevel_insert_update_delete";
					var parameters = new DynamicParameters();
					parameters.Add("@guid_staff_educational_level_id", body.guid_staff_educational_level_id);
					parameters.Add("@action_perform", "delete");
					parameters.Add("@guid_staff_id", body.guid_staff_id);
					parameters.Add("@guid_educational_level_id", body.guid_educational_level_id);
					parameters.Add("@created_date", DateTime.Now);
					parameters.Add("@guid_created_by", body.userId);
					parameters.Add("@updated_date", DateTime.Now);
					parameters.Add("@guid_updated_by", body.userId);
					parameters.Add("@is_active", body.education_is_active);

					connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
					return Ok(new { Message = "Data saved successfully." });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);

			}
		}
		[HttpPost("Address_Delete")]
		public IActionResult Address_Delete(StaffGetAddress body)
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
					string storedProcedureName = "HRM.StaffAddress_insert_update_delete";
					var parameters = new DynamicParameters();
					parameters.Add("@guid_staff_address_id", body.guid_staff_address_id);
					parameters.Add("@action_perform", "delete");
					parameters.Add("@guid_staff_id", body.guid_staff_id);
					parameters.Add("@guid_city_id", body.guid_city_id);
					parameters.Add("@guid_neighborhood_id", body.guid_neighborhood_id);
					parameters.Add("@staff_address_details", body.staff_address_details);
					parameters.Add("@is_active", body.address_is_active);
					parameters.Add("@created_date", DateTime.Now);
					parameters.Add("@guid_created_by", body.userId);
					parameters.Add("@updated_date", DateTime.Now);
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

		[HttpPost("Nationality_Delete")]
		public IActionResult Nationality_Delete(StaffGetNationality body)
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
					string storedProcedureName = "HRM.StaffNationality_insert_update_delete";
					var parameters = new DynamicParameters();
					parameters.Add("@@guid_staff_nationality_id", body.guid_staff_nationality_id);
					parameters.Add("@action_perform", "delete");
					parameters.Add("@guid_staff_id", body.guid_staff_id);
					parameters.Add("@guid_country_id", body.guid_country_id);
					parameters.Add("@is_native", body.is_native);
					parameters.Add("@created_date", DateTime.Now);
					parameters.Add("@guid_created_by", body.userId);
					parameters.Add("@updated_date", DateTime.Now);
					parameters.Add("@guid_updated_by", body.userId);
					parameters.Add("@is_active", body.nationality_is_active);
					connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
					return Ok(new { Message = "Data saved successfully." });
				}
				}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);

			}
		}
		[HttpPost("Contact_Delete")]
		public IActionResult Contact_Delete(StaffGetContactType body)
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
					string storedProcedureName = "HRM.StaffContact_insert_update_delete";
					var parameters = new DynamicParameters();
					parameters.Add("@guid_staff_contact_id", body.guid_staff_contact_id);
					parameters.Add("@action_perform", "delete");
					parameters.Add("@guid_staff_id", body.guid_staff_id);
					parameters.Add("@guid_contact_type_id", body.guid_contact_type_id);
					parameters.Add("@created_date", DateTime.Now);
					parameters.Add("@guid_created_by", body.userId);
					parameters.Add("@updated_date", DateTime.Now);
					parameters.Add("@guid_updated_by", body.userId);
					parameters.Add("@staff_contact_values", body.staff_contact_values);
					parameters.Add("@is_active", body.contact_is_active);

					connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
					return Ok(new { Message = "Data saved successfully." });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);

			}
		}
		[HttpPost("Document_Delete")]
		public IActionResult Document_Delete(StaffGetDocument body)
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
					string storedProcedureName = "HRM.StaffDocument_insert_update_delete";
					var parameters = new DynamicParameters();
					parameters.Add("@guid_staff_document_id", body.guid_staff_document_id);
					parameters.Add("@action_perform", "delete");
					parameters.Add("@guid_staff_id", body.guid_staff_id);
					parameters.Add("@guid_document_type_id", body.guid_document_type_id);
					parameters.Add("@document_number", body.document_number);
					parameters.Add("@start_date", body.document_start_date);
					parameters.Add("@end_date", body.document_end_date);
					parameters.Add("@document_notes", body.document_notes);
					parameters.Add("@document_file", body.document_file);
					parameters.Add("@created_date", DateTime.Now);
					parameters.Add("@guid_created_by", body.userId);
					parameters.Add("@updated_date", DateTime.Now);
					parameters.Add("@guid_updated_by", body.userId);
					parameters.Add("@is_active", body.document_is_active);

					connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
					return Ok(new { Message = "Data saved successfully." });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred: " + ex.Message);

			}
		}
		#endregion

		#region Inside DataTable Filter Search
		[HttpPost("GetNationalityFilterSearch")]
		public IActionResult GetNationalityFilterSearch(StaffGetNationality body)
		{
			try
			{
				string where_phrase = body.searchBody.Length > 0 ? "where " : "";

				for (var i = 0; i < body.searchBody.Length; i++)
				{
					where_phrase += body.searchBody[i].search_key + " LIKE '%" + body.searchBody[i].search_query + "%'";
					if (i + 1 < body.searchBody.Length)
						where_phrase += " and ";
				}

				string connectionString = _connectionString;
				var resultList = new List<Staff>();
				sqlConnectionBuilder.MultipleActiveResultSets = false;
				using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
				{

					connection.Open();
					using (SqlCommand command = new SqlCommand(
						"SELECT guid_country_id,guid_staff_nationality_id, guid_staff_id, nationality, is_native, is_active, created_date " +
						"FROM JRSDB.HRM.StaffNationality_getList " +
						 where_phrase +
                         "AND guid_staff_id = '" + body.guid_staff_id + "'" +
                        " ORDER BY created_date desc ", connection))

					{
						command.CommandType = CommandType.Text;

						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								resultList.Add(new Staff
								{
									guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
									guid_country_id = Guid.Parse(reader["guid_country_id"].ToString()),
									guid_staff_nationality_id = Guid.Parse(reader["guid_staff_nationality_id"].ToString()),
									created_date = reader["created_date"].ToString(),
									nationality_is_active = Convert.ToBoolean((reader["is_active"]).ToString()),
									is_native = Convert.ToBoolean((reader["is_native"]).ToString()),
									nationality = (reader["nationality"]).ToString(),
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

        [HttpPost("GetAddressFilterSearch")]
        public IActionResult GetAddressFilterSearch(StaffGetAddress body)
        {
            try
            {
                string where_phrase = body.searchBody.Length > 0 ? "where " : "";

                for (var i = 0; i < body.searchBody.Length; i++)
                {
                    where_phrase += body.searchBody[i].search_key + " LIKE '%" + body.searchBody[i].search_query + "%'";
                    if (i + 1 < body.searchBody.Length)
                        where_phrase += " and ";
                }

                string connectionString = _connectionString;
                var resultList = new List<Staff>();
                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "SELECT guid_staff_address_id, guid_staff_id, staff_address_details, guid_city_id, " +
						"city_name, state_name, guid_state_id, guid_country_id, country_name, nationality, " +
						"guid_neighborhood_id, neighborhood_name, is_active, created_date " +
                        "FROM JRSDB.HRM.StaffAddress_getList " +
                         where_phrase +
                         "AND guid_staff_id = '" + body.guid_staff_id + "'" +
                        " ORDER BY created_date desc ", connection))

                    {
                        command.CommandType = CommandType.Text;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new Staff
                                {
                                    guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
                                    guid_staff_address_id = Guid.Parse(reader["guid_staff_address_id"].ToString()),
                                    guid_country_id = Guid.Parse(reader["guid_country_id"].ToString()),
                                    guid_state_id = Guid.Parse(reader["guid_state_id"].ToString()),
                                    guid_city_id = Guid.Parse(reader["guid_city_id"].ToString()),
                                    guid_neighborhood_id = Guid.Parse(reader["guid_neighborhood_id"].ToString()),
                                    created_date = reader["created_date"].ToString(),
                                    address_is_active = Convert.ToBoolean((reader["is_active"]).ToString()),
                                    country_name = (reader["country_name"]).ToString(),
                                    state_name = (reader["state_name"]).ToString(),
                                    city_name = (reader["city_name"]).ToString(),
                                    neighborhood_name = (reader["neighborhood_name"]).ToString(),
                                    staff_address_details = (reader["staff_address_details"]).ToString(),
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

        [HttpPost("GetDocumentFilterSearch")]
        public IActionResult GetDocumentFilterSearch(StaffGetDocument body)
        {
            try
            {
                string where_phrase = body.searchBody.Length > 0 ? "where " : "";

                for (var i = 0; i < body.searchBody.Length; i++)
                {
                    where_phrase += body.searchBody[i].search_key + " LIKE '%" + body.searchBody[i].search_query + "%'";
                    if (i + 1 < body.searchBody.Length)
                        where_phrase += " and ";
                }

                string connectionString = _connectionString;
                var resultList = new List<Staff>();
                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "SELECT guid_staff_document_id, guid_staff_id, guid_document_type_id, document_type, " +
                        "document_file, document_notes, document_number, start_date, end_date, is_active, " +
                        "created_date " +
                        "FROM JRSDB.HRM.StaffDocument_getList " +
                         where_phrase +
                         "AND guid_staff_id = '" + body.guid_staff_id + "'" +
                        " ORDER BY created_date desc ", connection))

                    {
                        command.CommandType = CommandType.Text;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new Staff
                                {
                                    guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
                                    guid_staff_document_id = Guid.Parse(reader["guid_staff_document_id"].ToString()),
                                    guid_document_type_id = Guid.Parse(reader["guid_document_type_id"].ToString()),
                                    created_date = reader["created_date"].ToString(),
                                    document_is_active = Convert.ToBoolean((reader["is_active"]).ToString()),
                                    document_type = (reader["document_type"]).ToString(),
                                    document_notes = (reader["document_notes"]).ToString(),
                                    document_number = (reader["document_number"]).ToString(),
                                    document_start_date = (reader["start_date"]).ToString(),
                                    document_end_date = (reader["end_date"]).ToString(),
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

        [HttpPost("GetEducationalFilterSearch")]
        public IActionResult GetEducationalFilterSearch(StaffGetEducationalLevel body)
        {
            try
            {
                string where_phrase = body.searchBody.Length > 0 ? "where " : "";

                for (var i = 0; i < body.searchBody.Length; i++)
                {
                    where_phrase += body.searchBody[i].search_key + " LIKE '%" + body.searchBody[i].search_query + "%'";
                    if (i + 1 < body.searchBody.Length)
                        where_phrase += " and ";
                }

                string connectionString = _connectionString;
                var resultList = new List<Staff>();
                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "SELECT guid_staff_educational_level_id, guid_staff_id, guid_educational_level_id, " +
						"educational_level, is_active, created_date " +
                        "FROM JRSDB.HRM.StaffEducationalLevel_getList " +
                         where_phrase +
                         "AND guid_staff_id = '" + body.guid_staff_id + "'" +
                        " ORDER BY created_date desc ", connection))

                    {
                        command.CommandType = CommandType.Text;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new Staff
                                {
                                    guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
                                    guid_staff_educational_level_id = Guid.Parse(reader["guid_staff_educational_level_id"].ToString()),
                                    guid_educational_level_id = Guid.Parse(reader["guid_educational_level_id"].ToString()),
                                    created_date = reader["created_date"].ToString(),
                                    education_is_active = Convert.ToBoolean((reader["is_active"]).ToString()),
                                    educational_level = (reader["educational_level"]).ToString(),
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

        [HttpPost("GetMaritalFilterSearch")]
        public IActionResult GetMaritalFilterSearch(StaffGetMaritalStatus body)
        {
            try
            {
                string where_phrase = body.searchBody.Length > 0 ? "where " : "";

                for (var i = 0; i < body.searchBody.Length; i++)
                {
                    where_phrase += body.searchBody[i].search_key + " LIKE '%" + body.searchBody[i].search_query + "%'";
                    if (i + 1 < body.searchBody.Length)
                        where_phrase += " and ";
                }

                string connectionString = _connectionString;
                var resultList = new List<Staff>();
                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "SELECT guid_staff_marital_status_id, guid_staff_id, guid_marital_status_id, " +
                        "marital_status, is_active, created_date " +
                        "FROM JRSDB.HRM.StaffMaritalStatus_getList " +
                         where_phrase +
                        "AND guid_staff_id = '" + body.guid_staff_id + "'" +
                        " ORDER BY created_date desc ", connection))

                    {
                        command.CommandType = CommandType.Text;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new Staff
                                {
                                    guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
                                    guid_staff_marital_status_id = Guid.Parse(reader["guid_staff_marital_status_id"].ToString()),
                                    guid_marital_status_id = Guid.Parse(reader["guid_marital_status_id"].ToString()),
                                    created_date = reader["created_date"].ToString(),
                                    marital_is_active = Convert.ToBoolean((reader["is_active"]).ToString()),
                                    marital_status = (reader["marital_status"]).ToString(),
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

        [HttpPost("GetContactFilterSearch")]
        public IActionResult GetContactFilterSearch(StaffGetContactType body)
        {
            try
            {
                string where_phrase = body.searchBody.Length > 0 ? "where " : "";

                for (var i = 0; i < body.searchBody.Length; i++)
                {
                    where_phrase += body.searchBody[i].search_key + " LIKE '%" + body.searchBody[i].search_query + "%'";
                    if (i + 1 < body.searchBody.Length)
                        where_phrase += " and ";
                }

                string connectionString = _connectionString;
                var resultList = new List<Staff>();
                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "SELECT guid_staff_contact_id, guid_staff_id, guid_contact_type_id, contact_type, " +
                        "staff_contact_values, is_active, created_date " +
                        "FROM JRSDB.HRM.StaffContact_getList " +
                         where_phrase +
                         "AND guid_staff_id = '" + body.guid_staff_id + "'" +
                        " ORDER BY created_date desc ", connection))

                    {
                        command.CommandType = CommandType.Text;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new Staff
                                {
                                    guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
                                    guid_staff_contact_id = Guid.Parse(reader["guid_staff_contact_id"].ToString()),
                                    guid_contact_type_id = Guid.Parse(reader["guid_contact_type_id"].ToString()),
                                    created_date = reader["created_date"].ToString(),
                                    contact_is_active = Convert.ToBoolean((reader["is_active"]).ToString()),
                                    staff_contact_values = (reader["staff_contact_values"]).ToString(),
                                    contact_type = (reader["contact_type"]).ToString(),
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

        [HttpPost("GetDataSectionFilterSearch")]
        public IActionResult GetDataSectionFilterSearch(StaffGetDataSection body)
        {
            try
            {
                string where_phrase = body.searchBody.Length > 0 ? "where " : "";

                for (var i = 0; i < body.searchBody.Length; i++)
                {
                    where_phrase += body.searchBody[i].search_key + " LIKE '%" + body.searchBody[i].search_query + "%'";
                    if (i + 1 < body.searchBody.Length)
                        where_phrase += " and ";
                }

                string connectionString = _connectionString;
                var resultList = new List<Staff>();
                sqlConnectionBuilder.MultipleActiveResultSets = false;
                using (SqlConnection connection = new SqlConnection(sqlConnectionBuilder.ToString()))
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(
                        "SELECT guid_staff_datasection_id, guid_staff_id, guid_data_section_id, data_section, " +
                        "objective_code, start_date, end_date, is_active, created_date " +
                        "FROM JRSDB.HRM.StaffDataSection_getList " +
                         where_phrase +
                         "AND guid_staff_id = '" + body.guid_staff_id + "'" +
                        " ORDER BY created_date desc ", connection))

                    {
                        command.CommandType = CommandType.Text;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new Staff
                                {
                                    guid_staff_id = Guid.Parse(reader["guid_staff_id"].ToString()),
                                    guid_staff_datasection_id = Guid.Parse(reader["guid_staff_datasection_id"].ToString()),
                                    guid_data_section_id = Guid.Parse(reader["guid_data_section_id"].ToString()),
                                    created_date = reader["created_date"].ToString(),
                                    datasection_is_active = Convert.ToBoolean((reader["is_active"]).ToString()),
                                    data_section = (reader["data_section"]).ToString(),
                                    objective_code = (reader["objective_code"]).ToString(),
                                    datasection_start_date = (reader["start_date"]).ToString(),
                                    datasection_end_date = (reader["end_date"]).ToString(),
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
        #endregion
    }
}
