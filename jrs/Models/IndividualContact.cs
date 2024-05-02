using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class IndividualContact
{
    public Guid guid_individual_contact_id { get; set; }

    public Guid guid_individual_id { get; set; }

    public Guid guid_contact_type_id { get; set; }

    public string contact_value { get; set; } = null!;
    public string contact_type { get; set; } = null!;

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }
    public Guid userId { get; set; }

    public int? updated_by { get; set; }

    public DateTime? updated_date { get; set; }

    // public virtual ContactType guid_contact_type { get; set; } = null!;

    // public virtual Individual guid_individual { get; set; } = null!;
}



public partial class IndividualContactView
{
    public Guid guid_individual_contact_id { get; set; }

    public Guid guid_individual_id { get; set; }

    public Guid guid_contact_type_id { get; set; }

    public string contact_value { get; set; } = null!;
}