using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class DistributionCommodityType
{
    public Guid distribution_commodity_type_id { get; set; }

    public Guid? parent_distribution_commodity_type_id { get; set; }

    public string distribution_commodity_type { get; set; } = null!;

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public bool? is_active { get; set; }

    // public virtual ICollection<Distribution> Distributions { get; set; } = new List<Distribution>();

    // public virtual ICollection<DistributionCommodityType> Inverseparent_distribution_commodity_type { get; set; } = new List<DistributionCommodityType>();

    // public virtual DistributionCommodityType? parent_distribution_commodity_type { get; set; }
}
