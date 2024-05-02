using System.Threading.Tasks;
using jrs.Models;
using System;


namespace jrs.Interfaces
{
    public interface IBlobStorageService
    {
        Task<BlobUploadDto> UploadFileToContainer(FileModel model, string nameContainer, string folder,string user_uid, int office_id, int document_type_id );

        Task<BlobUploadDto> UploadOverwriteFileToContainer(FileModel model, string nameContainer, string folder, string uniqueFileName, Guid guidFile, string guidUser);
        Task<BlobDownloadDto> DownloadFileFromContainer(string containerName,string uniqueFileName);
        Task DeleteFileFromContainer(string fileName,string nameContainer);

        Task DeleteFileSpecificVersionFromContainer(string fileName,string nameContainer, string versionId);
    }
}