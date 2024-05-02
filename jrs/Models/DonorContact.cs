using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Import this namespace

namespace jrs.Models;

public partial class DonorContact
{
    public Guid guid_donor_contact_id { get; set; }
    public Guid guid_donor_id { get; set; }
    public Guid guid_contact_type_id { get; set; }

    public string donor_contact { get; set; }
    public string contact_type { get; set; }
 
    public bool is_active { get; set; }
    public Guid? guid_created_by { get; set; }
    public Guid? userId { get; set; }
    public DateTime? created_date { get; set; }
    public Guid? guid_updated_by { get; set; }
    public DateTime? updated_date { get; set; }
}

