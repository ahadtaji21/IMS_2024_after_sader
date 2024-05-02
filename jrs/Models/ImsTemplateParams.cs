using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jrs.DBContexts;

namespace jrs.Models{
    public partial class ImsTemplateParams{
        public int TemplateParamId { get; set; }
        // public int TemplateId { get; set; }
        public string TemplateParamDescr { get; set; }
        public string TemplateParamColumnName { get; set; }
        public int TemplateTypeId { get; set; }
    }
}
