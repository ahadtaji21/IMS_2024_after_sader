using System;
using System.Collections.Generic;

namespace ScaffoldModels.Models;

public partial class RoleModule
{
    public Guid guid_role_module_id { get; set; }

    public int? module_id { get; set; }

    public Guid? guid_role_id { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual Role? guid_role { get; set; }

    public virtual Module? module { get; set; }
}
