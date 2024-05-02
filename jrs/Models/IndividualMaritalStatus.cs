using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class IndividualMaritalStatus
{
    public Guid guid_individual_marital_status_id { get; set; }

    public Guid guid_individual_id { get; set; }

    public Guid guid_marital_status_id { get; set; }
    public string marital_status { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }
    public Guid? userId { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    // public virtual Individual guid_individual { get; set; } = null!;

}


public partial class IndividualMaritalStatusView
{
    public Guid guid_individual_marital_status_id { get; set; }

    public Guid guid_individual_id { get; set; }

    public Guid guid_marital_status_id { get; set; }
}