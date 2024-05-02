using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class ContactType
{
    public Guid guid_contact_type_id { get; set; }

    public string contact_type { get; set; } = null!;

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }


    public DateTime? updated_date { get; set; }

    public virtual ICollection<IndividualContact> IndividualContacts { get; set; } = new List<IndividualContact>();
}

public partial class ContactTypeView
{
    public Guid guid_contact_type_id { get; set; }

    public string contact_type { get; set; } = null!;
}