using System;
using System.Collections.Generic;

namespace jrs.Models
{
    public partial class Log
    {
        public long Id { get; set; }
        public DateTime RequestTime { get; set; }
        public long ResponseMillis { get; set; }
        public int Statuscode { get; set; }
        public string Headers { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string Requestbody { get; set; }
        public string Responsebody { get; set; }
        public string Servervariables { get; set; }
    }
}
