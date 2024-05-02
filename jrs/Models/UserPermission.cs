using jrs.Models;
using System;
using System.Collections.Generic;

namespace ScaffoldModels.Models;

public partial class UserPermission
{
    public Guid guid_user_permission_id { get; set; }

    public Guid? guid_user_id { get; set; }

    public int? permission_id { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual User? guid_user { get; set; }

    public virtual Permission? permission { get; set; }
}
