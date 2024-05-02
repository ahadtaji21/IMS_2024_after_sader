using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class HouseholdDataSection
{
    public Guid guid_household_datasection_id { get; set; }

    public Guid guid_household_id { get; set; }

    public Guid guid_data_section_id { get; set; }

    public DateTime? start_date { get; set; }

    public DateTime? end_date { get; set; }

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual DataSection guid_data_section { get; set; } = null!;

    public virtual Household guid_household { get; set; } = null!;
}


public partial class HouseholdDataSectionView
{
    public Guid guid_household_datasection_id { get; set; }

    public Guid guid_household_id { get; set; }

    public Guid guid_data_section_id { get; set; }
}