using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Import this namespace

namespace jrs.Models;

public partial class DonorCategory
{
    public Guid guid_donor_category_id { get; set; }
    public string donor_category { get; set; }
    public string donor_category_description { get; set; }
    public Guid? guid_parent_donor_category_id { get; set; }
    public bool is_active { get; set; }
    public Guid? guid_created_by { get; set; }
    public DateTime? created_date { get; set; }
    public Guid? guid_updated_by { get; set; }
    public DateTime? updated_date { get; set; }
}

public partial class DonorCategoryView
{
    public Guid guid_donor_category_id { get; set; }
    public string donor_category { get; set; } = null!;
}