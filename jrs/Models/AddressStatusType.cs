using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class AddressStatusType
{
    public Guid guid_address_status_type_id { get; set; }

    public string address_status_type { get; set; } = null!;

    public string? address_status_type_code { get; set; }

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    //public virtual ICollection<HouseholdAddress> HouseholdAddresses { get; set; } = new List<HouseholdAddress>();
}

public class AddressStatusTypeView{
    
    public Guid guid_address_status_type_id { get; set; }

    public string address_status_type { get; set; } = null!; 
}