using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class Country
{
    public Guid guid_country_id { get; set; }

    public int country_id { get; set; }

    public int continent_id { get; set; }

    public Guid guid_continent_id { get; set; }

    public string country_name { get; set; } = null!;

    public string? country_formal_name { get; set; }

    public string? country_native_language_name { get; set; }

    public string nationality { get; set; } = null!;

    public string country_code { get; set; } = null!;

    public string country_iso3_code { get; set; } = null!;

    public string country_iso2_code { get; set; } = null!;

    public string? time_zone { get; set; }

    public string? gmt_offset { get; set; }

    public decimal? latitude { get; set; }

    public decimal? longitude { get; set; }

    public string? phone_code { get; set; }

    public string? internet_code { get; set; }

    public string? flag { get; set; }

    public string? google_map_link { get; set; }

    public string? wiki_link { get; set; }

    public string? vehicle_code { get; set; }

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    //public virtual ICollection<IndividualNationality> IndividualNationalities { get; set; } = new List<IndividualNationality>();

    //public virtual ICollection<State> States { get; set; } = new List<State>();

    //public virtual Continent guid_continent { get; set; } = null!;
}


public class Country_view
{
    public Guid guid_country_id { get; set; }
    public String country_name { get; set; }
    public String phone_code { get; set; }
    
}

public class Nationality_view {
    public Guid guid_country_id { get; set; }
    public String nationality { get; set; }
    public String country_code { get; set; }


}