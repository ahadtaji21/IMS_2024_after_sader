using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class HouseholdStatusType
{
    public Guid guid_household_status_type_id { get; set; }

    public string household_status_type { get; set; } = null!;

    public string? household_code { get; set; }

    public Guid? guid_country_id { get; set; }

    public int? country_id { get; set; }

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    // public virtual ICollection<HouseholdCaseVisit> HouseholdCaseVisits { get; set; } = new List<HouseholdCaseVisit>();
}



public partial class HouseholdStatusTypeView
{
    public Guid guid_household_status_type_id { get; set; }

    public string household_status_type { get; set; } = null!;
    public bool has_original_address { get; set; } = false!;
}