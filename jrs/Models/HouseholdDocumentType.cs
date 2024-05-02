using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class HouseholdDocumentType
{
    public Guid household_document_type_id { get; set; }

    public Guid guid_household_id { get; set; }

    public Guid? guid_document_type_id { get; set; }


    public string document_number { get; set; } = null!;
    public string document_type { get; set; }

    public string? letter { get; set; }

    public DateTime? start_date { get; set; }

    public DateTime? end_date { get; set; }

    public string? document_notes { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public int? updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    //public virtual DocumentType? guid_document_type { get; set; }

    //public virtual Household guid_household { get; set; } = null!;
}


public partial class HouseholdDocumentTypeView
{
    public Guid? guid_document_type_id { get; set; }
    public string document_type { get; set; }
}