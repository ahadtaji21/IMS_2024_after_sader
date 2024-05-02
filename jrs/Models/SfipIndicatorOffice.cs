using System;
using System.Collections.Generic;

namespace jrs.Models
{
	public partial class SfipIndicatorOffice
	{
		public int id { get; set; }
		public int indicatorId { get; set; }
		public int officeId { get; set; }
		public virtual SfipIndicator Indicator { get; set; }
		// public virtual HrOffice Office { get; set; }
	}
}