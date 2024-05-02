using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace jrs.Models;

public partial class Pregnant
{
    
    public Guid guid_pregnant_id { get; set; }
    public Guid guid_individual_id { get; set; }
    public DateTime pregnancy_start_date { get; set; }
    public string? pregnant_description { get; set; }
    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

 

}



