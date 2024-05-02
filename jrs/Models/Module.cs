using jrs.Models;
using System;
using System.Collections.Generic;

namespace ScaffoldModels.Models;

public partial class Module
{
    public int module_id { get; set; }

    public string? module_name { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();

    public virtual ICollection<RoleModule> RoleModules { get; set; } = new List<RoleModule>();
}
