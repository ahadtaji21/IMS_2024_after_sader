using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class JrsRegionCountry
{
    public int JRS_region_country_id { get; set; }

    public Guid guid_JRS_region_country_id { get; set; }

    public int JRS_region_id { get; set; }

    public Guid guid_jrs_region_id { get; set; }

    public int country_id { get; set; }

    public Guid guid_country_id { get; set; }

    public bool? is_regional_office { get; set; }

    public DateTime? start_date { get; set; }

    public DateTime? end_date { get; set; }

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    // public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    // public virtual JrsRegion guid_jrs_region { get; set; } = null!;
}


public partial class JrsRegionCountryView
{
    public Guid guid_JRS_region_country_id { get; set; }
    public string region_country_name { get; set; }
}
