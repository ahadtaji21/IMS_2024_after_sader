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
	public class HrOfficeController : ControllerBase
	{
		private readonly GeneralContext _context;
		private readonly IMSLogContext _logContext;
		private string UserId = "";
		private string IP = "";
		ImsUtility utility = null;

		public HrOfficeController(GeneralContext context, IMSLogContext logContext)
		{
			_context = context;
			_logContext = logContext;
			utility = new ImsUtility();
		}

		// [HttpGet("api/offices")]
		// [Authorize]
		// public async Task<ActionResult<IEnumerable<HrOffice>>> GetHrOffices()
		// {
		// 	try
		// 	{
		// 		// Set event variables
		// 		IP = utility.GetIP(Response);
		// 		UserId = User.Identity.Name;

		// 		List<HrOffice> offices = await _context.HrOffices
		// 			.Include("HrOfficeRelationChildOffices")
		// 			.ToListAsync();

		// 		// Log event
		// 		ImsLogevents evt = utility.GetLogEvent("HrOfficeController", "GetOffices", "GetOffices", UserId, new { data = offices }, IP);
		// 		_logContext.ImsLogevents.Add(evt);
		// 		_logContext.SaveChanges();

		// 		return offices;
		// 	}
		// 	catch (Exception ex)
		// 	{
		// 		ImsLogerrors evt = utility.GetLogError("HrOfficeController", "GetOffices", ex, UserId, new { }, IP);
		// 		_logContext.ImsLogerrors.Add(evt);
		// 		_logContext.SaveChanges();

		// 		// Return error
		// 		return Problem();
		// 	}
		// }

		[HttpGet("api/office", Name = "GetHrOffice")]
		[Authorize]
		public async Task<ActionResult<HrOffice>> GetHrOffice(int officeId, bool includeChildren = false, bool includeDescendants = false)
		{
			try
			{
				// Set event variables
				IP = utility.GetIP(Response);
				UserId = User.Identity.Name;

				HrOffice office = null;

				if (includeChildren || includeDescendants)
				{
					office = await _context.HrOffices
					  .Where(off => off.HrOfficeId == officeId)
					  .Include("HrOfficeRelationChildOffices")
					  .SingleOrDefaultAsync();

					if (includeDescendants)
					{
						// office.CalcDescendants();
						office.FlatDescendantOfficeList = await office.CalcFlatDescendants();
					}
				}
				else
				{
					office = await _context.HrOffices
					  .Where(off => off.HrOfficeId == officeId)
					  .SingleOrDefaultAsync();
				}

				// Log event
				ImsLogevents evt = utility.GetLogEvent("HrOfficeController", "GetHrOffice", "GetHrOffice", UserId, new { data = office }, IP);
				_logContext.ImsLogevents.Add(evt);
				_logContext.SaveChanges();

				return office;
			}
			catch (Exception ex)
			{
				ImsLogerrors evt = utility.GetLogError("HrOfficeController", "GetHrOffice", ex, UserId, new { }, IP);
				_logContext.ImsLogerrors.Add(evt);
				_logContext.SaveChanges();

				// Return error
				return Problem();
			}
		}

		[HttpGet("api/sfip/office-relations")]
		// [Authorize]
		public async Task<ActionResult<IEnumerable<HrOfficeRelation>>> GetHrOfficeRelations()
		{
			try
			{
				// Set event variables
				IP = utility.GetIP(Response);
				UserId = User.Identity.Name;

				List<HrOfficeRelation> indicatorOffices = await _context.HrOfficeRelations
					.Include("ChildOffice")
					.ToListAsync();

				// Log event
				ImsLogevents evt = utility.GetLogEvent("HrOfficeController", "GetHrOfficeRelations", "GetHrOfficeRelations", UserId, new { data = indicatorOffices }, IP);
				_logContext.ImsLogevents.Add(evt);
				_logContext.SaveChanges();

				return indicatorOffices;
			}
			catch (Exception ex)
			{
				ImsLogerrors evt = utility.GetLogError("HrOfficeController", "GetHrOfficeRelations", ex, UserId, new { }, IP);
				_logContext.ImsLogerrors.Add(evt);
				_logContext.SaveChanges();

				// Return error
				return Problem();
			}
		}
	}
}
