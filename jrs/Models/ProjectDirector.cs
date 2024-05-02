﻿using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class ProjectDirector
{
    public Guid guid_project_director_id { get; set; }

    public int project_director_id { get; set; }

    public Guid? guid_project_id { get; set; }

    public int project_id { get; set; }

    public int personal_id { get; set; }

    public DateTime start_date { get; set; }

    public DateTime? end_date { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    // public virtual Project? guid_project { get; set; }
}


