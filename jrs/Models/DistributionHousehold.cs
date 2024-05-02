using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class DistributionHousehold
{
    public Guid guid_distribution_household_id { get; set; }

    public Guid? guid_distribution_id { get; set; }

    public Guid guid_household_id { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public int? updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    // public virtual Distribution? guid_distribution { get; set; }

    // public virtual Household guid_household { get; set; } = null!;
}

public partial class DistributionHouseholdView
{
    public Guid guid_distribution_household_id { get; set; }

    public Guid? guid_distribution_id { get; set; }

    public Guid guid_household_id { get; set; }
}

