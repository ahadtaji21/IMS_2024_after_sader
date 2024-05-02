using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Import this namespace

namespace jrs.Models;

public partial class GrantStatus
{
    public Guid guid_grant_status_id { get; set; }
    public string grant_status { get; set; }
    public string grant_status_code { get; set; }
    public string grant_status_color { get; set; }

    public string grant_status_description { get; set; }
    public int grant_status_order { get; set; }

    public bool is_active { get; set; }
    public Guid? guid_created_by { get; set; }
    public DateTime? created_date { get; set; }
    public Guid? guid_updated_by { get; set; }
    public DateTime? updated_date { get; set; }
}

public partial class GrantStatusView
{
    public Guid guid_grant_status_id { get; set; }
    public string grant_status { get; set; }
  

}