using System;
using System.Collections.Generic;

namespace jrs.Models
{
    public partial class ImsLanguage
    {
        public ImsLanguage()
        {
            ImsLabels = new HashSet<ImsLabels>();
        }

        public int ImsLanguageId { get; set; }
        public string ImsLanguageIso6391Code { get; set; }
        public string ImsLanguageName { get; set; }
        public string ImsLanguageFamily { get; set; }
        public string ImsLnaguageNotes { get; set; }
        public string ImsLanguageLocale { get; set; }

        public virtual ICollection<ImsLabels> ImsLabels { get; set; }
    }
}
