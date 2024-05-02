using System;
using System.Collections.Generic;

namespace ScaffoldModels.Models;

public partial class RoleMenu
{
    public Guid guid_role_menu_id { get; set; }

    public Guid? guid_role_id { get; set; }

    public Guid? guid_menu_id { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual Role? guid_role { get; set; }
}
