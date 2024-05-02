using System;
using System.Collections.Generic;

namespace jrs.Models
{
	public partial class SfipObjective
	{
		public int id { get; set; }
		public string code { get; set; }
		public string formulation { get; set; }
		public int goalId { get; set; }
		public bool isDeleted { get; set; }
		public virtual SfipGoal Goal { get; set; }
		public virtual ICollection<SfipIndicator> IndicatorList { get; set; }
	}
}