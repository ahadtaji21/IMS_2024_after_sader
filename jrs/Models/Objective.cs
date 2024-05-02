using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class Objective
{
    public Guid guid_objective_id { get; set; }

    public Guid guid_navision_objective_id { get; set; }

    public int objective_id { get; set; }

    public Guid? guid_project_id { get; set; }

    public int? project_id { get; set; }

    public string objective_code { get; set; } = null!;

    public DateTime? start_date { get; set; }

    public DateTime? end_date { get; set; }

    public string? description { get; set; }

    public bool? is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual ICollection<ObjectiveCategoryIntervention> ObjectiveCategoryInterventions { get; set; } = new List<ObjectiveCategoryIntervention>();

    public virtual Project? guid_project { get; set; }
}


public partial class ObjectiveView
{
    public Guid guid_objective_id { get; set; }

    public Guid? guid_project_id { get; set; }

    public string objective_code { get; set; } = null!;
}