using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class IndividualMovementType
{
    public Guid guid_individual_movement_type_id { get; set; }

    public string individual_movement_type { get; set; } = null!;

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }
}

public partial class IndividualMovementTypeView
{
    public Guid guid_individual_movement_type_id { get; set; }

    public string individual_movement_type { get; set; } = null!;

}