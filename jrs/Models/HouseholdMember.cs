using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class HouseholdMember
{
    public Guid guid_household_member_id { get; set; }

    public Guid guid_household_id { get; set; }

    public Guid guid_individual_id { get; set; }

    public int relationships_type_id { get; set; }

    public bool is_owner { get; set; }

    public bool? is_orphan { get; set; }
    public bool? owner_same_as_provider { get; set; }
    public string jrs_household_identifier { get; set; }
    public string relationships_type { get; set; }
    public string original_country { get; set; }
    public string household_provider { get; set; }
    public string provider_gender { get; set; }
    public string provider_relationship_type { get; set; }
    public DateTime file_open_date { get; set; }
    public int household_provider_relation_type_id { get; set; }
    public int household_provider_gender_id { get; set; }
    public Guid? guid_original_country_id { get; set; }


    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }
    public Guid? userId { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }


    public DateTime? updated_date { get; set; }
}



public partial class HouseholdMemberView
{
    public Guid guid_household_member_id { get; set; }

    public Guid guid_household_id { get; set; }

    public Guid guid_individual_id { get; set; }


}