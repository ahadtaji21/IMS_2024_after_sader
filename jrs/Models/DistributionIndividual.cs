using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class DistributionIndividual
{
    public Guid guid_distribution_individual_id { get; set; }

    public Guid? guid_distribution_id { get; set; }

    public Guid? guid_individual_id { get; set; }

    public bool? is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual Distribution? guid_distribution { get; set; }

    public virtual Individual? guid_individual { get; set; }
}


public partial class DistributionIndividualView
{
    public Guid guid_distribution_individual_id { get; set; }

    public Guid? guid_distribution_id { get; set; }

    public Guid? guid_individual_id { get; set; }
}