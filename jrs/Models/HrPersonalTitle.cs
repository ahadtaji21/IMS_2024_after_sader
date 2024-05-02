using System;
using System.Collections.Generic;

namespace jrs.Models
{
    public partial class HrPersonalTitle
    {
        public HrPersonalTitle()
        {
            // HrBiodata = new HashSet<HrBiodata>();
        }

        public int HrPersonalTitleId { get; set; }
        public string HrPersonalTitleDescr { get; set; }

        // public virtual ICollection<HrBiodata> HrBiodata { get; set; }
    }
}
