using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class MaritalStatus
{
    public Guid guid_marital_status_id { get; set; }

    public int marital_status_id { get; set; }

    public string? marital_status { get; set; }

    public string? marital_status_code { get; set; }

    public string? description { get; set; }

    public bool? is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    //public virtual ICollection<IndividualMaritalStatus> IndividualMaritalStatuses { get; set; } = new List<IndividualMaritalStatus>();
}


public partial class MaritalStatusView
{
    public Guid guid_marital_status_id { get; set; }
    public string? marital_status { get; set; }

}
