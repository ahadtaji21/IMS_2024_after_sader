using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace jrs.Models{
    public partial class ImsQuestionnaire{
        public int id { get; set; }
        public string code { get; set; }
        public string title { get; set; }
        public string descr { get; set; }
        public bool includeBeneficiarySelection { get; set; }
        public bool includeHouseholdSelection { get; set; }
        public bool includeSettlementSelection { get; set; }
        public bool includeProjectSelection { get; set; } 
        public virtual ICollection<ImsQuestionnaireOffice> QuestionnaireOfficeList { get; set; }
        public virtual ICollection<ImsQuestionnaireQuestion> QuestionnaireQuestionList { get; set; }
        public virtual ICollection<ImsQuestionnaireTab> QuestionnaireTabList { get; set; }
        // public virtual ICollection<ImsQuestion> QuestionList { get; set; }
    }
}