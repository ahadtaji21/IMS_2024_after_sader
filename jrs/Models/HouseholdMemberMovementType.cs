using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class HouseholdMemberMovementType
{
    public Guid guid_household_member_movement_type_id { get; set; }

    public string household_member_movement_type { get; set; } = null!;

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual ICollection<HouseholdMemberMovement> HouseholdMemberMovements { get; set; } = new List<HouseholdMemberMovement>();
}


public partial class HouseholdMemberMovementTypeView
{
    public Guid guid_household_member_movement_type_id { get; set; }

    public string household_member_movement_type { get; set; } = null!;
}