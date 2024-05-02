using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class EducationalLevel
{
    public Guid guid_educational_level_id { get; set; }

    public string? educational_level { get; set; }

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    // public virtual ICollection<IndividualEducationalLevel> IndividualEducationalLevels { get; set; } = new List<IndividualEducationalLevel>();
}



public partial class EducationalLevelView
{
    public Guid guid_educational_level_id { get; set; }

    public string? educational_level { get; set; }
}