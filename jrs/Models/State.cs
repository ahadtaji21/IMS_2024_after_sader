using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class State
{
    public Guid guid_state_id { get; set; }

    public int state_id { get; set; }

    public string state_name { get; set; } = null!;

    public string? iso_code { get; set; }

    public int country_id { get; set; }

    public Guid guid_country_id { get; set; }

    public Guid? guid_state_type_id { get; set; }

    public int state_type_id { get; set; }

    public int? parent_id { get; set; }

    public double? latitude { get; set; }

    public double? longitude { get; set; }

    public string? phone_code { get; set; }

    public string? native_language { get; set; }

    public string? postal_code { get; set; }

    public bool? is_capital { get; set; }

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    // public virtual ICollection<City> Cities { get; set; } = new List<City>();

    // public virtual Country guid_country { get; set; } = null!;

    // public virtual StateType? guid_state_type { get; set; }
}


public partial class StateView
{
    public Guid guid_state_id { get; set; }

    public string state_name { get; set; } = null!;

    public Guid guid_country_id { get; set; }
}