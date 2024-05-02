using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class HouseholdMemberMovement
{
    public Guid guid_household_member_movement_id { get; set; }

    public Guid guid_household_member_movement_type_id { get; set; }

    public Guid old_guid_household_member_id { get; set; }

    public Guid new_guid_household_member_id { get; set; }

    public DateTime movement_date { get; set; }

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual HouseholdMemberMovementType guid_household_member_movement_type { get; set; } = null!;

    public virtual HouseholdMember old_guid_household_member { get; set; } = null!;
}



public partial class HouseholdMemberMovementView
{
    public Guid guid_household_member_movement_id { get; set; }

    public Guid guid_household_member_movement_type_id { get; set; }

    public Guid old_guid_household_member_id { get; set; }

    public Guid new_guid_household_member_id { get; set; }

}