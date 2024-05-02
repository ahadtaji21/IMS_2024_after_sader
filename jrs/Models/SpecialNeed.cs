using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class SpecialNeed
{
    public Guid guid_special_need_id { get; set; }
    public Guid guid_special_need_type_id { get; set; }
    public string guid_special_need_type_ids { get; set; }

    public Guid guid_individual_id { get; set; }

    public string special_need_description { get; set; } = null!;
    public string special_need_type { get; set; } = null!;



    public bool? is_active { get; set; }

    public Guid? guid_created_by { get; set; }
    public Guid? userId { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }
}

