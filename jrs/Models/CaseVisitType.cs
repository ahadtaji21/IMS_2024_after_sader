using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class CaseVisitType
{
    public Guid guid_case_visit_type__id { get; set; }

    public string case_visit_type { get; set; } = null!;

    public string? case_visit_code { get; set; }

    public string? notes { get; set; }

    public bool is_active { get; set; }

    public int? created_by { get; set; }

    public DateTime? created_date { get; set; }

    public int? updated_by { get; set; }

    public DateTime? updated_date { get; set; }

}


public partial class CaseVisitTypeView
{
    public Guid guid_case_visit_type__id { get; set; }

    public string case_visit_type { get; set; } 
}