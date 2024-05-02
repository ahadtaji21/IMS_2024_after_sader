using Microsoft.AspNetCore.Http;
using System;

 
namespace jrs.Models
{
    public class FileModel
    {
        public IFormFile File{get;set;}
    }

    public class BlobUploadDto
    {
        public Guid ST_GUID { get; set; }

        public string ST_FILE_NAME { get; set; }

        public string ST_UNIQUE_FILE_NAME { get; set; }

        public string ST_CONTAINER_NAME { get; set; }

        public string ST_EXTENSION {get; set;}

        public string ST_PATH {get; set;}

        public string ST_PATH_ROOT {get; set;}

        public int ST_OFFICE_ID {get; set;}

        public string ST_USER_UID {get; set;}

        public int ST_TYPE_DOCUMENT {get; set;}

        public string ST_DATE_CREATION {get; set;}


        public string ST_VERSION {get; set;}

        


        /// <summary>
        /// Generates a string representation of the GenericSqlPayload instance.
        /// </summary>
        /// <returns>
        /// The string representation of the object.
        /// </returns>
        public override string ToString(){
            string ret = $"{{\"ST_GUID\":\"{this.ST_GUID}\", \"ST_FILE_NAME\":\"{this.ST_FILE_NAME}\",\"ST_UNIQUE_FILE_NAME\":\"{this.ST_UNIQUE_FILE_NAME}\",\"ST_EXTENSION\":\"{this.ST_EXTENSION}\",\"ST_PATH\":\"{this.ST_PATH}\",\"ST_PATH_ROOT\":\"{this.ST_PATH_ROOT}\",\"ST_OFFICE_ID\":\"{this.ST_OFFICE_ID}\",\"ST_USER_UID\":\"{this.ST_USER_UID}\",\"ST_TYPE_DOCUMENT\":\"{this.ST_TYPE_DOCUMENT}\",\"ST_DATE_CREATION\":\"{this.ST_DATE_CREATION}\",\"ST_VERSION\":\"{this.ST_VERSION}\",\"ST_CONTAINER_NAME\":\"{this.ST_CONTAINER_NAME}\"}}";
            return ret;
        }

    }

    
    public class BlobPayload : BlobUploadDto
      {
        public string PAYLOAD  { get; set; }

        public string ACTION_TYPE {get;set;}

        // public string COLUMN_NAME_TO_SAVE {get;set;}
        // public string TABLE_NAME_TO_SAVE {get;set;}


        

          /// <summary>
        /// Generates a string representation of the GenericSqlPayload instance.
        /// </summary>
        /// <returns>
        /// The string representation of the object.
        /// </returns>
        public override string ToString(){
            string ret = $"{{ \"actionType\":\"{this.ACTION_TYPE}\",  \"jsonPayload\": {this.PAYLOAD}}}"; //\"tableNameToSave\":\"{this.TABLE_NAME_TO_SAVE}\",\"columnNameToSave\":\"{this.COLUMN_NAME_TO_SAVE}\",
            return ret;
        }

    }

    

    public class BlobDownloadDto
      {
        public byte[] Content { get; set; }

        public string Name { get; set; }

        public string Extension {get; set;}

        public string Path {get; set;}

        public string Office {get; set;}

        public string UserUid {get; set;}

        public int TypeDocument {get; set;}

        public string Date {get; set;}
    }
}