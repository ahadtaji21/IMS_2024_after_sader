using System.Collections.Generic;

namespace jrs.Models{
    public partial class ImsQuestionnaireQuestion{
        public int id { get; set; }
        public int questionnaireId { get; set; }
        public int questionId { get; set; }
        public int ordinalPosition { get; set; }
        public int? tabId { get; set; }
        public bool isRequired { get; set; }

        public ImsQuestionnaire Questionnaire { get; set; }
        public ImsQuestion Question { get; set; }
        public ImsQuestionnaireTab QuestionnaireTab { get; set; }
    }
}
