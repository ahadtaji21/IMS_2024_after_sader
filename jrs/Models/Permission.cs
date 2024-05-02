using System;
using System.Collections.Generic;

namespace ScaffoldModels.Models;

public partial class Permission
{
    public int permission_id { get; set; }

    public string? permission1 { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
