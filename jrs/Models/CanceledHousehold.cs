using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class CanceledHousehold
{
    public Guid guid_canceled_household { get; set; }

    public Guid guid_household_id { get; set; }

    public Guid guid_canceled_household_type_id { get; set; }

    public string? canceled_household_note { get; set; }

    public DateTime start_date { get; set; }

    public DateTime? end_date { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public int? updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual CanceledHouseholdType guid_canceled_household_type { get; set; } = null!;

    public virtual Household guid_household { get; set; } = null!;
}
