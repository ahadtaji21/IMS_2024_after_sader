using System;
using System.Collections.Generic;

namespace jrs.Models
{
    public partial class SfipActivitySchedule
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int ScheduleYear { get; set; }
        public int ScheduleQuarterLookupId { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual SfipActivity Activity { get; set; } = null!;
        public virtual ImsLookup ScheduleQuarterLookup { get; set; } = null!;
    }
}
