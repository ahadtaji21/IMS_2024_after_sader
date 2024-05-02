using jrs.Models;
using System;
using System.Collections.Generic;

namespace ScaffoldModels.Models;

public partial class UserLevel
{
    public int user_level_id { get; set; }

    public string? user_level { get; set; }

    public string? user_level_description { get; set; }

    public Guid? guid_project_id { get; set; }

    public Guid? guid_site_id { get; set; }

    public Guid? guid_project_location_id { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
