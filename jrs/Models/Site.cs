using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class Site
{
    public Guid guid_site_id { get; set; }

    public int site_id { get; set; }

    public int? organization_id { get; set; }

    public int? site_type_id { get; set; }

    public Guid? guid_site_type_id { get; set; }

    public string site_name { get; set; } = null!;

    public int? parent_id { get; set; }

    public string? site_description { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public int? updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public bool? is_active { get; set; }

    public virtual SiteType? guid_site_type { get; set; }
}

public partial class SiteView
{
    public Guid guid_site_id { get; set; }

    public string site_name { get; set; } = null!;
}