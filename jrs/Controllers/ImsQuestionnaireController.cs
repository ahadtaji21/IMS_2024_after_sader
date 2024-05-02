using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using jrs.DBContexts;
using jrs.Models;
using jrs.Classes;
using Microsoft.Extensions.Configuration;
using jrs.Services;
using System.Text.Json;

namespace jrs.Controllers
{
	/// <summary>
	/// Controller class for all the questionnaires.
	/// </summary>
	[ApiController]
	[Consumes("application/json")]
	public class ImsQuestionnaireController : ControllerBase
	{
		private readonly GeneralContext _context;
		private readonly IMSLogContext _logContext;
		// private readonly IConfiguration _configuration;
		private string UserId = "";
		private string IP = "";
		ImsUtility utility = null;


		/// <summary>
		/// Constructor for the "ImsTemplateController" class.
		/// </summary>
		public ImsQuestionnaireController(GeneralContext context, IMSLogContext logContext)
		{
			_context = context;
			_logContext = logContext;
			// _configuration = configuration;
			utility = new ImsUtility();
		}

		/// <summary>Get Questionnaire</summary>
		/// <param name="officeId">ID of the current office used to filter the returned questionnaires.</param>
		///<returns>List of requested questionnaires.</returns>
		[HttpGet("api/questionnaire/{officeId}")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<ImsQuestionnaire>>> GetImsQuestionnaire(int officeId)
		{
			try
			{
				// Set event variables
				IP = utility.GetIP(Response);
				UserId = User.Identity.Name;

				// Get questionnaires for office
				ImsQuestionnaire[] tmp = await _context.ImsQuestionnaires
					.Where<ImsQuestionnaire>(questionnaire =>
						questionnaire.QuestionnaireOfficeList.Any<ImsQuestionnaireOffice>(qo => qo.officeId == officeId)
					).ToArrayAsync();

				// Log event
				ImsLogevents evt = utility.GetLogEvent("ImsQuestionnaireController", "GetImsQuestionnaire", "GetImsQuestionnaire", UserId, new { data = tmp }, IP);
				_logContext.ImsLogevents.Add(evt);
				_logContext.SaveChanges();

				// Return data
				return tmp.ToList();
			}
			catch (Exception ex)
			{
				// Log error
				ImsLogerrors evt = utility.GetLogError("ImsQuestionnaireController", "GetImsQuestionnaire", ex, UserId, new { officeID = officeId }, IP);
				_logContext.ImsLogerrors.Add(evt);
				_logContext.SaveChanges();

				// Return error
				return BadRequest();
			}
		}

		/// <summary>Get Questionnaire</summary>
		/// <param name="officeId">ID of the current office used to filter the returned questionnaires.</param>
		/// <param name="questionnaireId">ID of the questionnaire to recover.</param>
		///<returns>List of requested questionnaires.</returns>
		[HttpGet("api/questionnaire-by-id/{officeId}/{questionnaireId}")]
		[Authorize]
		public async Task<ActionResult<ImsQuestionnaire>> GetImsQuestionnaireById(int officeId, int questionnaireId)
		{
			try
			{
				// Set event variables
				IP = utility.GetIP(Response);
				UserId = User.Identity.Name;

				// Get questionnaire
				ImsQuestionnaire questionnaire = await _context.ImsQuestionnaires
				.Where<ImsQuestionnaire>(questionnaire =>
					questionnaire.QuestionnaireOfficeList.Any<ImsQuestionnaireOffice>(qo => qo.officeId == officeId)
					&& questionnaire.id == questionnaireId
				)
				.Include("QuestionnaireOfficeList")
				.Include("QuestionnaireTabList")
				.Include("QuestionnaireQuestionList.Question.QuestionAnswerOptionsList")
				.Include("QuestionnaireQuestionList.Question.FormFieldType")
				.SingleOrDefaultAsync();

				// Log event
				ImsLogevents evt = utility.GetLogEvent("ImsQuestionnaireController", "GetImsQuestionnaireById", "GetImsQuestionnaireById", UserId, new { data = questionnaire }, IP);
				_logContext.ImsLogevents.Add(evt);
				_logContext.SaveChanges();

				// Check if data is valid
				if (questionnaire == null)
				{
					return NoContent();
				}

				// Return data
				return Ok(questionnaire);
			}
			catch (System.Exception ex)
			{
				// Log error
				ImsLogerrors evt = utility.GetLogError("ImsQuestionnaireController", "GetImsQuestionnaireById", ex, UserId, new { officeID = officeId, questionnaireId = questionnaireId }, IP);
				_logContext.ImsLogerrors.Add(evt);
				_logContext.SaveChanges();

				// Return error
				return BadRequest();
			}
		}

		/// <summary>Get Questionnaire</summary>
		/// <param name="officeId">ID of the current office used to filter the returned questionnaires.</param>
		/// <param name="questionnaireCode">Code of the questionnaire to recover.</param>
		///<returns>List of requested questionnaires.</returns>
		[HttpGet("api/questionnaire-by-code/{officeId}/{questionnaireCode}")]
		[Authorize]
		public async Task<ActionResult<ImsQuestionnaire>> GetImsQuestionnaireByCode(int officeId, string questionnaireCode)
		{
			try
			{
				// Set event variables
				IP = utility.GetIP(Response);
				UserId = User.Identity.Name;

				// Get questionnaire
				ImsQuestionnaire questionnaire = await _context.ImsQuestionnaires
				.Where<ImsQuestionnaire>(questionnaire =>
					questionnaire.QuestionnaireOfficeList.Any<ImsQuestionnaireOffice>(qo => qo.officeId == officeId)
				)
				.Where<ImsQuestionnaire>(questionnaire => questionnaire.code == questionnaireCode)
				.Include("QuestionnaireOfficeList")
				.Include("QuestionnaireTabList")
				.Include("QuestionnaireQuestionList.Question.QuestionAnswerOptionsList")
				.Include("QuestionnaireQuestionList.Question.FormFieldType")
				.SingleOrDefaultAsync();

				// Log event
				ImsLogevents evt = utility.GetLogEvent("ImsQuestionnaireController", "GetImsQuestionnaireByCode", "GetImsQuestionnaireByCode", UserId, new { data = questionnaire }, IP);
				_logContext.ImsLogevents.Add(evt);
				_logContext.SaveChanges();

				// Check if data is valid
				if (questionnaire == null)
				{
					return NoContent();
				}

				return Ok(questionnaire);
			}
			catch (System.Exception ex)
			{
				// Log error
				ImsLogerrors evt = utility.GetLogError("ImsQuestionnaireController", "GetImsQuestionnaireById", ex, UserId, new { officeID = officeId, questionnaireCode = questionnaireCode }, IP);
				_logContext.ImsLogerrors.Add(evt);
				_logContext.SaveChanges();

				// Return error
				return BadRequest();
			}
		}

		/// <summary>Update Questionnaire-Question</summary>
		/// <param name="questionnaireId">Id of the questionnaire referenced by the question configuration.</param>
		/// <param name="qqList">Array of Questionnaire-Question configurations.</param>
		[HttpPost("api/questionnaireQuestion/{questionnaireId:int}")]
		[Authorize]
		public async Task<IActionResult> PostImsQuestionnaireQuestion(int questionnaireId, ImsQuestionnaireQuestion[] qqList)
		{
			try
			{
				// Set event variables
				IP = utility.GetIP(Response);
				UserId = User.Identity.Name;

				foreach (ImsQuestionnaireQuestion qq in qqList)
				{
					// Verify data consistency
					if (questionnaireId != qq.questionnaireId)
					{
						// Trow Error
						throw new Exception("Invlalid data. Questionnaire-Question configuration inconsistent with \"questionnaireId\".");
					}
					// Setup changed objects
					_context.Entry(qq).State = EntityState.Modified;
				}
				// Save all changes
				await _context.SaveChangesAsync();

				// Log event
				ImsLogevents evt = utility.GetLogEvent("ImsQuestionnaireController", "PutImsQuestionnaireQuestion", "PutImsQuestionnaireQuestion", UserId, new { }, IP);
				_logContext.ImsLogevents.Add(evt);
				_logContext.SaveChanges();

				// Return no content
				return NoContent();
			}
			catch (System.Exception ex)
			{
				// Log error
				ImsLogerrors evt = utility.GetLogError("ImsQuestionnaireController", "GetImsQuestionnaireById", ex, UserId, new { questionnaireId = questionnaireId, qqList = qqList }, IP);
				_logContext.ImsLogerrors.Add(evt);
				_logContext.SaveChanges();

				// Return error
				return BadRequest();
			}
		}

		/// <summary>Get Tabs for the requested Questionnaire</summary>
		/// <param name="officeId">ID of the current office used to filter the questionnaires.</param>
		/// <param name="questionnaireId">Questionnaire to retrieve Tabs for.</param>
		///<returns>List of requested questionnaires.</returns>
		[HttpGet("api/questionnaire-tab-list/{officeId}/{questionnaireId}")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<ImsQuestionnaireTab>>> GetQuestionnaireTabList(int officeId, int questionnaireId)
		{
			try
			{
				// Set event variables
				IP = utility.GetIP(Response);
				UserId = User.Identity.Name;

				// Get tabs
				ImsQuestionnaireTab[] tabList = await _context.ImsQuestionnaireTab
					.Where<ImsQuestionnaireTab>(qt => qt.questionnaireId == questionnaireId)
					.ToArrayAsync();

				// Log event
				ImsLogevents evt = utility.GetLogEvent("ImsQuestionnaireController", "GetQuestionnaireTabList", "GetQuestionnaireTabList", UserId, new { data = tabList }, IP);
				_logContext.ImsLogevents.Add(evt);
				_logContext.SaveChanges();

				// Check if data is valid
				if (tabList.Length == 0)
				{
					return NoContent();
				}

				// Return no content
				return Ok(tabList);
			}
			catch (System.Exception ex)
			{
				// Log error
				ImsLogerrors evt = utility.GetLogError("ImsQuestionnaireController", "GetImsQuestionnaireById", ex, UserId, new { officeId = officeId, questionnaireId = questionnaireId }, IP);
				_logContext.ImsLogerrors.Add(evt);
				_logContext.SaveChanges();

				// Return error
				return BadRequest();
			}
		}


		/// <summary>Get Questionnaire Instances</summary>
		/// <param name="officeId">ID of the current office used to filter the returned questionnaires.</param>
		/// <param name="questionnaireId">ID of the questionnaire to recover instances for.</param>
		///<returns>List of requested questionnaire instances with the related anwers.</returns>
		[HttpGet("api/questionnaireInstance/{officeId}/{questionnaireId}")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<ImsQuestionnaireInstance>>> GetImsQuestionnaireInstance(int officeId, int questionnaireId)
		{
			try
			{
				// Set event variables
				IP = utility.GetIP(Response);
				UserId = User.Identity.Name;

				// Get Questionnaire-Instances
				List<ImsQuestionnaireInstance> instances = await _context.ImsQuestionnaireInstance
				.Where<ImsQuestionnaireInstance>(instance =>
					instance.Questionnaire.id == questionnaireId
					&& instance.Questionnaire.QuestionnaireOfficeList.Any<ImsQuestionnaireOffice>(qo => qo.officeId == officeId)
				)
				.Include("AnswerInstanceList.Question")
				.Include("Questionnaire")
				.Include("Status")
				.ToListAsync();

				// Log event
				ImsLogevents evt = utility.GetLogEvent("ImsQuestionnaireController", "GetImsQuestionnaireInstance", "GetImsQuestionnaireInstance", UserId, new { data = instances }, IP);
				_logContext.ImsLogevents.Add(evt);
				_logContext.SaveChanges();

				if (instances.Count() == 0)
				{
					return NoContent();
				}

				// Return no content
				return Ok(instances);
			}
			catch (System.Exception ex)
			{
				// Log error
				ImsLogerrors evt = utility.GetLogError("ImsQuestionnaireController", "GetImsQuestionnaireInstance", ex, UserId, new { officeId = officeId, questionnaireId = questionnaireId }, IP);
				_logContext.ImsLogerrors.Add(evt);
				_logContext.SaveChanges();

				// Return error
				return BadRequest(ex);
			}
		}

		/// <summary>Retrieve Questionnaire Instances</summary>
		/// <param name="officeId">ID of the current office used to filter the returned questionnaires.</param>
		/// <param name="questionnaireId">ID of the questionnaire to recover instances for.</param>
		/// <param name="conditionRules">Array of conditions.</param>
		///<returns>List of requested questionnaire instances with the related anwers.</returns>
		[HttpPost("api/questionnaireInstanceWithConditions/{officeId}/{questionnaireId}")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<ImsQuestionnaireInstance>>> GetImsQuestionnaireInstanceWithConditions(int officeId, int questionnaireId, [FromBody] GenericConditionRule[]? conditionRules)
		{
			HashSet<int> validIds = new HashSet<int>();
			try
			{
				// Set event variables
				IP = utility.GetIP(Response);
				UserId = User.Identity.Name;

				// Get a list of valid Questionnaire-Instance ID
				string viewName = "UTIL_VI_QUESTIONNAIRE_RESULTS";
				string columnList = "DISTINCT QUESTIONNAIRE_ISNTANCE_ID";
				string whereCondition = $"QUESTIONNAIRE_ID = {questionnaireId.ToString()}";

				string conditions = null;
				if (conditionRules != null && conditionRules.Length > 0)
				{
					string[] conditionStrings = Array.ConvertAll<GenericConditionRule, string>(conditionRules, item => item.ToString());
					conditions = string.Join(",", conditionStrings);
					conditions = $"[{conditions}]";
				}

				var retJson = new SqlParameter("RET_JSON", "") { Direction = ParameterDirection.Output, DbType = DbType.String, Size = -1 };
				await _context.Database.ExecuteSqlInterpolatedAsync($"dbo.UTIL_SP_GENERIC_QUERY_DATA @VIEW_NAME={viewName}, @COLUMN_LIST={columnList}, @WHERE_CONDITION={whereCondition}, @ORDER_STATEMENT={null}, @FETCH_NBR={null}, @OFFSET_NBR={null}, @OFFICE_ID={officeId}, @LOCALE={null}, @IGNORE_OFFICE_FILTER={false}, @CONDITION_RULES={conditions}, @RET_JSON={retJson} output");

				using (JsonDocument jd = JsonDocument.Parse(retJson.Value.ToString()))
				{
					JsonElement root = jd.RootElement;

					// Get data rows
					int countRows = root.TryGetProperty("item_count", out JsonElement itemCount) ? itemCount.GetInt32() : 0;
					JsonElement[] tableRows = new JsonElement[countRows];
					tableRows = root.TryGetProperty("table_data", out JsonElement tableData) ? tableData.EnumerateArray().ToArray() : tableRows;

					// Get Result IDs
					string idColName = "QUESTIONNAIRE_ISNTANCE_ID";
					foreach (JsonElement tableRow in tableRows)
					{
						JsonElement colProperty = tableRow.TryGetProperty(idColName, out JsonElement prop) ? prop : new JsonElement();

						int currId;
						if (colProperty.TryGetInt32(out currId))
						{
							validIds.Add(currId);
						}
					}
				}

				// Get Questionnaire-Instances
				List<ImsQuestionnaireInstance> instances = await _context.ImsQuestionnaireInstance
				.Where<ImsQuestionnaireInstance>(instance =>
					instance.Questionnaire.id == questionnaireId
					&& instance.Questionnaire.QuestionnaireOfficeList.Any<ImsQuestionnaireOffice>(qo => qo.officeId == officeId)
					&& validIds.Contains(instance.id)
				)
				.Include("AnswerInstanceList.Question")
				.Include("Questionnaire")
				.Include("Status")
				.ToListAsync();

				// Log event
				ImsLogevents evt = utility.GetLogEvent("ImsQuestionnaireController", "GetImsQuestionnaireInstanceWithConditions", "GetImsQuestionnaireInstanceWithConditions", UserId, new { data = instances }, IP);
				_logContext.ImsLogevents.Add(evt);
				_logContext.SaveChanges();

				if (instances.Count() == 0)
				{
					return NoContent();
				}

				// Return data
				return Ok(instances);
			}
			catch (System.Exception ex)
			{
				// Log error
				ImsLogerrors evt = utility.GetLogError("ImsQuestionnaireController", "GetImsQuestionnaireInstanceWithConditions", ex, UserId, new { officeId = officeId, questionnaireId = questionnaireId, conditionRules = conditionRules }, IP);
				_logContext.ImsLogerrors.Add(evt);
				_logContext.SaveChanges();

				// Return error
				return BadRequest(ex);
			}
		}

		/// <summary>Get Questionnaire Instance</summary>
		/// <param name="questionnaireInstanceId">ID of the questionnaire-instance to recover.</param>
		///<returns>The requested questionnaire instance with the related anwers.</returns>
		[HttpGet("api/questionnaireInstance/{questionnaireInstanceId}")]
		[Authorize]
		public async Task<ActionResult<ImsQuestionnaireInstance>> GetImsQuestionnaireInstance(int questionnaireInstanceId)
		{
			try
			{
				// Set event variables
				IP = utility.GetIP(Response);
				UserId = User.Identity.Name;

				// Get questionnaire instance
				ImsQuestionnaireInstance qi = await _context.ImsQuestionnaireInstance
					.Where<ImsQuestionnaireInstance>(qi => qi.id == questionnaireInstanceId)
					.Include("AnswerInstanceList.Question")
					.Include("Questionnaire")
					.Include("Status")
					.SingleOrDefaultAsync();
                    
				// Log event
				ImsLogevents evt = utility.GetLogEvent("ImsQuestionnaireController", "GetImsQuestionnaireInstanceWithConditions", "GetImsQuestionnaireInstanceWithConditions", UserId, new { data = qi }, IP);
				_logContext.ImsLogevents.Add(evt);
				_logContext.SaveChanges();

				// Check if data is valid
				if (qi == null)
				{
					return NoContent();
				}

                // Return data
				return Ok(qi);
			}
			catch (System.Exception ex)
			{
				// Log error
				ImsLogerrors evt = utility.GetLogError("ImsQuestionnaireController", "GetImsQuestionnaireInstanceWithConditions", ex, UserId, new { questionnaireInstanceId = questionnaireInstanceId }, IP);
				_logContext.ImsLogerrors.Add(evt);
				_logContext.SaveChanges();

                // Return error
				return BadRequest();
			}
		}


	}
}

