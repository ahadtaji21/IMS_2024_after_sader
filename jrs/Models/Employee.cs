using System;
using System.Collections.Generic;

namespace jrs.Models;

public partial class Employee
{
    public Guid guid_employee_id { get; set; }

    public string? first_name { get; set; }

    public string? middle_name { get; set; }

    public string? surname { get; set; }

    public string? first_name_native { get; set; }

    public string? middle_name_native { get; set; }

    public string? surname_native { get; set; }

    public string? father_name_native { get; set; }

    public string? mother_name_native { get; set; }

    public int? gender_id { get; set; }

    public DateTime? birth_date { get; set; }

    public string? birth_place { get; set; }

    public Guid? nationality_id { get; set; }

    public bool? religious { get; set; }

    public string? photo_profile { get; set; }

    public int? blood_type_id { get; set; }

    public bool is_active { get; set; }

    public Guid? guid_created_by { get; set; }

    public DateTime? created_date { get; set; }

    public Guid? guid_updated_by { get; set; }

    public DateTime? updated_date { get; set; }
}
