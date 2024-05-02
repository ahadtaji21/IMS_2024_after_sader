using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class AddressPropertyType
{
    public Guid guid_address_property_type_id { get; set; }

    public string address_property_type { get; set; } = null!;

    public string? address_property_type_code { get; set; }

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

}

public class AddressPropertyTypeView {
    public Guid guid_address_property_type_id { get; set; }

    public string address_property_type { get; set; } = null!;   
}
