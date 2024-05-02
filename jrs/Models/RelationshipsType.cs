using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class RelationshipsType
{
    public int relationships_type_id { get; set; }

    public string relationships_type { get; set; } = null!;

    public int? gender_id { get; set; }

    public string? relationships_type_code { get; set; }

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    // public virtual ICollection<HouseholdMember> HouseholdMembers { get; set; } = new List<HouseholdMember>();
}



public partial class RelationshipsTypeView
{
    public int relationships_type_id { get; set; }

    public string relationships_type { get; set; } = null!;
}