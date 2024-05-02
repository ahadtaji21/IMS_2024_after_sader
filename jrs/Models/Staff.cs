using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Import this namespace

namespace jrs.Models
{
	public partial class Staff
	{
         
        public int page { get; set; }
        public int limit { get; set; }
        public SearchQueryBody[] searchBody { get; set; }
        public Guid guid_staff_id { get; set; }
		public Guid? guid_native_language_id { get; set; }
		public string staff_code { get; set; }
		public string staff_first_name { get; set; }
		public string staff_last_name { get; set; }
		public string jrs_entry_date { get; set; }
		public string objective_code { get; set; }
		public string staff_first_name_native { get; set; }
		public string staff_last_name_native { get; set; }
		public string staff_father_name { get; set; }
		public string staff_mother_name { get; set; }
		public string staff_date_of_birth { get; set; }
        public string Age { get; set; }
        public string gender { get; set; }
		public string staff_place_of_birth { get; set; }
		public int staff_gender_id { get; set; }
		public bool is_religious { get; set; }
		public int? blood_type_id { get; set; }
		public bool staff_is_active { get; set; }
		public Guid? userId { get; set; }
		public byte[] staff_photo { get; set; }
		public byte[] document_file { get; set; }
		//public bool is_active { get; set; }
		public Guid? guid_created_by { get; set; }
		public string created_date { get; set; }
		public Guid? guid_updated_by { get; set; }
		public DateTime? updated_date { get; set; }
		public Guid guid_staff_nationality_id { get; set; }
		public Guid? guid_staff_marital_status_id { get; set; }
		public Guid? guid_country_id { get; set; }
		public Guid? guid_state_id { get; set; }
		public Guid? guid_city_id { get; set; }
		public bool is_native { get; set; }
		public string city_name { get; set; }
		public Guid? guid_district_id { get; set; }
		public Guid? guid_marital_status_id { get; set; }
		public Guid? guid_contact_type_id { get; set; }
		public Guid? guid_staff_address_id { get; set; }
		public Guid? guid_neighborhood_id { get; set; }
		public Guid? guid_staff_educational_level_id { get; set; }
		public Guid? guid_educational_level_id { get; set; }
		public Guid? guid_staff_document_id { get; set; }
		public Guid? guid_document_type_id { get; set; }
		public string staff_contact_values { get; set; }
		public string staff_address_details { get; set; }
		public string document_notes { get; set; }
		public string document_number { get; set; }
		public bool nationality_is_active { get; set; }
		public string? nationality { get; set; }
		public Guid guid_staff_contact_id { get; set; }
		public string? contact_type { get; set; }
		public bool contact_is_active { get; set; }
		public string? marital_status { get; set; }
		public bool marital_is_active { get; set; }
		public string state_name { get; set; }
		public string country_name { get; set; }
		public string neighborhood_name { get; set; }
		public bool address_is_active { get; set; }
		public string educational_level { get; set; }
		public bool education_is_active { get; set; }
		public string document_type { get; set; }
		public string document_start_date { get; set; }
		public string document_end_date { get; set; }
		public bool document_is_active { get; set; }
		public Guid guid_staff_datasection_id { get; set; }
		public Guid guid_data_section_id { get; set; }
		public string data_section { get; set; }
		public string datasection_start_date { get; set; }
		public string datasection_end_date { get; set; }
		public bool datasection_is_active { get; set; }
		 
	}

    public partial class StaffView
    {
        public Guid guid_staff_id { get; set; }
        public string staff_code { get; set; }
        public string staff_first_name { get; set; }
        public string staff_last_name { get; set; }
        public string jrs_entry_date { get; set; }
        public string city_name { get; set; }
        public string objective_code { get; set; }
        public Guid guid_data_section_id { get; set; }
        public Guid? userId { get; set; }
    }
    public partial class StaffSearchQueryBody
    {
        public Guid guid_data_section_id { get; set; }
        public string jrs_entry_date { get; set; }
        public string city_name { get; set; }
        public string objective_code { get; set; }
        public string search_key { get; set; }
        public string search_query { get; set; }
        public Guid? userId { get; set; }

    }
    public partial class StaffPaginationBody
    {
        public Guid guid_staff_id { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public int office_id { get; set; }
        public Guid guid_data_section_id { get; set; }
        public string jrs_entry_date { get; set; }
        public string city_name { get; set; }

        public string objective_code { get; set; }
        public Guid? user_id { get; set; }

        public SearchQueryBody[] searchBody { get; set; }

    }
    public partial class GenderStatistics
    {
        public Guid guid_data_section_id { get; set; }
        public int genderCount { get; set; }
        public string gender_category { get; set; }
    }
    public partial class StaffStatistics_Country
    {
        public Guid guid_data_section_id { get; set; }
        public int staffCount { get; set; }
        public string staffcountry_name { get; set; }
    }
    public partial class Staff_Statistics
    {
        public List<GenderStatistics> genderStatistics { get; set; }
        public List<StaffStatistics_Country> staffStatistics_Country { get; set; }


    }
    public partial class StaffGetData
    {
        public int? IsActiveTrue { get; set; }
        public Guid guid_staff_id { get; set; }
        public Guid? guid_native_language_id { get; set; }
        public string staff_code { get; set; }
        public string staff_first_name { get; set; }
        public string staff_last_name { get; set; }
        public string jrs_entry_date { get; set; }
        public string objective_code { get; set; }
        //public DateTime? jrs_entry_date { get; set; }
        public string staff_first_name_native { get; set; }
        public string staff_last_name_native { get; set; }
        public string staff_father_name { get; set; }
        public string staff_mother_name { get; set; }
        //public DateTime? staff_date_of_birth { get; set; }
        public string? staff_date_of_birth { get; set; }
        public string staff_place_of_birth { get; set; }
        public int staff_gender_id { get; set; }
        public bool is_religious { get; set; }
        public int? blood_type_id { get; set; }
        public bool staff_is_active { get; set; }
        public Guid? userId { get; set; }
        public List<StaffGetNationality> StaffGetNationality { get; set; }
        public List<StaffGetContactType> StaffGetContactType { get; set; }
        public List<StaffGetMaritalStatus> StaffGetMaritalStatus { get; set; }
        public List<StaffGetAddress> StaffGetAddress { get; set; }
        public List<StaffGetEducationalLevel> StaffGetEducationalLevel { get; set; }
        public List<StaffGetDocument> StaffGetDocument { get; set; }
        public List<StaffGetDataSection> StaffGetDataSection { get; set; }
    }
    public partial class StaffGetNationality
    {
        public int? IsActiveTrue { get; set; }
        public SearchQueryBody[] searchBody { get; set; }
        public int page { get; set; }
        public int limit { get; set; }

