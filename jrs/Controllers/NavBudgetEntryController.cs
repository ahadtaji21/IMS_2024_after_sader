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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Xml.Serialization;
using System.Collections;
using jrs.Classes;

namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    public class NavBudgetEntryController : ControllerBase
    {

        private readonly GeneralContext _context;
        private readonly IMSLogContext _logContext;

        private string UserId = "";
        private string IP = "";

        public NavBudgetEntryController(GeneralContext context, IMSLogContext logContext)
        {
            _context = context;
            _logContext = logContext;

        }


 
        [HttpGet]
        public jsClassBE GetFromNav(string Company)
        {
            ImsUtility utility = new ImsUtility();
     
            string urlPrefix = utility.GetNavUrlPrefix();
            string url = urlPrefix + Company + "')/G_L_Budget_Entry";
            try
            {
                using (System.Net.WebClient wc = new System.Net.WebClient())
                {
                    wc.Credentials = utility.GetNavCredential();
                    byte[] data = wc.DownloadData(url);
                    string utfString = Encoding.UTF8.GetString(data, 0, data.Length);
                    //JsonTextReader reader = new JsonTextReader(new StringReader(utfString));

                    jsClassBE aa = JsonConvert.DeserializeObject<jsClassBE>(utfString);

                    //List<JRSCoa> jrscoa1 = MapNavJrs(aa.value);
                    return aa;

                }
            }
            catch (System.Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("CMP:");
                sb.Append(Company);
                sb.Append(";");
                ImsLogerrors evt = utility.GetLogError("NavBudgetEntryController", "GetFromNav", ex, UserId, sb.ToString(), IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();


            }
            return null;

        }





     }
    public class jsClassBE
    {

        public List<NavBudgetEntry> value { get; set; }
        public string odatacontext { get; set; }
    }
}