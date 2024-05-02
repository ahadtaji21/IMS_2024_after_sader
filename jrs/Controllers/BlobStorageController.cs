using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using jrs.Classes;
using jrs.DA;
using jrs.DBContexts;
using jrs.Interfaces;
using jrs.Models;
using jrs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace jrs.Controllers {
    [Route ("api/[controller]")]
    [ApiController]

    /// <summary>
    /// Controller class to manage Blob Azure Storage files
    /// </summary>

    public class BlobStorageAzure : ControllerBase {
        private readonly IBlobStorageService _fileManagerLogic;

        private readonly GeneralContext _context;

        private readonly IMSLogContext _logContext;

        public BlobStorageAzure (GeneralContext context, IBlobStorageService fileManagerLogic, IMSLogContext logContext) {
            _context = context;
            _fileManagerLogic = fileManagerLogic;
            _logContext = logContext;
        }

        /// <summary>
        /// Upload File (New) to Container in Blob Storage Azure (Storage account needed)
        /// </summary>

        [Route ("UploadFileToContainer")]
        [Consumes ("multipart/form-data")]
        [Authorize]
        [HttpPost]
        public async Task<Object> UploadFileToContainer ([FromForm] FileModel model, string nameContainer, string folder, string user_uid, int office_id, int document_type_id) // string tableNameToSaveDocument, string columnNameToSaveDocument)
        {
            BlobUploadDto PayLoadDocument = new BlobUploadDto ();
            ImsUtility utility = new ImsUtility ();
            var nameCont = nameContainer.Replace ("_", "");
            var IP = utility.GetIP (Response);
            var UserId = User.Identity.Name;

            var retJson = new SqlParameter ("RET_JSON", "") { Direction = ParameterDirection.Output, DbType = DbType.String, Size = -1 };

            string fileOK = "OK";
            if (model == null || model.File == null) fileOK = "KO";
            ImsLogevents evt1 = utility.GetLogEvent ("BlobStorageController", "UploadFileToContainer", "UploadFileToContainer", UserId, new { fileOK = fileOK, nameContainer = nameContainer, folder = folder,office_id=office_id, user_uid = user_uid, document_type_id = document_type_id }, IP);
            _logContext.ImsLogevents.Add (evt1);
            _logContext.SaveChanges ();

            if (model.File != null) {
                try {
                    PayLoadDocument = await _fileManagerLogic.UploadFileToContainer (model, nameCont, folder, user_uid, office_id, document_type_id);

                    BlobPayload payload = new BlobPayload ();

                    payload.PAYLOAD = PayLoadDocument.ToString ();
                    payload.ACTION_TYPE = "Upload";
                    // payload.TABLE_NAME_TO_SAVE = tableNameToSaveDocument  ;
                    // payload.COLUMN_NAME_TO_SAVE =  columnNameToSaveDocument ;

                    await _context.Database.ExecuteSqlInterpolatedAsync ($"dbo.ST_SP_INS_UPD_STORAGE_DOCUMENT @JSON_PAYLOAD={payload.ToString()} , @RET_JSON={retJson} output");

                } catch (Exception ex)

                {
                    //@TODO: Implementare Log

                    var Date = DateTime.Now.ToString ("MM/dd/yyyy HH:mm:ss");
                    PayLoadDocument = new BlobUploadDto {
                        ST_FILE_NAME = PayLoadDocument.ST_FILE_NAME,
                        ST_GUID = PayLoadDocument.ST_GUID,
                        ST_UNIQUE_FILE_NAME = PayLoadDocument.ST_UNIQUE_FILE_NAME,
                        ST_PATH = PayLoadDocument.ST_PATH,
                        ST_PATH_ROOT = PayLoadDocument.ST_PATH_ROOT,
                        ST_EXTENSION = PayLoadDocument.ST_EXTENSION,
                        ST_OFFICE_ID = PayLoadDocument.ST_OFFICE_ID,
                        ST_USER_UID = PayLoadDocument.ST_USER_UID,
                        ST_TYPE_DOCUMENT = PayLoadDocument.ST_TYPE_DOCUMENT,
                        ST_DATE_CREATION = PayLoadDocument.ST_DATE_CREATION,
                        ST_VERSION = PayLoadDocument.ST_VERSION,
                        ST_CONTAINER_NAME = PayLoadDocument.ST_CONTAINER_NAME
                    };

                    await _fileManagerLogic.DeleteFileFromContainer (folder + "/" + PayLoadDocument.ST_UNIQUE_FILE_NAME, nameCont);

                    BlobPayload payload = new BlobPayload ();

                    payload.PAYLOAD = PayLoadDocument.ToString ();
                    payload.ACTION_TYPE = "Blob Eliminated for error in upload action";

                    ImsLogerrors evt = utility.GetLogError ("BlobStorageController", "BlobStorageStructure", ex, UserId, payload.ToString (), IP);
                    _logContext.ImsLogerrors.Add (evt);
                    _logContext.SaveChanges ();
                }
            } else {
                return retJson.Value;
            };

           ImsLogevents evt2 = utility.GetLogEvent ("BlobStorageController", "UploadFileToContainer", "UploadFileToContainer", UserId, new { fileOK = fileOK, nameContainer = nameContainer, folder = folder,office_id=office_id, user_uid = user_uid, document_type_id = document_type_id ,retjson=retJson.Value}, IP);
            _logContext.ImsLogevents.Add (evt2);
            _logContext.SaveChanges ();
            return retJson.Value;

        }

        /// <summary>
        /// Upload File to Container for overwriting of existing file in Blob Storage Azure (Storage account needed)
        /// Warming : Same extension of file ( To read after downloading in correctly way)
        /// </summary>

        [Route ("UploadOverwriteFileToContainer")]
        [Consumes ("multipart/form-data")]
        [Authorize]
        [HttpPost]
        public async Task<Object> UploadOverwriteFileToContainer ([FromForm] FileModel model, string nameContainer, string folder, string uniqueFileName, Guid guidfile, string guidUser) //string tableNameToSaveDocument, string columnNameToSaveDocument)
        {
            BlobUploadDto PayLoadDocument = new BlobUploadDto ();
            ImsUtility utility = new ImsUtility ();

            var retJson = new SqlParameter ("RET_JSON", "") { Direction = ParameterDirection.Output, DbType = DbType.String, Size = -1 };
            var nameCont = nameContainer.Replace ("_", "");
            if (model.File != null) {
                try {

                    PayLoadDocument = await _fileManagerLogic.UploadOverwriteFileToContainer (model, nameCont, folder, uniqueFileName, guidfile, guidUser);

                    BlobPayload payload = new BlobPayload ();

                    payload.PAYLOAD = PayLoadDocument.ToString ();
                    payload.ACTION_TYPE = "Upload";
                    // payload.TABLE_NAME_TO_SAVE = tableNameToSaveDocument  ;
                    // payload.COLUMN_NAME_TO_SAVE =  columnNameToSaveDocument ;

                    await _context.Database.ExecuteSqlInterpolatedAsync ($"dbo.ST_SP_INS_UPD_STORAGE_DOCUMENT @JSON_PAYLOAD={payload.ToString()} , @RET_JSON={retJson} output");

                } catch (Exception ex)

                {
                    //@TODO: Implementare Log

                    var IP = utility.GetIP (Response);
                    var UserId = User.Identity.Name;
                    var Date = DateTime.Now.ToString ("MM/dd/yyyy HH:mm:ss");
                    PayLoadDocument = new BlobUploadDto {
                        ST_FILE_NAME = PayLoadDocument.ST_FILE_NAME,
                        ST_GUID = PayLoadDocument.ST_GUID,
                        ST_UNIQUE_FILE_NAME = PayLoadDocument.ST_UNIQUE_FILE_NAME,
                        ST_PATH = PayLoadDocument.ST_PATH,
                        ST_PATH_ROOT = PayLoadDocument.ST_PATH_ROOT,
                        ST_EXTENSION = PayLoadDocument.ST_EXTENSION,
                        ST_OFFICE_ID = PayLoadDocument.ST_OFFICE_ID,
                        ST_USER_UID = PayLoadDocument.ST_USER_UID,
                        ST_TYPE_DOCUMENT = PayLoadDocument.ST_TYPE_DOCUMENT,
                        ST_DATE_CREATION = PayLoadDocument.ST_DATE_CREATION,
                        ST_VERSION = PayLoadDocument.ST_VERSION,
                        ST_CONTAINER_NAME = PayLoadDocument.ST_CONTAINER_NAME
                    };

                    await _fileManagerLogic.DeleteFileSpecificVersionFromContainer (uniqueFileName, nameCont, PayLoadDocument.ST_VERSION);

                    BlobPayload payload = new BlobPayload ();

                    payload.PAYLOAD = PayLoadDocument.ToString ();
                    payload.ACTION_TYPE = "Blob Eliminated for error in upload action";

                    ImsLogerrors evt = utility.GetLogError ("BlobStorageController", "BlobStorageStructure", ex, UserId, payload.ToString (), IP);
                    _logContext.ImsLogerrors.Add (evt);
                    _logContext.SaveChanges ();
                }
            } else {
                return retJson.Value;
            };

            return retJson.Value;

        }

        /// <summary>
        /// Download File in Blob Storage Azure 
        /// </summary>

        [Route ("DownloadFileFromContainer")]
        [Consumes ("application/json")]
        [Authorize]
        [HttpGet]
        public async Task<Object> DownloadFileFromContainer (string containerName, string UniqueFileName, string extension, string fileName) {
            if (UniqueFileName != null) {
                var nameCont = containerName.Replace ("_", "");
                var file = await _fileManagerLogic.DownloadFileFromContainer (nameCont, UniqueFileName);

                //return File(file.Content, "application/"+extension, fileName);
                //File(file.Content, "application/octet-stream", fileName);
                return file.Content;

            } else {
                return NotFound ();
            };

        }

        /// <summary>
        /// DElete file in Blob Storage Azure 
        /// </summary>

        [Route ("DeleteFileFromContainer")]
        [Consumes ("application/json")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteFileFromContainer (string fileName, string nameContainer) {
            if (fileName != null) {
                var nameCont = nameContainer.Replace ("_", "");
                await _fileManagerLogic.DeleteFileFromContainer (fileName, nameCont);

                return Ok ();

            } else {
                return NotFound ();
            };

        }

        /// <summary>
        /// DElete specific version of the file in Blob Storage Azure 
        /// </summary>

        [Route ("DeleteFileSpecificVersionFromContainer")]
        [Consumes ("application/json")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteFileSpecificVersionFromContainer (string fileName, string nameContainer, string versionId) {
            if (fileName != null) {
                var nameCont = nameContainer.Replace ("_", "");
                await _fileManagerLogic.DeleteFileSpecificVersionFromContainer (fileName, nameCont, versionId);

                return Ok ();

            } else {
                return NotFound ();
            };

        }

    }
}