using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class SpecialNeedType
{
    public Guid guid_special_need_type_id { get; set; }

    public string special_need_type { get; set; } = null!;

    public string? special_need_type_code { get; set; }

    public string? special_need_type_description { get; set; }

    public bool? is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }
}


public partial class SpecialNeedView
{
    public Guid guid_special_need_type_id { get; set; }

    public string special_need_type { get; set; } = null!;
}