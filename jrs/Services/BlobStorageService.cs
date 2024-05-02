using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure;
using jrs.Models;
using System.IO;
using jrs.Interfaces;
using Azure.Storage.Blobs.Specialized;
using System; 
using Microsoft.Extensions.Configuration;

using Microsoft.AspNetCore.Mvc;
 
namespace jrs.Services
{
    public class BlobStorageService: IBlobStorageService
    {
        private IConfiguration _config;
        
        public BlobStorageService(IConfiguration config)
        {
            this._config = config;
        }
        public async Task<BlobUploadDto> UploadFileToContainer(FileModel model, string nameContainer, string folder, string user_uid, int office_id, int document_type_id )
        { 
            var _blobServiceClient = new BlobServiceClient(_config.GetValue<string>("BlobStorageAzure:ConnectionStrings:jrs-blob-storage-azure"));

            var Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            BlobContainerClient blobContainer = null;

            try{

                blobContainer = _blobServiceClient.GetBlobContainerClient(nameContainer);

                //you can check if the container exists or not, then determine to create it or not
                bool isExist = blobContainer.Exists();
                if (!isExist)
                {
                    blobContainer.Create();
                }
            } catch (RequestFailedException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}", e.Status, e.ErrorCode);
                Console.WriteLine(e.Message);
            }
            
            var guidFile = Guid.NewGuid();

            var UniqueName = GetRandomBlobName(model.File.FileName,guidFile);

            var PathFileBlob = folder+"/"+UniqueName;
 
            var blobClient = blobContainer.GetBlobClient(PathFileBlob);

            string uploadedDocVersion = string.Empty;
          
            var blob =await blobClient.UploadAsync(model.File.OpenReadStream());

            uploadedDocVersion = blob.Value.VersionId;
            
            return  new BlobUploadDto
                                            {
                                                ST_GUID = guidFile,
                                                ST_FILE_NAME =  model.File.FileName,
                                                ST_UNIQUE_FILE_NAME = UniqueName,
                                                ST_CONTAINER_NAME = nameContainer,
                                                ST_PATH = nameContainer+"/"+PathFileBlob,
                                                ST_PATH_ROOT = PathFileBlob,
                                                ST_EXTENSION = Path.GetExtension(model.File.FileName),
                                                ST_OFFICE_ID = office_id,
                                                ST_USER_UID = user_uid,
                                                ST_TYPE_DOCUMENT = document_type_id,
                                                ST_DATE_CREATION = Date,
                                                ST_VERSION = uploadedDocVersion,
                                            };
            
            
        }

        public async Task<BlobDownloadDto> DownloadFileFromContainer(string containerName, string uniqueFileName)
        {
            // Get a temporary path on disk where we can download the file
    
            var _blobServiceClient = new BlobServiceClient(_config.GetValue<string>("BlobStorageAzure:ConnectionStrings:jrs-blob-storage-azure"));
            
            var blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);

            var Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

            // Get a reference to a blob named "sample-file
            var blob = blobContainer.GetBlobClient(uniqueFileName);
        
            if (await blob.ExistsAsync())
            {
                    // Download the blob's contents and save it to a file
                       using (var memorystream = new MemoryStream())
                                {
                                    await blob.DownloadToAsync(memorystream);
                                     return  new BlobDownloadDto
                                            {
                                                Name =  uniqueFileName,
                                                Content =  memorystream.ToArray(),
                                                Path = null,
                                                Extension = Path.GetExtension(uniqueFileName),
                                                Office = null,
                                                UserUid = null,
                                                TypeDocument = 0,
                                                Date = Date
                                            };
                                }
                             
            }else{
                       return  new BlobDownloadDto
                       {
                           Name =  uniqueFileName,
                           Content =  null,
                           Path = null,
                           Extension = Path.GetExtension(uniqueFileName),
                           Office = null,
                           UserUid = null,
                           TypeDocument = 0,
                           Date = Date
                       };
            }
        }

        public async Task<BlobUploadDto> UploadOverwriteFileToContainer(FileModel model, string nameContainer, string folder, string uniqueFileName, Guid guidFile, string guidUser)
        { 
            var _blobServiceClient = new BlobServiceClient(_config.GetValue<string>("BlobStorageAzure:ConnectionStrings:jrs-blob-storage-azure"));

            var Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");


            var blobContainer = _blobServiceClient.GetBlobContainerClient(nameContainer);

            var blobName = Path.GetFileName(uniqueFileName);

            var PathFileBlob =folder+"/"+uniqueFileName;;
 
            var blobClient = blobContainer.GetBlobClient(PathFileBlob);

            string uploadedDocVersion = string.Empty;
          
            var blob =await blobClient.UploadAsync(model.File.OpenReadStream(),true);

            uploadedDocVersion = blob.Value.VersionId;  

            
            
            return  new BlobUploadDto
                                            {
                                                ST_GUID = guidFile,
                                                ST_FILE_NAME =  model.File.FileName,
                                                ST_UNIQUE_FILE_NAME = blobName,
                                                ST_CONTAINER_NAME = nameContainer,
                                                ST_PATH = nameContainer+"/"+PathFileBlob,
                                                ST_PATH_ROOT = PathFileBlob,
                                                ST_EXTENSION = Path.GetExtension(model.File.FileName),
                                                ST_OFFICE_ID = 2,
                                                ST_USER_UID = guidUser,
                                                ST_TYPE_DOCUMENT = 1,
                                                ST_DATE_CREATION = Date,
                                                ST_VERSION = uploadedDocVersion,
                                            };
            
            
        }


        public async Task DeleteFileFromContainer(string fileName,string nameContainer)
        {
            // Get a temporary path on disk where we can download the file
    
            var _blobServiceClient = new BlobServiceClient(_config.GetValue<string>("BlobStorageAzure:ConnectionStrings:jrs-blob-storage-azure"));
            
            var blobContainer = _blobServiceClient.GetBlobContainerClient(nameContainer);

            // Get a reference to a blob named "sample-file
            var blob = blobContainer.GetBlobClient(fileName);
        
            if (await blob.ExistsAsync())
            {
                    // Download the blob's contents and save it to a file
                  blob.DeleteIfExists();                            
            }
        }



        public async Task DeleteFileSpecificVersionFromContainer(string fileName,string nameContainer, string versionId)
        {
            // Get a temporary path on disk where we can download the file
    
            var _blobServiceClient = new BlobServiceClient(_config.GetValue<string>("BlobStorageAzure:ConnectionStrings:jrs-blob-storage-azure"));
            
            var blobContainer = _blobServiceClient.GetBlobContainerClient(nameContainer);

            // Get a reference to a blob named "sample-file
            var blob = blobContainer.GetBlobClient(fileName).WithVersion(versionId);
        
            if (await blob.ExistsAsync())
            {
                    // Download the blob's contents and save it to a file
                   await blob.DeleteAsync();                           
            }
        }


        /// <summary> 
		/// string GetRandomBlobName(string filename): Generates a unique random file name to be uploaded  
		/// </summary> 
		private string GetRandomBlobName(string filename, Guid guid )
		{
			string ext = Path.GetExtension(filename);
            //string name = Path.GetFileName(filename);
            string name = Path.GetFileNameWithoutExtension(filename);
            //string uniqueName = name+"_"+string.Format("{0:10}_{1}{2}", DateTime.Now.Ticks,guid, ext);
            string uniqueName = name+"_"+guid;
			return uniqueName;
		}

    }
}