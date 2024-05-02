using System;
using System.Collections.Generic;

namespace jrs.Models
{
    public partial class PmsCategoryOfIntervention
    {
        public PmsCategoryOfIntervention()
        {
            PmsCoiTosRel = new HashSet<PmsCoiTosRel>();
        }

        public int PmsCoiId { get; set; }
        public string PmsCoiCode { get; set; }
          public int PmsCoiDescriptionLabelId { get; set; }
        public bool PmsCoiEnabled { get; set; }
        public bool PmsCoiDeleted { get; set; }

        public virtual ICollection<PmsCoiTosRel> PmsCoiTosRel { get; set; }

    }
}
