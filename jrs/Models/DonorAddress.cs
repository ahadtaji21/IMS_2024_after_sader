using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Import this namespace

namespace jrs.Models;

public partial class DonorAddress
{
    public Guid guid_donor_address_id { get; set; }
    public Guid guid_donor_id { get; set; }
    public Guid guid_city_id { get; set; }
    public string donor_address { get; set; }
    public string country_name { get; set; }
    public string country_code { get; set; }
    public string state_name { get; set; }
    public string city_name { get; set; }


    public bool is_active { get; set; }
    public Guid? guid_created_by { get; set; }
    public Guid? userId { get; set; }

    public DateTime? created_date { get; set; }
    public Guid? guid_updated_by { get; set; }
    public DateTime? updated_date { get; set; }
}

