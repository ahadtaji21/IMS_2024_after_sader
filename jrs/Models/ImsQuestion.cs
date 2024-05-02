using System.Collections.Generic;

namespace jrs.Models{
    public partial class ImsQuestion{
        public int id { get; set; }
        public string questionText { get; set; }
        public string questionHint { get; set; }
        public int questionType { get; set; }
        public ImsFormFieldType FormFieldType { get; set; }
        public virtual ICollection<ImsQuestionnaireQuestion> QuestionnaireQuestionList { get; set; }
        public virtual ICollection<ImsQuestionAnswerOptions> QuestionAnswerOptionsList { get; set; }
    }
}