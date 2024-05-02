using System;
using System.Collections.Generic;

namespace jrs.Models{
	public partial class SfipPrioritySet{
		public int id { get; set; }
		public string code { get; set; }
		public string name { get; set; }
		public bool isDeleted { get; set; }
		public virtual ICollection<SfipPriority> PriorityList { get; set; }
		// public virtual SfipPlan Plan { get; set; }
	}
}