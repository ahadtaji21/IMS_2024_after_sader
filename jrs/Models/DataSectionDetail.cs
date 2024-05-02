using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class DataSectionDetail
{
    public Guid guid_data_section_detail_id { get; set; }

    public Guid? guid_data_section_id { get; set; }

    public Guid? guid_project_location_id { get; set; }

    public Guid? guid_objective_id { get; set; }

    public DateTime? start_date { get; set; }

    public DateTime? end_date { get; set; }

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    public virtual DataSection? guid_data_section { get; set; }

    public virtual ProjectLocation? guid_project_location { get; set; }
}


