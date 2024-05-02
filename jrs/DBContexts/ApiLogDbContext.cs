using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jrs.DBContexts
{
    /// <summary>
    /// Class to handle api call log
    /// </summary>
    public class ApiLogDbContext : DbContext
    {
        public ApiLogDbContext(DbContextOptions<ApiLogDbContext> options) : base(options)
        {
            
        }
        /// <summary>
        /// log table
        /// </summary>
        public DbSet<JRSApiLog> JRSApiLogs { get; internal set; }
    }
}
