using System;
using System.Collections.Generic;

namespace jrs.Models{
    public partial class ImsQuestionnaireInstance{
        public int id { get; set; }
        public int questionnaireId { get; set; }
        public Guid? fillInUser { get; set; }
        public Guid? fillInHousehold { get; set; }
        public Guid fillInAssistant { get; set; }
        public DateTime fillInDate { get; set; }
        public Guid? reviewUser { get; set; }
        public DateTime? reviewDate { get; set; }
        public int statusId { get; set; }
        public int relevantOfficeId { get; set; }
        public int? selectedSettlement { get; set; }
        public int? selectedProject { get; set; }
        public virtual ICollection<ImsAnswerInstance> AnswerInstanceList { get; set; }
        public virtual ImsQuestionnaire Questionnaire { get; set; }
        public virtual ImsStatus Status { get; set; }
    }
}