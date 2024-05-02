using System;
using System.Collections.Generic;
using jrs.Services;
using jrs.DBContexts;
using System.Linq;

using System.Threading.Tasks;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


namespace jrs.Models
{
	public partial class HrOffice
	{
		public int HrOfficeId { get; set; }
        public string HrOfficeCode { get; set; } = null!;
        public string HrOfficeLegalName { get; set; } = null!;
        public string? HrOfficeLegalNameNative { get; set; }
        public int? HrOfficeLegalLanguageId { get; set; }
        public int? HrOfficeNativeLanguageId { get; set; }
        public string? HrOfficeLegalRepName { get; set; }
        public string? HrOfficeSignatoryName { get; set; }
        public int? HrOfficeLegalAddressId { get; set; }
        public int? HrOfficeCountryAdminAreaId { get; set; }
        public string? HrOfficeRegisteredName { get; set; }
        public bool? IsInternational { get; set; }
        public bool? IsRegional { get; set; }
        public bool? IsCountry { get; set; }
		public virtual ICollection<SfipIndicatorOffice> IndicatorOfficeList { get; set; }

        public virtual ICollection<HrOfficeRelation> HrOfficeRelationChildOffices { get; set; }
        public virtual ICollection<HrOfficeRelation> HrOfficeRelationParentOffices { get; set; }



		[NotMapped]
		public virtual ICollection<HrOffice> FlatDescendantOfficeList { get; set; }

		private readonly GeneralContext _generalContext;

		public HrOffice(GeneralContext generalContext)
		{
			_generalContext = generalContext;
		}

		public async Task<ICollection<HrOffice>> CalcFlatDescendants()
		{
			ICollection<HrOffice> descendants = await _generalContext.HrOffices.FromSqlRaw(
				@"SELECT CHILDREN.*
			FROM HR_UF_OFFICE_CHILDREN({0}) AS TREE
				INNER JOIN HR_OFFICE AS CHILDREN ON CHILDREN.HR_OFFICE_ID = TREE.CHILD_OFFICE_ID
			WHERE DESCENDANCE_LVL > 0", this.HrOfficeId
			).ToListAsync();
			// this.FlatDescendantOfficeList = descendants;
			return descendants;
		}
	}
}
