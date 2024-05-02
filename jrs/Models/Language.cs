using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace jrs.Models;


[NotMapped]
public partial class Language
{
}


[NotMapped]
public partial class LanguageView
{
    public Guid guid_language_id { get; set; }

    public string language_name { get; set; }
}