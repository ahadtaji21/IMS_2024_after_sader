using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class HouseholdAddress
{
    public Guid guid_houshold_address_id { get; set; }

    public Guid guid_household_id { get; set; }
    public string country_name { get; set; }
    public string state_name { get; set; }
    public string city_name { get; set; }
    public string neighborhood_name { get; set; }

    public Guid guid_city_id { get; set; }
    public Guid guid_individual_id { get; set; }


    public Guid guid_country_id { get; set; }
    public Guid guid_state_id { get; set; }
    public Guid guid_neighborhood_id { get; set; }


    public Guid guid_address_property_type_id { get; set; }
    public string address_property_type { get; set; }

    public Guid guid_address_status_type_id { get; set; }
    public string address_status_type { get; set; }

    public bool is_native_address { get; set; }

    public decimal? rent_value { get; set; }

    public string household_address { get; set; } = null!;

    public string? address_notes { get; set; }
    public int total_rooms { get; set; }

    public string? house_mate { get; set; }
    public string mobileNumber  { get; set; }
    public string mobileNumber2  { get; set; }
    public string email { get; set; }

    public Guid? guid_district_id { get; set; }

    public Guid? guid_block_id { get; set; }

    public decimal? latitude { get; set; }

    public decimal? longitude { get; set; }

    public bool is_active { get; set; }


    public Guid? guid_created_by { get; set; }
    public Guid? userId { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    //public virtual AddressPropertyType guid_address_property_type { get; set; } = null!;

    //public virtual AddressStatusType guid_address_status_type { get; set; } = null!;

    //public virtual Household guid_household { get; set; } = null!;

    //public virtual Neighborhood guid_neighborhood { get; set; } = null!;
}


public partial class HouseholdAddressView
{
    public Guid guid_houshold_address_id { get; set; }

    public Guid guid_household_id { get; set; }
    public Guid guid_address_property_type_id { get; set; }
}