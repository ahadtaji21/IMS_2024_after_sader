using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jrs.DBContexts;

namespace jrs.Models{
    public partial class ImsTemplate{
        public int TemplateId { get; set; }
        public int TemplateTypeId { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateText { get; set; }
        // public virtual ICollection<ImsTemplateParams> TemplateParams { get; set; }
        public virtual ImsTemplateType TemplateType { get; set; }
        public virtual ICollection<ImsTemplateOffice> TemplateOfficeList { get; set; }
    }
}