using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace jrs.Models{
    public partial class ImsQuestionnaireTab{
        public int id { get; set; }
        public string code { get; set; }
        public string descr { get; set; }
        public int questionnaireId { get; set; }
        public int ordinalPosition { get; set; }
        // public virtual ICollection<ImsQuestionnaireQuestion> QuestionnaireQuestionList { get; set; }
        public ImsQuestionnaire Questionnaire { get; set; }
    }
}