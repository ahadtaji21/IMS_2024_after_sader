using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class IndividualPregnancy
{
    public Guid guid_pregnant_id { get; set; }
    public Guid guid_individual_id { get; set; }
    public DateTime? pregnancy_start_date { get; set; }

   
    public Guid? guid_created_by { get; set; }
    public Guid? userId { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }
    public Boolean is_active { get; set; }

}
