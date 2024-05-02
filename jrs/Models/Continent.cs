using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class Continent
{
    public Guid guid_continent_id { get; set; }

    public int continent_id { get; set; }

    public string continent_name { get; set; } = null!;

    public string continent_code { get; set; } = null!;

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual ICollection<Country> Countries { get; set; } = new List<Country>();
}



public partial class ContinentView
{
    public Guid guid_continent_id { get; set; }


    public string continent_name { get; set; } = null!;
}