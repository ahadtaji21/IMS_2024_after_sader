using System.Threading.Tasks;
using jrs.Interfaces;
using jrs.Models;
using jrs.Services;
using jrs.DBContexts;
using System.Linq;
// using jrs.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using System.Xml.Serialization;
using System.Collections;
using jrs.Classes;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
namespace jrs.Controllers
{

    // [Produces("application/atomsvc+xml")]
[Produces("application/json")]
    public class NavCoaController : ODataController
    {

        private string UserId = "";
        private string IP = "";
        private readonly IMSLogContext _logContext;

        // [HttpGet]
        // [EnableQuery()]

        // public IEnumerable<NavCoa> Get()
        // {

        //     List<NavCoa> coaList = new List<NavCoa>();

        //     NavCoa coa = new NavCoa();

        //     coa.PMS_JRS_COA_NAV_UPDATE_DATE = DateTime.Now;
        //     coaList.Add(coa);
        //     return coaList;


        //}

        private AppDbContext _dbContext;
        private readonly GeneralContext _context;
        public NavCoaController(GeneralContext context, IMSLogContext logContext, AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _context = context;
            _logContext = logContext;
        }

        public IActionResult Get()

        {
            ImsUtility utility = new ImsUtility();
            UserId = User.Identity.Name;
            IP = utility.GetIP(Response);
            List<NavCoa> coaList = new List<NavCoa>();
            string spName = DatabaseConstants.spIMSSelGetJRSCOA;
            SqlDataReader dr = null;
            try
            {
                if (_dbContext.navCoa != null && _dbContext.navCoa.Count() > 0)
                {
                    foreach (var entity in _dbContext.navCoa)
                        _dbContext.navCoa.Remove(entity);
                    _dbContext.SaveChanges();
                }
                using (SqlConnection conn = _context.Database.GetDbConnection() as SqlConnection)
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandType = CommandType.StoredProcedure;

                    sqlcmd.CommandText = spName;

                    dr = sqlcmd.ExecuteReader();
                    while (dr.Read())
                    {
                        NavCoa coa = new NavCoa();
                        coa.Id = int.Parse(dr["PMS_JRS_COA_ID"] + "");
                        coa.No = dr["PMS_JRS_COA_CODE"] + "";
                        coa.Last_Date_Modified = (DateTime.Parse(dr["PMS_JRS_COA_NAV_UPDATE_DATE"] + "")).ToString("s", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                        coa.Name = dr["PMS_JRS_COA_DESCRIPTION"] + "";
                        coa.Search_Name = (dr["PMS_JRS_COA_DESCRIPTION"] + "").ToUpper();
                        coa.Account_Type = dr["PMS_JRS_COA_TYPE"] + "";
                        coa.Account_Category = dr["PMS_JRS_COA_CATEGORY"] + "";
                        coa.Blocked = (dr["PMS_JRS_COA_ENABLED"] + "").ToLower(); ;
                        coa.Global_Dimension_1_Code = "";
                        coa.Global_Dimension_2_Code = "";
                        coa.Income_Balance = "";
                        coa.Debit_Credit = "";
                        coa.No_2 = "";
                        coa.Direct_Posting = "";
                        coa.Reconciliation_Account = "";
                        coa.New_Page = "";
                        coa.No_of_Blank_Lines = "";
                        coa.Indentation = "";
                        coa.Totaling = "";
                        coa.Consol_Translation_Method = "";
                        coa.Consol_Debit_Acc = "";
                        coa.Consol_Credit_Acc = "";
                        coa.Gen_Posting_Type = "";
                        coa.Gen_Bus_Posting_Group = "";
                        coa.Gen_Prod_Posting_Group = "";
                        coa.Automatic_Ext_Texts = "";
                        coa.Tax_Area_Code = "";
                        coa.Omit_Default_Descr_in_Jnl = "";
                        coa.Cost_Type_No = "";
                        coa.Default_Deferral_Template_Code = "";
                        coa.Parent = "";
                        coa.Comment = "";
                        coa.Balance_at_Date = "";
                        coa.Net_Change = "";
                        coa.Budgeted_Amount = "";
                        coa.Balance = "";
                        coa.Budget_at_Date = "";
                        coa.Debit_Amount = "";
                        coa.Credit_Amount = "";
                        coa.Budgeted_Debit_Amount = "";
                        coa.Budgeted_Credit_Amount = "";

                        coa.French_Name = "";
                        _dbContext.navCoa.Add(coa);
                    }
                }



                _dbContext.SaveChanges();
                ImsLogevents evt = utility.GetLogEvent("NavCoaController", "NavCoaController", "Extracted COA list from IMS", UserId, null, IP);
                _logContext.ImsLogevents.Add(evt);
                _logContext.SaveChanges();
                return Ok(_dbContext.navCoa);
            }
            catch (Exception ex)
            {

                ImsLogerrors evt = utility.GetLogError("NavCoaController", "NavCoaController", ex, UserId, "", IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
                throw ex;

            }
        }

        public IActionResult Get(int key)
        {
            return Ok(_dbContext.jrsCoa.Find(key));
        }

        public async Task<IActionResult> Post([FromBody] NavCoa product)
        {
            _dbContext.navCoa.Add(product);
            await _dbContext.SaveChangesAsync();
            return Ok(product);
        }

        public async Task<IActionResult> Update(int key, [FromBody] Delta<NavCoa> delta)
        {
            var product = await _dbContext.navCoa.FindAsync(key);
            delta.Patch(product);
            _dbContext.navCoa.Update(product);
            await _dbContext.SaveChangesAsync();
            return Ok(product);
        }

        public async Task<IActionResult> Delete(int key)
        {
            var product = await _dbContext.navCoa.FindAsync(key);
            _dbContext.navCoa.Remove(product);
            await _dbContext.SaveChangesAsync();
            return Ok(product);
        }

    }



}