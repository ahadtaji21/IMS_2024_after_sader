using System;
using System.Collections.Generic;

namespace jrs.Models
{
    public partial class ImsUser
    {
        public Guid ImsUserUid { get; set; }
        public DateTime? ImsUserCreationDate { get; set; }
        public string ImsUserLocale { get; set; }
        public string ImsUserUsername { get; set; }
        public string ImsUserPassword { get; set; }
        public string ImsUserRefreshToken { get; set; }

        ///<summary>E-mail address used by Microsoft Azure AD</summary>
        public string ImsUserEmail { get; set; }

        ///<summary>Account Identifier from Microsoft Azure AD</summary>
        public string accountIdentifier { get; set; }

        ///<summary>Bit to verify is the user is active or not (logic deletion)</summary>
        public Boolean ImsUserIsDeleted { get; set; }
    }
}
