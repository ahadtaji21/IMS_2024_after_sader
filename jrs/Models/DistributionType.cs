using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class DistributionType
{
    public Guid guid_distribution_type { get; set; }

    public string distribution_type { get; set; } = null!;

    public string? notes { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public bool? is_active { get; set; }

    public virtual ICollection<Distribution> Distributions { get; set; } = new List<Distribution>();
}


public partial class DistributionTypeView
{
    public Guid guid_distribution_type { get; set; }

    public string distribution_type { get; set; } = null!;
}