using System;

namespace jrs.Models
{
    public partial class ImsTemplateOffice
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public int OfficeId { get; set; }
        // public DateTime DateFrom { get; set; }
        // public DateTime DateTo { get; set; }
        public Boolean IsDeleted { get; set; }
    }
}