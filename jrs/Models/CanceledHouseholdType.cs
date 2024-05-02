using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class CanceledHouseholdType
{
    public Guid guid_canceled_household_type_id { get; set; }

    public string canceled_household_type { get; set; } = null!;

    public string? household_type_notes { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public int? updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual ICollection<CanceledHousehold> CanceledHouseholds { get; set; } = new List<CanceledHousehold>();
}
