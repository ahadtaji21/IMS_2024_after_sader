using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jrs.DBContexts;

namespace jrs.Models
{
    public partial class ImsLookup
    {
        public int ImsLookupId { get; set; }
        public int ImsLookupLookupTypeId { get; set; }
        public string ImsLookupCode { get; set; }
        public string ImsLookupValue { get; set; }

        public virtual ImsLookupType ImsLookupLookupType { get; set; }
        public virtual ICollection<SfipActivitySchedule> SfipActivitySchedules { get; set; }

        ///<summary>
        /// Returns a list of lookups based on the LookupType code
        ///</summary>
        /// <param name="typeCode">String of the type of lookups to recover.</param>
        /// <param name="cntx">DbContext to use to query the DB.</param>
        public static async Task<List<ImsLookup>> GetLookupsForType(string typeCode, GeneralContext cntx)
        {
            var _context = cntx;

            var lookups = await _context.ImsLookup
                .Include(l => l.ImsLookupLookupType)
                .Where(l => Equals(l.ImsLookupLookupType.ImsLookupTypeCode, typeCode))
                .ToListAsync();

            return lookups;
        }

        ///<summary>
        /// Returns a list of lookups based on an Array of LookupType codes
        ///</summary>
        /// <param name="typeCodes">Array of strings of the types of lookups to recover.</param>
        /// <param name="cntx">DbContext to use to query the DB.</param>
        public static async Task<List<ImsLookup>> GetLookupsForType(string[] typeCodes, GeneralContext cntx)
        {
            var _context = cntx;

            var lookups = await _context.ImsLookup
                .Include(l => l.ImsLookupLookupType)
                .Where(l => typeCodes.Contains(l.ImsLookupLookupType.ImsLookupTypeCode))
                .ToListAsync();

            return lookups;
        }
    }


    public  class ImsList
    {

        public string value { get; set; }
        public string text { get; set; }
    }

}
