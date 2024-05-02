using System.Collections.Generic;
namespace jrs.Models{
    public partial class ImsQuestionAnswerOptions{
        public int id { get; set; }
        public int questionId { get; set; }
        public string answerText { get; set; }
        public bool isCorrectAnswer { get; set; }
        public int score { get; set; }
        public string matrix_code { get; set; }
         public int matrix_row { get; set; }
        public int matrix_column { get; set; }
        // public ImsQuestion Question { get; set; }
    }
}