using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace jrs.Models{
    /// <summary>
    /// Payload for generic sql execution of SP.
    /// </summary>
    public class GenericSqlPayload{

        /// <summary>Type of actions for sql SP.</summary>        
        public enum SqlActionType
        {
            /// <summary>Creation of new row/rows.</summary>
            Create,
            /// <summary>Update of row/rows.</summary>
            Update,
            /// <summary>Deletion of row/rows.</summary>
            Destroy,
            /// <summary>Execution of complet SP with multiple operations.</summary>
            Execute
        }
        
        /// <summary>Name of SP to execute.</summary>
        public string spName { get; set; }

        /// <summary>Tye of action for the selected SP.</summary>
        public SqlActionType actionType { get; set; }

        /// <summary>Payload in JSON format to be used by the SP.</summary>
        public string jsonPayload { get; set; }

        /// <summary>UID of the user that initiated the action.</summary>
        public string userUid { get; set; }

        /// <summary>ID of the active office of the user that initiated the action.</summary>
        public int? officeId { get; set; }

        /// <summary>The users current locale</summary>
        public string currentLocale { get; set; }

        /// <summary>
        /// Generates a string representation of the GenericSqlPayload instance.
        /// </summary>
        /// <returns>
        /// The string representation of the object.
        /// </returns>
        public override string ToString(){
            string ret = $"{{\"spName\":\"{this.spName}\", \"actionType\":\"{this.actionType}\", \"jsonPayload\":{this.jsonPayload}, \"userUid\":\"{this.userUid}\", \"officeId\":\"{this.officeId}\"}}";
            return ret;
        }
    }
}