		public Guid guid_staff_id { get; set; }
		public Guid guid_staff_nationality_id { get; set; }
		public Guid guid_country_id { get; set; }
		public bool is_native { get; set; }
		public bool nationality_is_active { get; set; }
		public string? created_date { get; set; }
		public string? nationality { get; set; }
		public Guid? userId { get; set; }
	}
	public partial class StaffGetContactType
	{
        public int? IsActiveTrue { get; set; }
        public SearchQueryBody[] searchBody { get; set; }
		public int page { get; set; }
		public int limit { get; set; }
		public Guid guid_staff_id { get; set; }
		public Guid guid_staff_contact_id { get; set; }
		public Guid guid_contact_type_id { get; set; }
		public string? staff_contact_values { get; set; }
		public string? contact_type { get; set; }
		public bool contact_is_active { get; set; }
		public string? created_date { get; set; }
		public Guid? userId { get; set; }
	}
	public partial class StaffGetMaritalStatus
	{
        public int? IsActiveTrue { get; set; }
        public SearchQueryBody[] searchBody { get; set; }
		public int page { get; set; }
		public int limit { get; set; }
		public Guid guid_staff_id { get; set; }
		public Guid guid_staff_marital_status_id { get; set; }
		public Guid guid_marital_status_id { get; set; }
		public string? marital_status { get; set; }
		public bool marital_is_active { get; set; }
		public string? created_date { get; set; }
		public Guid? userId { get; set; }
	}
	public partial class StaffGetAddress
	{
        public int? IsActiveTrue { get; set; }
        public SearchQueryBody[] searchBody { get; set; }
		public int page { get; set; }
		public int limit { get; set; }
		public Guid guid_staff_id { get; set; }
		public Guid guid_staff_address_id { get; set; }
		public Guid guid_city_id { get; set; }
		public Guid guid_state_id { get; set; }
		public Guid guid_country_id { get; set; }
		public Guid guid_neighborhood_id { get; set; }
		public string city_name { get; set; }
		public string staff_address_details { get; set; }
		public string state_name { get; set; }
		public string country_name { get; set; }
		public string neighborhood_name { get; set; }
		public bool address_is_active { get; set; }
		public string? created_date { get; set; }
		public Guid? userId { get; set; }
	}
	public partial class StaffGetEducationalLevel
	{
        public int? IsActiveTrue { get; set; }
        public SearchQueryBody[] searchBody { get; set; }
		public int page { get; set; }
		public int limit { get; set; }
		public Guid guid_staff_id { get; set; }
		public Guid guid_staff_educational_level_id { get; set; }
		public Guid guid_educational_level_id { get; set; }
		public string educational_level { get; set; }
		public bool education_is_active { get; set; }
		public string? created_date { get; set; }
		public Guid? userId { get; set; }
	}
	public partial class StaffGetDocument
	{
        public int? IsActiveTrue { get; set; }
        public SearchQueryBody[] searchBody { get; set; }
		public int page { get; set; }
		public int limit { get; set; }
		public Guid guid_staff_id { get; set; }
		public Guid guid_staff_document_id { get; set; }
		public Guid guid_document_type_id { get; set; }
		public string document_type { get; set; }
		public string document_file { get; set; }
		public string document_notes { get; set; }
		public string document_number { get; set; }
		public string document_start_date { get; set; }
		public string document_end_date { get; set; }
		public bool document_is_active { get; set; }
		public string? created_date { get; set; }
		public Guid? userId { get; set; }
	}
	public partial class StaffGetDataSection
	{
		public SearchQueryBody[] searchBody { get; set; }
		public int page { get; set; }
		public int limit { get; set; }
		public Guid guid_staff_id { get; set; }
		public Guid guid_staff_datasection_id { get; set; }
		public Guid guid_data_section_id { get; set; }
		public string data_section { get; set; }
		public string objective_code { get; set; }
		public string datasection_start_date { get; set; }
		public string datasection_end_date { get; set; }
		public bool datasection_is_active { get; set; }
		public string? created_date { get; set; }
		public Guid? userId { get; set; }
        public int? IsActiveTrue { get; set; }
    }
}
