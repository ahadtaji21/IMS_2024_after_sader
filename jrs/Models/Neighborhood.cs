using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class Neighborhood
{
    public Guid guid_neighborhood_id { get; set; }

    public int neighborhood_id { get; set; }

    public string neighborhood_name { get; set; } = null!;
    public string neighborhood_native_name { get; set; } = null!;


    public Guid? guid_city_id { get; set; }

    public int? city_id { get; set; }

    public int? neighborhood_type_id { get; set; }

    public decimal? latitude { get; set; }

    public decimal? longitude { get; set; }

    public string? description { get; set; }

    public bool? is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    //public virtual ICollection<HouseholdAddress> HouseholdAddresses { get; set; } = new List<HouseholdAddress>();

    // public virtual City? guid_city { get; set; }

    // public virtual NeighborhoodType? neighborhood_type { get; set; }
}
public partial class NeighborhoodView
{
    public Guid guid_neighborhood_id { get; set; }
    public string neighborhood_name { get; set; } = null!;
    public Guid guid_city_id { get; set; }
}