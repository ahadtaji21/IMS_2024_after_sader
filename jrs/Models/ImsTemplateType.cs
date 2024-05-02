using System.Collections.Generic;


namespace jrs.Models{
    public partial class ImsTemplateType{
        public int TemplateTypeId { get; set; }
        public string TemplateTypeCode { get; set; }
        public string TemplateTypeDescr { get; set; }
        public string TemplateDataQuery { get; set; }
        public string SelectItemsDatasource { get; set; }
        public string SelectItemKey { get; set; }
        public string SelectItemText { get; set; }
        public string FieldTranslationKey { get; set; }
        public string DefaultDataQueryCondition { get; set; }
        public virtual ICollection<ImsTemplateParams> TemplateParams { get; set; }
    }
}