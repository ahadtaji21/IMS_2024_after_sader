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
    public class NavImsController : ControllerBase
    {
        private readonly GeneralContext _context;
        private readonly IMSLogContext _logContext;
        private readonly IConfiguration Configuration;
        private string _connectionString;
        private string UserId = "";
        private string IP = "";
        public NavImsController(GeneralContext context, IMSLogContext logContext, IConfiguration configuration)
        {
            _context = context;
            _logContext = logContext;
            Configuration = configuration;
        }


        [HttpGet("api/navDefaultFromCompany")]
        public jsLedger GetDefaultFromCompany(string company)
        {
            ImsUtility utility = new ImsUtility();
            try
            {
              
                string _serviceUrl = Configuration.GetValue<string>("NavOData:urlLedger");
                string _serviceUser = Configuration.GetValue<string>("NavOData:user");
                string _servicePwd = Configuration.GetValue<string>("NavOData:password");
                using (System.Net.WebClient wc = new System.Net.WebClient())
                {
                    _serviceUrl = _serviceUrl.Replace("#company#", utility.GetIsoCompany(company));
                    wc.Credentials = new NetworkCredential(_serviceUser, _servicePwd);
                    byte[] data = wc.DownloadData(_serviceUrl);
                    string utfString = Encoding.UTF8.GetString(data, 0, data.Length);
                    JsonTextReader reader = new JsonTextReader(new StringReader(utfString));
                    jsLedger jsledger = JsonConvert.DeserializeObject<jsLedger>(utfString);
                    return jsledger;
                }
            }
            catch (System.Exception ex)
            {
                // check exception object for the error
                ImsLogerrors evt = utility.GetLogError("NavImsController", "GetDefaultFromCompany", ex, UserId, new { company = company }, IP);
                _logContext.ImsLogerrors.Add(evt);
                _logContext.SaveChanges();
            }
            return null;
        }


        [HttpGet]
        public IEnumerable<JRSCoa> GetFromNav()
        {

            ImsUtility utility = new ImsUtility();
            List<NavCoa> coaList = new List<NavCoa>();
            // using (System.Net.WebClient wc = new System.Net.WebClient())
            string _serviceUrl = Configuration.GetValue<string>("NavODataCoa:urlCoa");
            string _serviceUser = Configuration.GetValue<string>("NavODataCoa:user");
            string _servicePwd = Configuration.GetValue<string>("NavODataCoa:password");
            // {
            try
            {
                using (System.Net.WebClient wc = new System.Net.WebClient())
                {
                    wc.Credentials = new NetworkCredential(_serviceUser, _servicePwd);
                    //byte[] data = wc.DownloadData("http://nav.jrs.net:7048/NAV02/ODataV4/Company(%27VAT%27)/Parent_GL_Account");
                    byte[] data = wc.DownloadData(_serviceUrl);
                    //byte[] data = wc.DownloadData("http://nav.jrs.net:7048/NAV02/ODataV4/Company('VAT')/Dimension_Value");
                    string utfString = Encoding.UTF8.GetString(data, 0, data.Length);
                    JsonTextReader reader = new JsonTextReader(new StringReader(utfString));

                    jsClass aa = JsonConvert.DeserializeObject<jsClass>(utfString);

                    List<JRSCoa> jrscoa1 = MapNavJrs(aa.value);
                    return jrscoa1;

                }



                XmlUrlResolver resolver = new XmlUrlResolver();
                resolver.Credentials =  new NetworkCredential("NAV.INTEGRATION", "yBZgObvaGJk/RSL5swufffZjnLMx13qdzXRGDqSq3MY=");
 
              
                XmlUrlResolver resolverProd = new XmlUrlResolver();
                resolverProd.Credentials = new NetworkCredential("LUCA.TARTAGLIA", "HSBt1JFwwiuZlQ+NtPX/loV5CDu/pLH6EZNXbHwu3lY=");


                // Create the reader.
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.XmlResolver = resolver;


                XmlReaderSettings settingsProd = new XmlReaderSettings();
                settingsProd.XmlResolver = resolverProd;


                // Create the XmlReader object.
                String URLString = "https://navstaging.jrs.net:7048/jrsnavtest/OData/Company(%27MASTER%20COMPANY_%27)/Parent_G_L_Account"; ;

                String URLStringProd = "https://nav.jrs.net:7048/NAV02/OData/Company(%27VAT%27)/Parent_GL_Account";


                // Uri serviceRoot = new Uri("https://navstaging.jrs.net:7048/jrsnavtest/OData");
                // Microsoft.Data.Edm.IEdmModel mdl =  Microsoft.Data.Edm.Csdl.EdmxReader.Parse(XmlReader.Create(serviceRoot + "/$metadata", settings)) ;
                // IEdmModel model = Microsoft.Data.Edm.Csdl.EdmxReader.Parse(XmlReader.Create(serviceRoot + "/$metadata", settings)) as IEdmModel;
                // Uri requestUri = new Uri("https://navstaging.jrs.net:7048/jrsnavtest/OData/Company(%27MASTER%20COMPANY_%27)/Parent_G_L_Account");
                // ODataUriParser parser = new ODataUriParser(model, serviceRoot, requestUri);



                // String URLString1 = "https://navstaging.jrs.net:7048/jrsnavtest/OData/Company(%27MASTER%20COMPANY_%27)/$metadata";
                // XmlReader reader1 = XmlReader.Create(URLString1, settings);
                // string outs= reader1.ReadOuterXml();
                //reader1.Read()
                // Stream stream = GenerateStreamFromString(utfString);

                Uri serviceRoot = new Uri("https://nav.jrs.net:7048/NAV02/ODataV4/");
                IEdmModel model = CsdlReader.Parse(XmlReader.Create(serviceRoot + "/$metadata", settingsProd));
                Uri requestUri = new Uri("https://nav.jrs.net:7048/NAV02/ODataV4/Company(%27VAT%27)/Parent_GL_Account?$select=*");
                ODataUriParser parser = new ODataUriParser(model, serviceRoot, requestUri);
                Microsoft.OData.UriParser.ODataPath path = parser.ParsePath();
                using (XmlReader reader = XmlReader.Create(URLString, settings))
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(reader);
                    StringBuilder sb = new StringBuilder();
                    XmlNode node = xmldoc.GetElementsByTagName("feed")[0];
                    XmlNodeList list = node.ChildNodes;
                    string[] atributes = new string[node.ChildNodes.Count];
                    int l = 0;
                    for (int j = 0; j < node.ChildNodes.Count; j++)
                    {


                        if (list[j].Name == "entry")
                        {
                            XmlNodeList entryNodeList = list[j].ChildNodes;
                            NavCoa navCoa = new NavCoa();
                            for (int k = 0; k < entryNodeList.Count; k++)
                            {

                                XmlNode entryNode = entryNodeList[k];

                                switch (entryNode.Name)
                                {
                                    case "id":
                                        break;
                                    case "category":
                                        break;
                                    case "link":
                                        break;
                                    case "title":
                                        break;
                                    case "updated":
                                        break;
                                    case "author":
                                        break;
                                    case "content":
                                        XmlNodeList propertiesList = entryNode.ChildNodes;
                                        XmlNode propertiesNode = propertiesList[0];
                                        XmlNodeList properties = propertiesNode.ChildNodes;

                                        for (int z = 0; z < properties.Count; z++)
                                        {
                                            XmlNode property = properties[z];
                                            switch (property.Name)
                                            {
                                                case "d:No":
                                                    navCoa.No = property.InnerText;
                                                    break;
                                                case "d:Name":
                                                    navCoa.Name = property.InnerText;
                                                    break;
                                                case "d:Search_Name":
                                                    navCoa.Search_Name = property.InnerText;
                                                    break;
                                                case "d:Account_Type":
                                                    navCoa.Account_Type = property.InnerText;
                                                    break;
                                                case "d:Global_Dimension_1_Code":
                                                    navCoa.Global_Dimension_1_Code = property.InnerText;
                                                    break;
                                                case "d:Global_Dimension_2_Code":
                                                    navCoa.Global_Dimension_2_Code = property.InnerText;
                                                    break;
                                                case "d:Income_Balance":
                                                    navCoa.Income_Balance = property.InnerText;
                                                    break;
                                                case "d:Debit_Credit":
                                                    navCoa.Debit_Credit = property.InnerText;
                                                    break;
                                                case "d:No_2":
                                                    navCoa.No_2 = property.InnerText;
                                                    break;
                                                case "d:Blocked":
                                                    navCoa.Blocked = property.InnerText;
                                                    break;
                                                case "d:Direct_Posting":
                                                    navCoa.Direct_Posting = property.InnerText;
                                                    break;
                                                case "d:Reconciliation_Account":
                                                    navCoa.Reconciliation_Account = property.InnerText;
                                                    break;
                                                case "d:New_Page":
                                                    navCoa.New_Page = property.InnerText;
                                                    break;
                                                case "d:No_of_Blank_Lines":
                                                    navCoa.No_of_Blank_Lines = property.InnerText;
                                                    break;
                                                case "d:Indentation":
                                                    navCoa.Indentation = property.InnerText;
                                                    break;
                                                case "d:Last_Date_Modified":
                                                    navCoa.Last_Date_Modified = property.InnerText;
                                                    break;
                                                case "d:Totaling":
                                                    navCoa.Totaling = property.InnerText;
                                                    break;
                                                case "d:Consol_Translation_Method":
                                                    navCoa.Consol_Translation_Method = property.InnerText;
                                                    break;
                                                case "d:Consol_Debit_Acc":
                                                    navCoa.Consol_Debit_Acc = property.InnerText;
                                                    break;
                                                case "d:Consol_Credit_Acc":
                                                    navCoa.Consol_Credit_Acc = property.InnerText;
                                                    break;
                                                case "d:Gen_Posting_Type":
                                                    navCoa.Gen_Posting_Type = property.InnerText;
                                                    break;
                                                case "d:Gen_Bus_Posting_Group":
                                                    navCoa.Gen_Bus_Posting_Group = property.InnerText;
                                                    break;
                                                case "d:Gen_Prod_Posting_Group":
                                                    navCoa.Gen_Prod_Posting_Group = property.InnerText;
                                                    break;
                                                case "d:Automatic_Ext_Texts":
                                                    navCoa.Automatic_Ext_Texts = property.InnerText;
                                                    break;
                                                case "d:Tax_Area_Code":
                                                    navCoa.Tax_Area_Code = property.InnerText;
                                                    break;
                                                case "d:Omit_Default_Descr_in_Jnl":
                                                    navCoa.Omit_Default_Descr_in_Jnl = property.InnerText;
                                                    break;
                                                case "d:Cost_Type_No":
                                                    navCoa.Cost_Type_No = property.InnerText;
                                                    break;
                                                case "d:Default_Deferral_Template_Code":
                                                    navCoa.Default_Deferral_Template_Code = property.InnerText;
                                                    break;
                                                case "d:Parent":
                                                    navCoa.Parent = property.InnerText;
                                                    break;
                                                case "d:Comment":
                                                    navCoa.Comment = property.InnerText;
                                                    break;
                                                case "d:Balance_at_Date":
                                                    navCoa.Balance_at_Date = property.InnerText;
                                                    break;
                                                case "d:Net_Change":
                                                    navCoa.Net_Change = property.InnerText;
                                                    break;
                                                case "d:Budgeted_Amount":
                                                    navCoa.Budgeted_Amount = property.InnerText;
                                                    break;
                                                case "d:Balance":
                                                    navCoa.Balance = property.InnerText;
                                                    break;
                                                case "d:Budget_at_Date":
                                                    navCoa.Budget_at_Date = property.InnerText;
                                                    break;
                                                case "d:Debit_Amount":
                                                    navCoa.Debit_Amount = property.InnerText;
                                                    break;
                                                case "d:Credit_Amount":
                                                    navCoa.Credit_Amount = property.InnerText;
                                                    break;
                                                case "d:Budgeted_Debit_Amount":
                                                    navCoa.Budgeted_Debit_Amount = property.InnerText;
                                                    break;
                                                case "d:Budgeted_Credit_Amount":
                                                    navCoa.Budgeted_Credit_Amount = property.InnerText;
                                                    break;
                                                case "d:Account_Category":
                                                    navCoa.Account_Category = property.InnerText;
                                                    break;
                                                case "d:French_Name":
                                                    navCoa.French_Name = property.InnerText;
                                                    break;
                                            }

                                        }


                                        break;

                                }

                            }
                            coaList.Add(navCoa);

                        }
                        l++;
                    }
                    // for (int i = 0; i < node.ChildNodes.Count; i++)
                    // {
                    //     if (searchText.Text.ToUpper() == atributes[i])
                    //     {
                    //         XmlNodeList lastlist = node.ChildNodes;
                    //         XmlNodeList endlist = lastlist[i].ChildNodes;
                    //         for (int k = 0; k < endlist.Count; k++)
                    //         {
                    //             sb.Append(endlist[k].Name + " - " + endlist[k].InnerText);
                    //             sb.Append("\n" + "\n");
                    //         }

                    //     }

                    // }



                    // while (reader.Read())
                    // {
                    //     switch (reader.NodeType)
                    //     {
                    //         case XmlNodeType.Element:
                    //             Console.WriteLine($"Start Element: {reader.Name}. Has Attributes? : {reader.HasAttributes}");
                    //             break;
                    //         case XmlNodeType.Text:
                    //             Console.WriteLine($"Inner Text: {reader.Value}");
                    //             break;
                    //         case XmlNodeType.EndElement:
                    //             Console.WriteLine($"End Element: {reader.Name}");
                    //             break;
                    //         default:
                    //             Console.WriteLine($"Unknown: {reader.NodeType}");
                    //             break;
                    //     }
                    // }
                    // SyndicationFeed feed = SyndicationFeed.Load(reader);
                    // reader.Close();
                    // // SyndicationFeedFormatter formatter = new Rss20FeedFormatter();
                    // // formatter.ReadFrom(reader);
                    // // SyndicationFeed feed = formatter.Feed;
                    // if (feed != null)
                    // {
                    //     var item = feed.Items.First();
                    //     foreach (SyndicationElementExtension extension in item.ElementExtensions)
                    //     {
                    //         XElement ele = extension.GetObject<XElement>();
                    //         Console.WriteLine(ele.Value);
                    //     }

                    //     var content = item.Content;
                    //     string creator = item.GetCreator();
                    //     string publisher = item.GetPublisher();

                    //     var blogAuthor = (feed.Authors.FirstOrDefault() ?? new SyndicationPerson()).Name;
                    //     var chosenAuthor = creator ?? publisher ?? blogAuthor;
                    //     Console.WriteLine("   Chosen author: {0}", chosenAuthor);

                    //     var namr = item.GetName();
                    // }
                }

                // do something  with data
            }
            catch (System.Exception ex)
            {
                // check exception object for the error
            }
            //  }
            List<JRSCoa> jrscoa = MapNavJrs(coaList);
            return jrscoa;
            // return new List<Student>
            // {
            //     CreateNewStudent("Cody Allen", 130),
            //     CreateNewStudent("Todd Ostermeier", 160),
            //     CreateNewStudent("Viral Pandya", 140)
            // };
        }
        private List<JRSCoa> MapNavJrs(List<NavCoa> navCoaList)
        {
            List<JRSCoa> coaList = new List<JRSCoa>();
            try
            {

                foreach (NavCoa navCoa in navCoaList)
                {
                    JRSCoa coa = new JRSCoa();
                    coa.PMS_JRS_COA_CODE = navCoa.No;
                    coa.PMS_JRS_COA_DESCRIPTION = navCoa.Name;
                    coa.PMS_JRS_COA_TYPE = navCoa.Account_Type;
                    coa.PMS_JRS_COA_CATEGORY = navCoa.Account_Category;
                    DateTime dt = DateTime.MinValue;
                    DateTime.TryParse(navCoa.Last_Date_Modified, out dt);
                    coa.PMS_JRS_COA_NAV_UPDATE_DATE = dt;
                    if (navCoa.Blocked.ToUpper() == "FALSE")
                        coa.PMS_JRS_COA_ENABLED = 1;
                    else
                    {
                        coa.PMS_JRS_COA_ENABLED = 0;
                    }
                    coaList.Add(coa);

                }
            }
            catch (System.Exception)
            {

                throw;
            }

            return coaList;
        }





    }


    public class jsClass
    {

        public List<NavCoa> value { get; set; }
        public string odatacontext { get; set; }
    }

    public class jsLedger
    {
        public string odatacontext { get; set; }

        public List<NavOdataLedger> value { get; set; }

    }


    // [HttpGet]
    // public IEnumerable<JRSCoa> GetFromNav()
    // {
    //     List<JRSCoa> coaList = new List<JRSCoa>();
    //     String URLString = "http://localhost:5000/odatacoa/NavCoa";
    //     XmlUrlResolver resolver = new XmlUrlResolver();
    //     resolver.Credentials = new NetworkCredential("test@atricaricoatbltiscali.onmicrosoft.com", "$bLog1c$123");
    //        XmlReaderSettings settings = new XmlReaderSettings();
    //         settings.XmlResolver = resolver;
    //     try
    //     {


    //     using (XmlReader reader = XmlReader.Create(URLString,settings))
    //     {
    //         XmlDocument xmldoc = new XmlDocument();
    //         xmldoc.Load(reader);
    //         StringBuilder sb = new StringBuilder();
    //         XmlNode node = xmldoc.GetElementsByTagName("feed")[0];
    //         XmlNodeList list = node.ChildNodes;
    //         string[] atributes = new string[node.ChildNodes.Count];
    //         int l = 0;
    //         for (int j = 0; j < node.ChildNodes.Count; j++)
    //         {
    //             //
    //             int k=0;
    //         }
    //     }
    //        }
    //     catch (System.Exception ex)
    //     {

    //         int i=0;
    //     }
    //     return coaList;
    // }

}