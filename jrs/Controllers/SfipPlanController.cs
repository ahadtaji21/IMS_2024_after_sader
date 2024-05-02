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
using System.Text.Json;

namespace jrs.Controllers
{

	/// <summary>
	/// Controller class for all SFIP objects.
	/// </summary>
	[ApiController]
	[Consumes("application/json")]
	public class SfipPlanController : ControllerBase
	{
		private readonly GeneralContext _context;
		private readonly IMSLogContext _logContext;
		private string UserId = "";
		private string IP = "";
		ImsUtility utility = null;

		public SfipPlanController(GeneralContext context, IMSLogContext logContext)
		{
			_context = context;
			_logContext = logContext;
			utility = new ImsUtility();
		}

		[HttpGet("api/sfip/plan", Name = "GetSfipPlan")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<SfipPlan>>> GetSfipPlan(int officeId)
		{
			try
			{
				// Set event variables
				IP = utility.GetIP(Response);
				UserId = User.Identity.Name;

				// Get valid plans
				SfipPlan[] validPlans = await _context.SfipActivities
					.Where<SfipActivity>(act => act.officeId == officeId)
					.Include("Indicator.Plan")
					// .Include("Indicator.Objective")
					.Select(act => act.Indicator.Plan)
					.ToArrayAsync();


				// Log event
				ImsLogevents evt = utility.GetLogEvent("SfipPlanController", "GetSfipPlan", "GetSfipPlan", UserId, new { data = validPlans }, IP);
				_logContext.ImsLogevents.Add(evt);
				_logContext.SaveChanges();

				return validPlans.ToList();
			}
			catch (Exception ex)
			{
				// ImsLogerrors evt = utility.GetLogError("SfipPlanController", "GetSfipPlan", ex, UserId, new { officeID = officeId }, IP);
				ImsLogerrors evt = utility.GetLogError("SfipPlanController", "GetSfipPlan", ex, UserId, new { }, IP);
				_logContext.ImsLogerrors.Add(evt);
				_logContext.SaveChanges();

				// Return error
				return Problem();
			}
		}

		/// <summary>Get Plan Tree starting from Priority Set to Indicator.</summary>
		// GET: api/sfip/plan-tree
		[HttpGet("api/sfip/plan-tree/{planId}/{officeId}", Name = "GetSfipPlanTree")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<SfipPrioritySet>>> GetSfipPlanTree(int planId, int officeId, bool includeActivities)
		{
			try
			{
				// Set event variables
				IP = utility.GetIP(Response);
				UserId = User.Identity.Name;


				HrOffice office = await _context.HrOffices
					.Where(off => off.HrOfficeId == officeId)
					.SingleOrDefaultAsync();
				List<HrOffice> validOffices = new List<HrOffice>();

				// If office is international don't add descendants
				if (office.IsInternational == false)
				{
					var tmpDescendats = await office.CalcFlatDescendants();
					validOffices = tmpDescendats.ToList();
				}
				validOffices.Add(office);

				// List of available offices
				List<int> validOfficeIds = validOffices.ConvertAll(office => office.HrOfficeId);

				SfipPrioritySet[] prioritySets = await this.getPlanChildTree(planId, validOfficeIds, office.HrOfficeId, includeActivities);

				// Log event
				ImsLogevents evt = utility.GetLogEvent("SfipPlanController", "GetSfipPlanTree", "GetSfipPlanTree", UserId, new { data = prioritySets }, IP);
				_logContext.ImsLogevents.Add(evt);
				_logContext.SaveChanges();

				return prioritySets;
			}
			catch (Exception ex)
			{
				ImsLogerrors evt = utility.GetLogError("SfipPlanController", "GetSfipPlanTree", ex, UserId, new { }, IP);
				_logContext.ImsLogerrors.Add(evt);
				_logContext.SaveChanges();

				// Return error
				return Problem();
			}
		}
		[HttpGet("api/sfip/indicator-offices", Name = "GetSfipIndicatorOffices")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<SfipIndicatorOffice>>> GetSfipIndicatorOffices()
		{
			try
			{
				// Set event variables
				IP = utility.GetIP(Response);
				UserId = User.Identity.Name;

				List<SfipIndicatorOffice> indicatorOffices = await _context.SfipIndicatorOffices
					.ToListAsync();

				// Log event
				ImsLogevents evt = utility.GetLogEvent("SfipPlanController", "GetSfipIndicatorOffices", "GetSfipIndicatorOffices", UserId, new { data = indicatorOffices }, IP);
				_logContext.ImsLogevents.Add(evt);
				_logContext.SaveChanges();

				return indicatorOffices;
			}
			catch (Exception ex)
			{
				ImsLogerrors evt = utility.GetLogError("SfipPlanController", "GetSfipIndicatorOffices", ex, UserId, new { }, IP);
				_logContext.ImsLogerrors.Add(evt);
				_logContext.SaveChanges();

				// Return error
				return Problem();
			}
		}

		[HttpGet("api/sfip/indicators")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<SfipIndicator>>> GetSfipIndicators()
		{
			try
			{
				// Set event variables
				IP = utility.GetIP(Response);
				UserId = User.Identity.Name;

				List<SfipIndicator> indicators = await _context.SfipIndicators
					.Include("IndicatorOfficeList")
					.ToListAsync();

				// Log event
				ImsLogevents evt = utility.GetLogEvent("SfipPlanController", "GetSfipIndicators", "GetSfipIndicators", UserId, new { data = indicators }, IP);
				_logContext.ImsLogevents.Add(evt);
				_logContext.SaveChanges();

				return indicators;
			}
			catch (Exception ex)
			{
				ImsLogerrors evt = utility.GetLogError("SfipPlanController", "GetSfipIndicators", ex, UserId, new { }, IP);
				_logContext.ImsLogerrors.Add(evt);
				_logContext.SaveChanges();

				// Return error
				return Problem();
			}
		}

		[HttpGet("api/sfip/activity/{activityId}/schedule", Name = "GetSfipActivitySchedule")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<SfipActivitySchedule>>> GetSfipActivitySchedule(int activityId)
		{
			try
			{
				// Set event variables
				IP = utility.GetIP(Response);
				UserId = User.Identity.Name;

				List<SfipActivitySchedule> schedule = await _context.SfipActivitySchedules
					.Where<SfipActivitySchedule>(scheduleItem => scheduleItem.ActivityId == activityId && (!scheduleItem.IsDeleted ?? false))
					.Include("ScheduleQuarterLookup")
					.ToListAsync();

				// Log event
				ImsLogevents evt = utility.GetLogEvent("SfipPlanController", "GetSfipActivitySchedule", "GetSfipActivitySchedule", UserId, new { data = schedule }, IP);
				_logContext.ImsLogevents.Add(evt);
				_logContext.SaveChanges();

				return schedule;
			}
			catch (Exception ex)
			{
				ImsLogerrors evt = utility.GetLogError("SfipPlanController", "GetSfipActivitySchedule", ex, UserId, new { }, IP);
				_logContext.ImsLogerrors.Add(evt);
				_logContext.SaveChanges();

				// Return error
				return Problem();
			}
		}

		/// <summary>Calculate the plan structure for a given plan, based on office visibility.</summary>
		/// <param name="planId">Plan to retrieve Tabs for.</param>
		/// <param name="validOfficeIds">List of office ids (current and descendants) to determine indicator visibility.</param>
		/// <param name="currentOfficeId">Current office of the user to determine activity visibility.</param>
		/// <param name="includeActivities">Specify if Activities should be included in the tree.</param>
		///<returns>List Priority Sets and children, drescribing the Plan structure.</returns>
		private async Task<SfipPrioritySet[]> getPlanChildTree(int planId, List<int> validOfficeIds, int currentOfficeId, bool includeActivities)
		{
			try
			{

				// Query components
				IQueryable<SfipActivity> activityQuery = (
					from act in _context.SfipActivities
					where !act.isDeleted
						&& act.officeId == currentOfficeId
					select act
				);

				IQueryable<SfipIndicator> indicatorQuery = (
					from indicator in _context.SfipIndicators
					where !indicator.isDeleted
						//&& indicator.objectiveId == objective.id
						&& indicator.sfipId == planId
						&& indicator.IndicatorOfficeList
							.Any(indOff => validOfficeIds.Contains(indOff.officeId))
					select new SfipIndicator()
					{
						id = indicator.id,
						code = indicator.code,
						formulation = indicator.formulation,
						objectiveId = indicator.objectiveId,
						sfipId = indicator.sfipId,
						value1 = indicator.value1,
						value2 = indicator.value2,
						value3 = indicator.value3,
						value4 = indicator.value4,
						value5 = indicator.value5,
						isDeleted = indicator.isDeleted,
						ActivityList = includeActivities
							? activityQuery.Where(act => act.indicatorId == indicator.id).ToArray()
							: null
					});

				IQueryable<SfipObjective> objectiveQuery = (
					from objective in _context.SfipObjectives
					where !objective.isDeleted
						&& objective.IndicatorList
							.Any<SfipIndicator>(ind => ind.sfipId == planId
								& !ind.isDeleted
								&& ind.IndicatorOfficeList
									.Any<SfipIndicatorOffice>(indOff => validOfficeIds.Contains(indOff.officeId))
							)
					select new SfipObjective()
					{
						id = objective.id,
						code = objective.code,
						formulation = objective.formulation,
						goalId = objective.goalId,
						isDeleted = objective.isDeleted,
						IndicatorList = indicatorQuery.Where(indicator => indicator.objectiveId == objective.id).ToArray()
					});

				IQueryable<SfipGoal> goalQuery = (
					from goal in _context.SfipGoals
					where !goal.isDeleted
						&& goal.ObjectiveList
							.Any<SfipObjective>(obj => obj.IndicatorList
							.Any<SfipIndicator>(ind => ind.sfipId == planId
								&& !ind.isDeleted
								&& ind.IndicatorOfficeList
								.Any<SfipIndicatorOffice>(indOff => validOfficeIds.Contains(indOff.officeId))
							))
					select new SfipGoal()
					{
						id = goal.id,
						code = goal.code,
						formulation = goal.formulation,
						priorityId = goal.priorityId,
						isDeleted = goal.isDeleted,
						ObjectiveList = objectiveQuery.Where(obj => obj.goalId == goal.id).ToArray()
					});

				IQueryable<SfipPriority> priorityQuery = (
					from priority in _context.SfipPriorities
					where !priority.isDeleted
						&& priority.GoalList
							.Any<SfipGoal>(goal => goal.ObjectiveList
							.Any<SfipObjective>(obj => obj.IndicatorList
							.Any<SfipIndicator>(ind => ind.sfipId == planId
								&& !ind.isDeleted
								&& ind.IndicatorOfficeList
									.Any<SfipIndicatorOffice>(indOff => validOfficeIds.Contains(indOff.officeId))
							)))
					select new SfipPriority()
					{
						id = priority.id,
						code = priority.code,
						name = priority.name,
						formulation = priority.formulation,
						prioritySetId = priority.prioritySetId,
						isDeleted = priority.isDeleted,
						GoalList = goalQuery.Where(goal => goal.priorityId == priority.id).ToArray()
					});

				IQueryable<SfipPrioritySet> setQuery = (
					from set in _context.SfipPrioritySets
					where !set.isDeleted
						&& set.PriorityList
							.Any<SfipPriority>(priority => priority.GoalList
							.Any<SfipGoal>(goal => goal.ObjectiveList
							.Any<SfipObjective>(obj => obj.IndicatorList
							.Any<SfipIndicator>(ind => ind.sfipId == planId
								&& !ind.isDeleted
								&& ind.IndicatorOfficeList
									.Any<SfipIndicatorOffice>(indOff => validOfficeIds.Contains(indOff.officeId))
							))))
					select new SfipPrioritySet()
					{
						id = set.id,
						code = set.code,
						name = set.name,
						isDeleted = set.isDeleted,
						PriorityList = priorityQuery.Where(priority => priority.prioritySetId == set.id).ToArray()
					}
				);

				var prioritySets = setQuery.ToArray();

				return prioritySets;
			}
			catch (Exception ex)
			{
				// Return error
				throw ex;
			}
		}


		[HttpPut("api/sfip/activity/{activityId}/schedule-list", Name = "PutSfipActivityScheduleList")]
		[Authorize]
		public async Task<IActionResult> PutSfipActivityScheduleList(int activityId, SfipActivitySchedule[] scheduleItems)
		{
			SfipActivitySchedule[] toCreate = scheduleItems.Where(item => item.Id == 0).ToArray();
			SfipActivitySchedule[] toUpdate = scheduleItems.Where(item => item.Id != 0).ToArray();


			using (var dbContextTransaction = _context.Database.BeginTransaction())
			{
				try
				{
					// Create items
					foreach(SfipActivitySchedule item in toCreate)
					{
						try
						{
							item.ScheduleQuarterLookup = null;
							int itemId = item.Id;
							if (activityId != item.ActivityId)
							{
								dbContextTransaction.Rollback();
								return BadRequest();
								// continue;
							}
							_context.SfipActivitySchedules.Add(item);
							await _context.SaveChangesAsync();
						}
						catch (Exception ex)
						{
							throw ex;
						}
					}
					// Update items
					foreach (SfipActivitySchedule item in toUpdate)
					{
						int itemId = item.Id;
						if (activityId != item.ActivityId)
						{
							dbContextTransaction.Rollback();
							return BadRequest();
							// continue;
						}

						_context.Entry(item).State = EntityState.Modified;

						try
						{
							await _context.SaveChangesAsync();
						}
						catch (DbUpdateConcurrencyException)
						{
							if (!SfipActivityScheduleExists(itemId))
							{
								return NotFound();
							}
							else
							{
								throw;
							}
						}
					}
					// Log event
					ImsLogevents evt = utility.GetLogEvent("SfipPlanController", "PutSfipActivityScheduleList", "PutSfipActivityScheduleList", UserId, new { activityId = activityId, scheduleItems = scheduleItems }, IP);
					_logContext.ImsLogevents.Add(evt);
					_logContext.SaveChanges();

					dbContextTransaction.Commit();
				}
				catch (Exception ex)
				{
					dbContextTransaction.Rollback();

					ImsLogerrors evt = utility.GetLogError("SfipPlanController", "PutSfipActivityScheduleList", ex, UserId, new { }, IP);
					_logContext.ImsLogerrors.Add(evt);
					_logContext.SaveChanges();

					// Return error
					return Problem();
				}
				return NoContent();
			}


		}
		private bool SfipActivityScheduleExists(int id)
		{
			return (_context.SfipActivitySchedules?.Any(e => e.Id == id)).GetValueOrDefault();
		}
	}
}
