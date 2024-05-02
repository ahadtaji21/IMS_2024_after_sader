using System.Collections.Generic;

namespace jrs.Models{
    public partial class ImsStatus{
        public int imsStatusId { get; set; }
        // public int imsStatusStatusClassId { get; set; }
        public string imsStatusName { get; set; }
        public string ImsStatusDescr { get; set; }
        public string? ImsStatusCode { get; set; }
    }
}