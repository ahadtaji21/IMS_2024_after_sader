using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class City
{
    public Guid guid_city_id { get; set; }

    public int city_id { get; set; }

    public int? state_id { get; set; }

    public Guid? guid_state_id { get; set; }

    public string city_name { get; set; } = null!;

    public decimal? latitude { get; set; }

    public decimal? longitude { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual ICollection<Neighborhood> Neighborhoods { get; set; } = new List<Neighborhood>();

    public virtual State? guid_state { get; set; }
}



public partial class CityView
{
    public Guid guid_city_id { get; set; }

    public Guid? guid_state_id { get; set; }

    public string city_name { get; set; }
}