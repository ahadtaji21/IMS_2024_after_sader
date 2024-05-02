using System;
using System.Collections.Generic;

namespace ScaffoldModels.Models;

public partial class Role
{
    public Guid guid_role_id { get; set; }

    public string? role_title { get; set; }

    public string? role_description { get; set; }

    public string? permission { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();

    public virtual ICollection<RoleModule> RoleModules { get; set; } = new List<RoleModule>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
