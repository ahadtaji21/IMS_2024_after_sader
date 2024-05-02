using System;
using System.Collections.Generic;

namespace jrs.Models
{
    /// <summary>
    /// Class defining the required payload for generic excel creation based on sql queried data.
    /// </summary>
    public class GenericExelSelectPayload : GenericSqlSelectPayload
    {
        public GenericExcelColumnLabel[] columnLabels { get; set; } 
        public string bodyValues {get;set;}
		public string sheetName {get;set;} 
    }

    /// <summary>
    /// Class defining labels for JrsTable columns.
    /// </summary>
    public class GenericExcelColumnLabel
    {
        public string columnName { get; set; }
        public string columnLabel { get; set; }
    }


    	public class OutpInd {
			public string OUTCOME { get; set; }
			public subCatOutcome[] subCatOutcome { get; set; }
		}
		public class subCatOutcome {
			public string PMS_COI_CODE { get; set; }
			public subCatPmsCoiCode[] subCatPmsCoiCode { get; set; }

		}

		public class subCatPmsCoiCode {
			public string PMS_TOS_DESCRIPTION { get; set; }
			public subCatPmsTosDescription[] subCatPmsTosDescription { get; set; }
		}
		public class subCatPmsTosDescription {
			public string PMS_PROCESS { get; set; }
			public subCatPmsProcess[] subCatPmsProcess { get; set; }
		}
		public class subCatPmsProcess {
			public string PMS_OUTPUT { get; set; }
			public subCatPmsOutput[] subCatPmsOutput { get; set; }
		}

		public class subCatPmsOutput {
			public string PMS_ACTIVITY_OUTPUT_REL_ID { get; set; }
			public string INDICATOR { get; set; }
			public string OUTCOME { get; set; }
			public string PMS_COI_CODE { get; set; }
			public string PMS_OUTPUT { get; set; }
			public string PMS_PROCESS { get; set; }
			public string PMS_TOS_DESCRIPTION { get;set; }
			public string TECHNIQUE { get;set; }

		}
}