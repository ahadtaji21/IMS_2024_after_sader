using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using jrs.Classes;
using jrs.DA;
using jrs.DBContexts;
using jrs.Models;
using jrs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
 
namespace jrs.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    [Consumes ("application/json")]
    public class GMTController : ControllerBase {
        private readonly GeneralContext _context;
        private readonly IMSLogContext _logContext;
        private readonly IConfiguration Configuration;
        private string _connectionString;
        string UserId = "";
        string IP = "";
        List<PmsObjective> projectObjectives;
        ImsUtility utility = null;
        /// <summary>
        /// Constructor for the "GenericSqlController" class.
        /// </summary>
        public GMTController (GeneralContext context, IMSLogContext logContext, IConfiguration configuration) {
            _context = context;
            _logContext = logContext;
            utility = new ImsUtility ();
            Configuration = configuration;
        }
        public static void SearchAndReplace (string document) {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open (document, true)) {
                string docText = null;
                using (StreamReader sr = new StreamReader (wordDoc.MainDocumentPart.GetStream ())) {
                    docText = sr.ReadToEnd ();
                }

                // Regex regexText = new Regex ("#%FULLNAME%#");
                // docText = regexText.Replace (docText, "Alberto");

                using (StreamWriter sw = new StreamWriter (wordDoc.MainDocumentPart.GetStream (FileMode.Create))) {
                    sw.Write (docText);
                }
            }
        }

        [HttpGet ("{GetGMTTemplate}")]
        public async Task<ActionResult<Object>> GetGMTTemplate () {

            //   GMTController.SearchAndReplace(@"/Users/albertotricarico/Downloads/sharednew/Appendix I_Grant Agreement Letter Template with Tag.docx");
            //   return StatusCode (500);

            string template = "";
            ImsUtility utility = new ImsUtility ();
            byte[] retBytes = null;

            try {
                UserId = User.Identity.Name;
                IP = utility.GetIP (Response);
                DalDOC dalGMT = new DalDOC (_context, _logContext, UserId, IP);
                byte[] document = dalGMT.GetDOCTemplate ("GMT");
                using (MemoryStream stream = new MemoryStream ()) {
                    stream.Write (document, 0, (int) document.Length);
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open (stream, true)) {
                        string docText = null;
                        using (StreamReader sr = new StreamReader (wordDoc.MainDocumentPart.GetStream ())) {
                            docText = sr.ReadToEnd ();
                        }

                        Regex regexText = new Regex ("#%FULLNAME%#");
                        docText = regexText.Replace (docText, "Alberto");
                        using (StreamWriter sw = new StreamWriter (wordDoc.MainDocumentPart.GetStream (FileMode.Create))) {
                            sw.Write (docText);
                        }
                    }
                    retBytes = stream.ToArray();
                    //stream.Read (retBytes, 0, (int) stream.Length);
                }

              

                // Return Byte array
                return StatusCode (200, retBytes);
            } catch (Exception ex) {
                ImsLogerrors evt = utility.GetLogError ("GMTController", "GetGMTTemplate", ex, UserId, "", IP);
                _logContext.ImsLogerrors.Add (evt);
                _logContext.SaveChanges ();
                return StatusCode (500);
            }

        }
        public byte[] ReadFully (Stream input) {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream ()) {
                int read;
                while ((read = input.Read (buffer, 0, buffer.Length)) > 0) {
                    ms.Write (buffer, 0, read);
                }
                return ms.ToArray ();
            }
        }

    }
}