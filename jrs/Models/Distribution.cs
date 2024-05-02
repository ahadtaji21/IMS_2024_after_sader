using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class Distribution
{
    public Guid guid_distribution_id { get; set; }

    public string distribution_title { get; set; } = null!;

    public Guid guid_data_section_id { get; set; }

    public Guid distribution_commodity_type_id { get; set; }

    public Guid guid_distribution_type { get; set; }

    public DateTime distribution_date { get; set; }

    public string? distribution_notes { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? updated_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public bool? is_active { get; set; }

    // public virtual ICollection<DistributionHousehold> DistributionHouseholds { get; set; } = new List<DistributionHousehold>();

    // public virtual ICollection<DistributionIndividual> DistributionIndividuals { get; set; } = new List<DistributionIndividual>();

    // public virtual DistributionCommodityType distribution_commodity_type { get; set; } = null!;

    // public virtual DataSection guid_data_section { get; set; } = null!;

    // public virtual DistributionType guid_distribution_typeNavigation { get; set; } = null!;
}


public partial class DistributionView
{
    public Guid guid_distribution_id { get; set; }

    public string distribution_title { get; set; } = null!;
}