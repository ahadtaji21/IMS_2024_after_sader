using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; 

namespace jrs.Models;

public partial class Errors
{
    public int error_id { get; set; }
    public Guid guid_user_id { get; set; }

    //A user-defined error message number stored in the sys.messages catalog view using sp_addmessage. Error numbers for user-defined error messages should be greater than 50000. When msg_id is not specified, RAISERROR raises an error message with an error number of 50000.
    public int error_number { get; set; }
    public int error_state { get; set; }

    //Severity levels from 20 through 25 are considered fatal
    //https://learn.microsoft.com/en-us/sql/relational-databases/errors-events/database-engine-error-severities?view=sql-server-ver16
    public int error_severity { get; set; }
    public int error_line { get; set; }
    public string error_procedure { get; set; }
    //A user-defined message 
    public string error_message { get; set; }
    public DateTime error_datetime { get; set; }



}
