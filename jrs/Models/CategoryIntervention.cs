using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class CategoryIntervention
{
    public Guid guid_nav_category_intervention_id { get; set; }

    public int category_intervention_id { get; set; }

    public string category_intervention { get; set; } = null!;

    public string category_intervention_code { get; set; } = null!;

    public string? category_intervention_description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    //public virtual ICollection<ObjectiveCategoryIntervention> ObjectiveCategoryInterventions { get; set; } = new List<ObjectiveCategoryIntervention>();
}

public partial class CategoryInterventionView
{
    public Guid guid_nav_category_intervention_id { get; set; }


    public string category_intervention { get; set; } = null!;
}