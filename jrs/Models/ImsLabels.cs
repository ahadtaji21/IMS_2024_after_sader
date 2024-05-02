using System;
using System.Collections.Generic;

namespace jrs.Models
{
    public partial class ImsLabels
    {
        public int ImsLabelsId { get; set; }
        public string ImsLabelsTableName { get; set; }
        public int ImsLabelsNumber { get; set; }
        public int ImsLabelsLanguageId { get; set; }
        public string ImsLabelsValue { get; set; }

        public virtual ImsLanguage ImsLabelsLanguage { get; set; }
    }
}
