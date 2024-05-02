using System;
using System.Collections.Generic;

namespace jrs.Models
{
	public partial class SfipIndicator
	{
		public int id { get; set; }
		public string code { get; set; }
		public string formulation { get; set; }
		public int objectiveId { get; set; }
		public int sfipId { get; set; }
		public string? value1 {get; set; }
		public string? value2 {get; set; }
		public string? value3 {get; set; }
		public string? value4 {get; set; }
		public string? value5 {get; set; }
		public bool isDeleted { get; set; }
		public virtual SfipObjective Objective { get; set; }
		public virtual SfipPlan Plan { get; set; }
		public virtual ICollection<SfipActivity> ActivityList { get; set; }
		public virtual ICollection<SfipIndicatorOffice> IndicatorOfficeList { get; set; }
	}
}