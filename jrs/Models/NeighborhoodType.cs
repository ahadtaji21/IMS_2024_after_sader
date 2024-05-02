using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class NeighborhoodType
{
    public int neighborhood_type_id { get; set; }

    public string? neighborhood_type { get; set; }

    public string? description { get; set; }

    public bool? is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual ICollection<Neighborhood> Neighborhoods { get; set; } = new List<Neighborhood>();
}

public partial class NeighborhoodTypeView
{
    public int neighborhood_type_id { get; set; }

    public string? neighborhood_type { get; set; }
}