using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace jrs.Models;

    
    public class WizardModel
    {
        public Guid wizard_guid { get; set; }
        public Guid? guid_data_section_id { get; set; }
        public String firstName { get; set; }
        public int gender { get; set; }
        public String lastName { get; set; }
        public String countryOrigin { get; set; }
        public String fatherName { get; set; }
        public DateTime?  dateOfBirth { get; set; }
        public String motherName { get; set; }
        public Guid? maritalStatus { get; set; }
        public Guid? familyStatus { get; set; }
        public Guid? specialNeeds { get; set; }
        public String relationType { get; set; }
        public string individualDocumentType { get; set; }
        public string familyDocumentType { get; set; }
        public Guid? educationLevel { get; set; }
        public String educationLvelTxt { get; set; }
        public Guid? country { get; set; }
        public Guid? state { get; set; }
        public Guid? city { get; set; }
        public string neighborhood { get; set; }
        public string district { get; set; }
        public double rentValue { get; set; }
        public string block { get; set; }
        public int totalRooms { get; set; }
        public string addressDetails { get; set; }
        public string originalAddress { get; set; }
        public Guid? propertyType { get; set; }
        public Guid? livingStatus { get; set; }
        public string mobileNumber { get; set; }
        public string mobileNumber2 { get; set; }
        public string email { get; set; }
        public string notes { get; set; }
        public string isPregnant { get; set; }
        public string isChronic { get; set; }
        public string countryCode { get; set; }



}
