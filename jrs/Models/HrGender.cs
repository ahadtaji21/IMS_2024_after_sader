using System;
using System.Collections.Generic;

namespace jrs.Models
{
    public partial class HrGender
    {
        public HrGender()
        {
            // HrBiodata = new HashSet<HrBiodata>();
        }

        public int HrGenderId { get; set; }
        public string HrGenderCode { get; set; }
        public string HrGenderDescr { get; set; }

        // public virtual ICollection<HrBiodata> HrBiodata { get; set; }
    }
}
