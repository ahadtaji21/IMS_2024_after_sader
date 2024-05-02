using System;
using System.Collections.Generic;

namespace jrs.Models
{
    public partial class ImsLogerrors
    {
        public long Id { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public DateTime? InsertDate { get; set; }
        public string ServerName { get; set; }
        public string Ip { get; set; }
        public string UserId { get; set; }
        public string Parameters { get; set; }
        public string Class { get; set; }
        public string Procedure { get; set; }
    }
}
