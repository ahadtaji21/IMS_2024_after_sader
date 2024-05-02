using System;
using System.Collections.Generic;

namespace jrs.Models
{
    public partial class PmsCoiTosRel
    {
        public int PmsCoiTosId { get; set; }
        public int PmsCoiId { get; set; }
        public int PmsTosId { get; set; }

        public virtual PmsCategoryOfIntervention PmsCoi { get; set; }
        public virtual PmsTypeOfService PmsTos { get; set; }
    }
}
