using System;
using System.Collections.Generic;

namespace jrs.Models
{
    public partial class HrBiodata
    {
        public int HrBiodataId { get; set; }
        public Guid? HrBiodataUserUid { get; set; }
        // public int? HrBiodataPersonalTitleId { get; set; }
        public string HrBiodataSurname { get; set; }
        public string HrBiodataMiddleName { get; set; }
        public string HrBiodataName { get; set; }
        // public int HrBiodataGenderId { get; set; }
        public DateTime HrBiodataBirthDate { get; set; }
        public string HrBiodataBirthPlace { get; set; }
        public string HrBiodataNationality { get; set; }
        public bool HrBiodataReligious { get; set; }
        public int? HrBiodataPermanentLocationId { get; set; }
        public string HrBiodataIdentificationDocuments { get; set; }
        public string HrBiodataRegnumber { get; set; }
        public byte[] HrBiodataPhoto { get; set; }
        public int? HrBiodataGenederLookupId { get; set; }
        public int? HrBiodataPersonalTitleLookupId { get; set; }

        // public virtual HrGender HrBiodataGender { get; set; }
        // public virtual HrPersonalTitle HrBiodataPersonalTitle { get; set; }
        public virtual ImsLookup LookupGender { get; set; }
        public virtual ImsLookup LookupPersonalTtitle { get; set; }
    }


    public partial class HrBiodataStatistics_Gender
    {
        public int count { get; set; }
        public string gender { get; set; }
        public int year { get; set; }
    }
    public partial class HrBiodataStatistics_Count
    {
        public int count { get; set; }
    }
    public partial class HrBiodataStatistics_Departments
    {
        public int count { get; set; }
        public string department_name { get; set; }
    }
    public partial class HrBiodataStatistics
    {
        public List<HrBiodataStatistics_Gender> hrBiodataStatistics_Gender { get; set; }
        public List<HrBiodataStatistics_Count> hrBiodataStatistics_Count { get; set; }
        public List<HrBiodataStatistics_Departments> hrBiodataStatistics_Departments { get; set; }

    }
}
