using System;
using System.Collections.Generic;

namespace jrs.Models
{
    /// <summary>
    /// Class defining the required payload for generic excel creation based on SP.
    /// </summary>

    public class ProcedureParameter
    {
        public string name { get; set; }
        public string type { get; set; }
        public object value { get; set; }

    }
    public class SqlProcedureParameter
    {
        public string name { get; set; }
        public object value { get; set; }

    }
    public class ExcelStoredProcedurePayload
    {
        public string PROCEDURE_NAME { get; set; }
        public string PROCEDURE_SCHEMA { get; set; }
        public ProcedureParameter[] PROCEDURE_PARAMETERS { get; set; }
        public SqlProcedureParameter[] SQL_PROCEDURE_PARAMETERS { get; set; }

    }


}