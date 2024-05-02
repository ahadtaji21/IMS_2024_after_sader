using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class DocumentType
{
    public Guid guid_document_type_id { get; set; }

    public int document_type_id { get; set; }

    public string document_type { get; set; } = null!;

    public Guid? guid_country_id { get; set; }

    public int? country_id { get; set; }

    public string? folder_path { get; set; }

    public bool is_household { get; set; }

    public string? sample { get; set; }

    public string? regular_expression { get; set; }

    public string? js_mask { get; set; }

    public string? max_length { get; set; }

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    //public virtual ICollection<HouseholdDocumentType> HouseholdDocumentTypes { get; set; } = new List<HouseholdDocumentType>();

    //public virtual ICollection<IndividualDocumentType> IndividualDocumentTypes { get; set; } = new List<IndividualDocumentType>();
}
