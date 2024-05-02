using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class Household
{
    public Guid guid_household_id { get; set; }

    public string jrs_household_identifier { get; set; }

    public Guid guid_original_country_id { get; set; }

    public int household_total_members { get; set; }

    public string household_provider { get; set; } = null!;

    public int household_provider_gender_id { get; set; }

    public int household_provider_relation_type_id { get; set; }

    public DateTime? displacment_date { get; set; }

    public DateTime? file_open_date { get; set; }

    public string? household_notes { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }
    public Guid? userId { get; set; }

    public int? updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    //public virtual ICollection<CanceledHousehold> CanceledHouseholds { get; set; } = new List<CanceledHousehold>();

    //public virtual ICollection<DistributionHousehold> DistributionHouseholds { get; set; } = new List<DistributionHousehold>();

    //public virtual ICollection<HouseholdAddress> HouseholdAddresses { get; set; } = new List<HouseholdAddress>();

    //public virtual ICollection<HouseholdCaseVisit> HouseholdCaseVisits { get; set; } = new List<HouseholdCaseVisit>();

    //public virtual ICollection<HouseholdDataSection> HouseholdDataSections { get; set; } = new List<HouseholdDataSection>();

    //public virtual ICollection<HouseholdDocumentType> HouseholdDocumentTypes { get; set; } = new List<HouseholdDocumentType>();

    //public virtual ICollection<HouseholdMember> HouseholdMembers { get; set; } = new List<HouseholdMember>();
}


public partial class HouseholdView
{
    public Guid guid_household_id { get; set; }

    public string jrs_household_identifier { get; set; }
}