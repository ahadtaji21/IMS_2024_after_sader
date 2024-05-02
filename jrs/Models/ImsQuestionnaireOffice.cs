using System.Collections.Generic;

namespace jrs.Models{
    public partial class ImsQuestionnaireOffice{
        public int id { get; set; }
        public int questionnaireId { get; set; }
        public int officeId { get; set; }
    }
}
