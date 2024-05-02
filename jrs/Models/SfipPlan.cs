using System;
using System.Collections.Generic;

namespace jrs.Models
{
	public partial class SfipPlan
	{
		public int id { get; set; }
		public string code { get; set; }
		public int startYear { get; set; }
		public int endYear { get; set; }
		public bool isDeleted { get; set; }
		public virtual ICollection<SfipIndicator> IndicarotList { get; set; }
		// public virtual ICollection<SfipPrioritySet> PrioritySets { get; set; }
	}
}