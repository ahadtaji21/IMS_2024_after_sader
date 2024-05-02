using System;
using System.Collections.Generic;

namespace ScaffoldModels.Models;

public partial class Cluster
{
    public int cluster_id { get; set; }

    public string clustor_name { get; set; } = null!;

    public string cluster_code { get; set; } = null!;

    public string? description { get; set; }

    public bool? is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }
}
