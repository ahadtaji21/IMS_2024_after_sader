using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class DataSection
{
    public Guid guid_data_section_id { get; set; }

    public Guid? guid_data_serction_parent_id { get; set; }

    public bool? is_enterable { get; set; }

    public bool? is_educational { get; set; }

    public string data_section { get; set; } = null!;

    public DateTime? start_date { get; set; }

    public DateTime? end_date { get; set; }

    public string? description { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    // public virtual ICollection<DataSectionDetail> DataSectionDetails { get; set; } = new List<DataSectionDetail>();

    // public virtual ICollection<Distribution> Distributions { get; set; } = new List<Distribution>();

    // public virtual ICollection<HouseholdDataSection> HouseholdDataSections { get; set; } = new List<HouseholdDataSection>();
}


public partial class DataSectionView
{
    public Guid guid_data_section_id { get; set; }

    public string data_section { get; set; } = null!;
}