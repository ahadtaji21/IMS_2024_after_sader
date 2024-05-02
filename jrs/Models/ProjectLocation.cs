using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class ProjectLocation
{
    public Guid guid_project_location_id { get; set; }

    public Guid? guid_project_id { get; set; }

    public Guid? guid_city_id { get; set; }

    public string? neighborhood { get; set; }

    public int? guid_neighborhood_id { get; set; }

    public string project_address { get; set; } = null!;

    public bool? is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public int? updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    // public virtual ICollection<DataSectionDetail> DataSectionDetails { get; set; } = new List<DataSectionDetail>();

    // public virtual Project? guid_project { get; set; }
}



public partial class ProjectLocationView
{
    public Guid guid_project_location_id { get; set; }

    public Guid? guid_project_id { get; set; }

    public Guid? guid_city_id { get; set; }
}