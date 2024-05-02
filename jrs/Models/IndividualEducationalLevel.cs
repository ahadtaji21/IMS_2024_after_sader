using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class IndividualEducationalLevel
{
    public Guid guid_individual_educational_level_id { get; set; }

    public Guid? guid_educational_level_id { get; set; }

    public Guid? guid_individual_id { get; set; }

    public string? description { get; set; }
    public string? education_description { get; set; }
    public string? education_level_description { get; set; }
    public string? educational_level { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }
    public Guid? userId { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual EducationalLevel? guid_educational_level { get; set; }

    public virtual Individual? guid_individual { get; set; }
}
