using System;
using System.Collections.Generic;

namespace jrs.Models
{
	public partial class SfipPriority
	{
		public int id { get; set; }
		public string code { get; set; }
		public string name { get; set; }
		public string formulation { get; set; }
		public int prioritySetId { get; set; }
		public bool isDeleted { get; set; }
		public virtual SfipPrioritySet PrioritySet { get; set; }
		public virtual ICollection<SfipGoal> GoalList { get; set; }
	}
}