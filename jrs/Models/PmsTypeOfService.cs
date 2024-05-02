using System;
using System.Collections.Generic;

namespace jrs.Models
{
    public partial class PmsTypeOfService
    {
        public PmsTypeOfService()
        {
            PmsCoiTosRel = new HashSet<PmsCoiTosRel>();
        }

        public int PmsTosId { get; set; }
        public string PmsTosCode { get; set; }
        public string PmsTosDescription { get; set; }
        public bool? PmsTosEnabled { get; set; }
        public bool PmsTosDeleted { get; set; }

        public virtual ICollection<PmsCoiTosRel> PmsCoiTosRel { get; set; }
    }
}
