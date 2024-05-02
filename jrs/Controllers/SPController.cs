using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.SharePoint;
using System.IO;
using System.Net;
//using Microsoft.SharePoint.Client;

namespace jrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SPController : ControllerBase
    {
        // GET: api/SP
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/SP/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SP
        [HttpPost]
        public void Post([FromBody] string localPath)
        {
            //NetworkCredential myCred = new NetworkCredential("vat100.imsblogic@jrsexternal.info", "Lag79195");
            //string filePath = "";
            //using (var sp = new ClientContext("https://jesuitrefugeeservice720.sharepoint.com/teams/IMSdev"))
            //{
            //    sp.Credentials = myCred;
               
            //    sp.Web.GetFileByServerRelativeUrl(filePath).DeleteObject();
            //    sp.ExecuteQuery();
            //}

            // var sourceFilePath = @"E:\PM1.pdf";
            // var targetUrl = "/Shared Documents";
            // System.Security.SecureString securePwd = new NetworkCredential("", "Lag79195").SecurePassword;
            
            // using (var ctx = new ClientContext("https://jesuitrefugeeservice720.sharepoint.com/teams/IMSdev/"))
            // {
            //     ctx.Credentials = new SharePointOnlineCredentials("vat100.imsblogic@jrsexternal.info", securePwd);

            //     //Upload file
            //     var targetFileUrl = String.Format("{0}/{1}", targetUrl, Path.GetFileName(sourceFilePath));
            //     using (var fs = new FileStream(sourceFilePath, FileMode.Open))
            //     {
            //         Microsoft.SharePoint.Client.File.SaveBinaryDirect(ctx, targetFileUrl, fs, true);
            //     }

            //     //Set document properties
            //     var uploadedFile = ctx.Web.GetFileByServerRelativeUrl(targetFileUrl);
            //     var listItem = uploadedFile.ListItemAllFields;
            //     listItem["DocumentType"] = "Information";
            //     listItem.Update();
            //     ctx.ExecuteQuery();

            // }
        }

        // PUT: api/SP/5
        [HttpPut("{id}")]
        public void Put(int id)
        {
            // var sourceFilePath = @"/Users/albertotricarico/Documents/blogic/gesuiti/documenti JRS/v2.3/GM Requirements V2.0 (1).pdf";
            // var targetUrl = "/Shared Documents";
            // System.Security.SecureString securePwd = new NetworkCredential("", "Lag79195").SecurePassword;

            // using (var ctx = new ClientContext("https://jesuitrefugeeservice720.sharepoint.com/teams/IMSdev"))
            // {
            //     ctx.Credentials = new SharePointOnlineCredentials("vat100.imsblogic@jrsexternal.info","Lag79195");
              
            //     //Upload file
            //     var targetFileUrl = String.Format("{0}/{1}", targetUrl, Path.GetFileName(sourceFilePath));
            //     using (var fs = new FileStream(sourceFilePath, FileMode.Open))
            //     {
            //         // Microsoft.SharePoint.Client.File.
            //         // Microsoft.SharePoint.Client.File.SaveBinaryDirect(ctx, targetFileUrl, fs, true);
            //     }

            //     //Set document properties
            //     var uploadedFile = ctx.Web.GetFileByServerRelativeUrl(targetFileUrl);
            //     var listItem = uploadedFile.ListItemAllFields;
            //     listItem["DocumentType"] = "Information";
            //     listItem.Update();
            //     ctx.ExecuteQueryAsync();

            // }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        //     NetworkCredential myCred = new NetworkCredential("vat100.imsblogic@jrsexternal.info", "Lag79195");

        //     string filePath = "";
        //     using (var sp = new ClientContext("https://jesuitrefugeeservice720.sharepoint.com/teams/IMSdev"))
        //     {
        //         sp.Credentials = myCred;
        //         sp.Web.GetFileByServerRelativeUrl(filePath).DeleteObject();
        //         sp.ExecuteQuery();
        //     }
        //     using (var sp = new ClientContext("webUrl"))
        //     {
        //         sp.Credentials = System.Net.CredentialCache.DefaultCredentials;
        //         var file = sp.Web.GetFileByServerRelativeUrl(filePath);
        //         sp.Load(file, f => f.Exists);
        //         file.DeleteObject();
        //         sp.ExecuteQuery();
        //         if (!file.Exists)
        //             throw new System.IO.FileNotFoundException();
        //     }
         }


    }
}
