namespace jrs.Models
{
    public class PmsAnnualPlan
    {


        public int Id { get; set; }
        public string apcode { get; set; }
        public string office_id { get; set; }
         public string office_code { get; set; }
        public string code { get; set; }
        public string code_id { get; set; }
        public string descr { get; set; }
        public string location_descr { get; set; }
        public string admin_area_id { get; set; }
        public string admin_area_name { get; set; }
        public string start_year { get; set; }
        public string end_year { get; set; }
        public string status_id { get; set; }
        public string status_name { get; set; }
        public string currency_code { get; set; }
        public string location_id { get; set; }
        public string location_description { get; set; }
        public string activation_state  { get; set; }
        public string country  { get; set; }
         public string guid  { get; set; }
    }
}

