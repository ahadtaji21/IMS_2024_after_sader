using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class HouseholdCaseVisit
{
    public Guid guid_case_visit_id { get; set; }

    public Guid guid_household_id { get; set; }

    public Guid guid_case_visit_type_id { get; set; }

    public Guid guid_household_status_type_id { get; set; }

    public DateTime? case_visit_date { get; set; }

    public string? case_visit_notes { get; set; }

    public bool is_address_changed { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual CaseVisitType guid_case_visit_type { get; set; } = null!;

    // public virtual Household guid_household { get; set; } = null!;

    // public virtual HouseholdStatusType guid_household_status_type { get; set; } = null!;
}

public partial class HouseholdCaseVisitView
{
    public Guid guid_case_visit_id { get; set; }

    public Guid guid_household_id { get; set; }

    public Guid guid_case_visit_type_id { get; set; }

    public Guid guid_household_status_type_id { get; set; }



}