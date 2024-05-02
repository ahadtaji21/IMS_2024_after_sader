using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class JrsRegion
{
    public int JRS_region_id { get; set; }

    public Guid guid_jrs_region_id { get; set; }

    public string JRS_region_name { get; set; } = null!;

    public string JRS_region_code { get; set; } = null!;

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual ICollection<JrsRegionCountry> JrsRegionCountries { get; set; } = new List<JrsRegionCountry>();
}
