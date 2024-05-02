using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class Project
{
    public Guid guid_project_id { get; set; }

    public Guid? guid_JRS_region_country_id { get; set; }

    public Guid? guid_site_id { get; set; }

    public int project_id { get; set; }

    public int? site_id { get; set; }

    public Guid? guid_parent_project_id { get; set; }

    public int? parent_project_id { get; set; }

    public string project_name { get; set; } = null!;

    public string project_code { get; set; } = null!;

    public bool? is_international { get; set; }

    public bool? is_regional { get; set; }

    public bool? is_country { get; set; }

    public string? project_description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    // public virtual ICollection<Project> Inverseguid_parent_project { get; set; } = new List<Project>();

    // public virtual ICollection<Objective> Objective1s { get; set; } = new List<Objective>();

    // public virtual ICollection<ProjectDetail> ProjectDetails { get; set; } = new List<ProjectDetail>();

    // public virtual ICollection<ProjectDirector> ProjectDirectors { get; set; } = new List<ProjectDirector>();

    // public virtual ICollection<ProjectLocation> ProjectLocations { get; set; } = new List<ProjectLocation>();

    // public virtual JrsRegionCountry? guid_JRS_region_country { get; set; }

    // public virtual Project? guid_parent_project { get; set; }
}


public partial class ProjectView
{
    public Guid guid_project_id { get; set; }

    public Guid? guid_JRS_region_country_id { get; set; }

    public string project_name { get; set; } = null!;
}