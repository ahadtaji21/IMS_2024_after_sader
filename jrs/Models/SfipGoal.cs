using System;
using System.Collections.Generic;

namespace jrs.Models
{
	public partial class SfipGoal
	{
		public int id { get; set; }
		public string code { get; set; }
		public string formulation { get; set; }
		public int priorityId { get; set; }
		public bool isDeleted { get; set; }
		public virtual SfipPriority Priority { get; set; }
		public virtual ICollection<SfipObjective> ObjectiveList { get; set; }
	}
}