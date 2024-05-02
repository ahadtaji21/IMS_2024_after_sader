using System;
using System.Collections.Generic;

namespace jrs.tmp
{
    public partial class PmsCategoryOfIntervention
    {
        public int PmsCoiId { get; set; }
        public string PmsCoiCode { get; set; }
        public bool PmsCoiEnabled { get; set; }
        public bool PmsCoiDeleted { get; set; }
        public int? PmsCoiDescriptionLabelId { get; set; }
    }
}
