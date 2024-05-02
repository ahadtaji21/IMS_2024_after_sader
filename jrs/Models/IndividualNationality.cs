using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class IndividualNationality
{
    public Guid guid_individual_nationality_id { get; set; }

    public Guid guid_individual_id { get; set; }

    public Guid guid_country_id { get; set; }
    public string native_nationality { get; set; }
    public Guid? userId { get; set; }

    public bool is_native { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }

}

public partial class IndividualNationalityView
{
    public Guid guid_individual_nationality_id { get; set; }

    public Guid guid_individual_id { get; set; }

    public Guid guid_country_id { get; set; }

    public bool is_native { get; set; }
}