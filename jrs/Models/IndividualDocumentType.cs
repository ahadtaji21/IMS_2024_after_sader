using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class IndividualDocumentType
{
    public Guid guid_individual_document_type_id { get; set; }
    public Guid guid_individual_id { get; set; }
    public Guid guid_document_type_id { get; set; }
    public String document_parent { get; set; }
    public String document_type { get; set; }
    public String document_number { get; set; }
    public String IndividualDocumentTypesJson { get; set; }
    public String individualDocumentType { get; set; }
    public Boolean is_active { get; set; }
    public Guid guid_country_id { get; set; }
    public Guid userId { get; set; }

}