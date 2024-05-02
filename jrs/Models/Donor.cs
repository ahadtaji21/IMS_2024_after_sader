using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Import this namespace

namespace jrs.Models;

public partial class Donor
{
    public Guid guid_donor_id { get; set; }
    public string donor_name { get; set; }
    public string donor_description { get; set; }
    public string donor_code { get; set; }

    public Guid? guid_donor_category_id { get; set; }
    public Guid? guid_navision_id { get; set; }
    public string donor_number { get; set; }
    public string donor_category { get; set; }

    public bool is_active { get; set; }
    public Guid? guid_created_by { get; set; }
    public Guid? userId { get; set; }
    public DateTime? created_date { get; set; }
    public Guid? guid_updated_by { get; set; }
    public DateTime? updated_date { get; set; }
}

public partial class DonorView
{
    public Guid guid_donor_id { get; set; }
    public string donor_name { get; set; }
    public string donor_code { get; set; }
  

}
public partial class SearchQueryBody {
    public string search_key { get; set; }
    public string search_query { get; set; }

}
public partial class PaginationBody
{
    public Guid guid_donor_id { get; set; }
    public int page { get; set; }
    public int limit { get; set; }
    public int office_id { get; set; }
    public Guid? user_id { get; set; }
    public SearchQueryBody[] searchBody { get; set; }

}
public partial class Donor_Add_Body {

    public String donor_name { get; set; }
    public String donor_description { get; set; }
    public String donor_code { get; set; }
    public Guid guid_donor_category_id { get; set; }
    public String donor_number { get; set; }
    public Guid guid_contact_type_id { get; set; }
    public String donor_contact { get; set; }
    public Guid guid_JRS_region_country_id { get; set; }
    public DateTime donor_country_start_date { get; set; }
    public DateTime donor_country_end_date { get; set; }
    public String donor_country_description { get; set; }
    public Guid guid_city_id { get; set; }
    public String donor_address { get; set; }
    public Guid? guid_created_by { get; set; }
    public Guid? userId { get; set; }

}


public partial class DonorStatistics_Category
{
    public int donnorsCount { get; set; }
    public string donor_category { get; set; }
}


public partial class DonorStatistics_Country
{
    public int donnorsCount { get; set; }
    public string country_name { get; set; }
}



public partial class Donor_Statistics { 
    public List<DonorStatistics_Category> donorStatistics_Category { get; set; }
    public List<DonorStatistics_Country> donorStatistics_Country { get; set; }
   
    
}