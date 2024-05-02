using System;
using System.Collections.Generic;

namespace jrs.Models
{
	public partial class HrOfficeRelation
	{
		public int Id { get; set; }
		public int ParentOfficeId { get; set; }
		public int ChildOfficeId { get; set; }
		public DateTime? DateStart { get; set; }
		public DateTime? DateEnd { get; set; }

		public virtual HrOffice ChildOffice { get; set; } = null!;
		public virtual HrOffice ParentOffice { get; set; } = null!;
	}
}

