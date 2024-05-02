using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Import this namespace

namespace jrs.Models;

public partial class Gender
{
    public int gender_id { get; set; }
    public string gender { get; set; } = null!;
    public string gender_code { get; set; } = null!;
    public string? description { get; set; }
    public bool is_active { get; set; }
    public Guid? guid_created_by { get; set; }
    public DateTime? created_date { get; set; }
    public Guid? guid_updated_by { get; set; }
    public DateTime? updated_date { get; set; }
}

public partial class GenderView
{
    public int gender_id { get; set; }
    public string gender { get; set; } = null!;
}