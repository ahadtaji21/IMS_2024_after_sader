using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace jrs.Models;

public partial class Individual
{
    public Guid guid_individual_id { get; set; }
    public Guid guid_data_section_id { get; set; }



    public string individual_first_name { get; set; } = null!;

    public string individual_first_name_native_lang { get; set; } = null!;
        public string JRS_individual_identifier { get; set; }


    public string individual_last_name { get; set; } = null!;

    public string individual_last_name_native_lang { get; set; } = null!;

    public int gender_id { get; set; }
    
    public Guid? native_language_id { get; set; }

    public string? individual_father_name { get; set; }

    public string? individual_father_name_native_lang { get; set; }

    public string? individual_mother_name { get; set; }

    public string? individual_mother_name_native_lang { get; set; }


    public DateTime date_of_birth { get; set; }

    public string? place_of_birth { get; set; }

    public string? place_of_birth_native_lang { get; set; }

    public Guid native_nationality_id { get; set; }
    public Guid guid_native_nationality_id { get; set; }


    public int? blood_type_id { get; set; }

    public string? photo_path { get; set; }

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    
}


[NotMapped]
public partial class IndividualWizardClass
{
    
    public Guid? guid_individual_id { get; set; }
    public String JRS_individual_identifier  { get; set; }

    public Guid? guid_data_section_id { get; set; }
    public String individual_first_name { get; set; }
    public String individual_first_name_native_lang { get; set; }
    public String individual_last_name { get; set; }
    public String individual_last_name_native_lang { get; set; }
    public int gender_id { get; set; }
    public string gender { get; set; }

    public String individual_father_name { get; set; }
    public String individual_mother_name { get; set; }
    public DateTime? date_of_birth { get; set; }
    public Guid? guid_native_language_id { get; set; }

    public String place_of_birth { get; set; }
    public String place_of_birth_native_lang { get; set; }
    public Guid guid_native_nationality_id { get; set; }
    public string nationality { get; set; }
    public string document_number { get; set; }

    public Guid? guid_household_status_type_id { get; set; }
    public Guid? guid_individual_marital_status_id { get; set; }
    public int relationships_type_id { get; set; }

    public string individualDocumentType { get; set; }
    public string householdDocumentType { get; set; }
    public Guid? guid_educational_level_id { get; set; }
    public String education_level_description { get; set; }
    public Guid? guid_country_id { get; set; }
    public Guid? guid_state_id { get; set; }
    public Guid? guid_city_id { get; set; }
    public String? country_name { get; set; }
    
    public Guid? guid_neighborhood_id { get; set; }
    public string district { get; set; }
    public string block { get; set; }
    public string address_housemates { get; set; }
    public bool address_notes { get; set; }

    public int totalRooms { get; set; }
    public double rent_value { get; set; }
    public string household_address { get; set; }
    public string originalAddress { get; set; }
    
    public Guid? guid_address_property_type_id { get; set; }
    public Guid? guid_address_status_type_id { get; set; }
    public Guid? guid_block_id { get; set; }
    public Guid? guid_district_id { get; set; }
    public Guid? guid_marital_status_id { get; set; }
    public Guid? userId { get; set; }
    public string mobileNumber { get; set; }
    public string mobileNumber2 { get; set; }
    public string education_description { get; set; }
    public string email { get; set; }
    public string notes { get; set; }

    public string guid_special_need_type_id { get; set; }
    public string household_provider { get; set; }
    public int household_provider_gender_id { get; set; }
    public int household_provider_relation_type_id { get; set; }
    public bool is_chronic { get; set; }
    public bool is_pregnant { get; set; }
    public DateTime created_date { get; set; }
    public bool owner_same_as_provider { get; set; }
    public bool is_active { get; set; }
}


public class CombinedData
{
    public string TableName { get; set; }
    public List<Dictionary<string, object>> Data { get; set; }
}


public partial class Individual_PaginationBody
{
    public Guid guid_individual_id { get; set; }
    public Guid guid_data_section_id { get; set; }
    public int page { get; set; }
    public int limit { get; set; }
    public int office_id { get; set; }
    public Guid? user_id { get; set; }
    public Guid? userId { get; set; }
    public SearchQueryBody[] searchBody { get; set; }

}


public partial class IndividualStatistics_Gender
{
    public int individualsCount { get; set; }
    public string gender { get; set; }
}


public partial class Individual_Statistics
{
    public List<IndividualStatistics_Gender> individualStatistics_Gender { get; set; }

}


public partial class IndividualHouseholdDocumentType
{
    public string individualDocumentType { get; set; }
    public string householdDocumentType { get; set; }
    public Guid guid_individual_id { get; set; }
    public Guid userId { get; set; }
}