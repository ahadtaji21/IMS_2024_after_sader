using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class StateType
{
    public Guid guid_state_type_id { get; set; }

    public int state_type_id { get; set; }

    public string state_type_name { get; set; } = null!;

    public bool is_active { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? updated_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public string? description { get; set; }

    public virtual ICollection<State> States { get; set; } = new List<State>();
}


public partial class StateTypeView
{
    public Guid guid_state_type_id { get; set; }
    public string state_type_name { get; set; } = null!;
}