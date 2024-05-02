using System;

namespace jrs.Models
{
    public partial class ImsAnswerInstance
    {
        public int id { get; set; }
        public int questionnaireInstanceId { get; set; }
        public int questionId { get; set; }
        public string? answerText { get; set; }
        public DateTime? answerDate { get; set; }
        public Boolean? answerCheckbox { get; set; }
        public decimal? answerNumber { get; set; }
        public int? answerOptionId { get; set; }
        public string? answerMatrixOptionId { get; set; }
        public Guid? fillInUser { get; set; }
        public Guid? fillInHousehold { get; set; }
        public virtual ImsQuestion Question { get; set; }
        public ImsQuestionAnswerOptions QuestionAnswerOption { get; set; }
    }
}