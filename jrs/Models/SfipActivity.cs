using System;
using System.Collections.Generic;

namespace jrs.Models
{
	public partial class SfipActivity
	{
		public int id { get; set; }
		public string code { get; set; }
		public string descr { get; set; }
		public int indicatorId { get; set; }
		public int officeId { get; set; }
		public bool isDeleted { get; set; }
		public virtual SfipIndicator Indicator { get; set; }
		public virtual ICollection<SfipActivitySchedule> SfipActivitySchedules { get; set; }
	}
}