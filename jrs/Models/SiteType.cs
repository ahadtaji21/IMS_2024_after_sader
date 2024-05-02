using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class SiteType
{
    public Guid guid_site_type_id { get; set; }

    public int site_type_id { get; set; }

    public string site_type_name { get; set; } = null!;

    public int? parent_id { get; set; }

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    // public virtual ICollection<Site> Sites { get; set; } = new List<Site>();
}
public partial class SiteTypeView
{
    public Guid guid_site_type_id { get; set; }

    public string site_type_name { get; set; } = null!;
}