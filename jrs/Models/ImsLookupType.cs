using System;
using System.Collections.Generic;

namespace jrs.Models
{
    public partial class ImsLookupType
    {
        // public ImsLookupType()
        // {
        //     ImsLookup = new HashSet<ImsLookup>();
        // }

        public int ImsLookupTypeId { get; set; }
        public string ImsLookupTypeCode { get; set; }
        public string ImsLookupTypeDescr { get; set; }

        // public virtual ICollection<ImsLookup> ImsLookup { get; set; }
    }
}
