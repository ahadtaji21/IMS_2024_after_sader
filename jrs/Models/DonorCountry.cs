using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Import this namespace

namespace jrs.Models;

public partial class DonorCountry
{
    public Guid guid_donor_country_id { get; set; }
    public Guid guid_JRS_region_country_id { get; set; }
    public Guid guid_donor_id { get; set; }
    public Guid guid_country_id { get; set; }
    public DateTime donor_country_start_date { get; set; }
    public DateTime donor_country_end_date { get; set; }
    public string donor_country_description { get; set; }
    public string country_name { get; set; }
    public string country_code { get; set; }
    public string JRS_region_code { get; set; }

    

    public bool is_active { get; set; }
    public Guid? guid_created_by { get; set; }
    public Guid? userId { get; set; }
    public DateTime? created_date { get; set; }
    public Guid? guid_updated_by { get; set; }
    public DateTime? updated_date { get; set; }
}

