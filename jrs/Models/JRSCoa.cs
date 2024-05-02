using System;
using Microsoft.EntityFrameworkCore;
namespace jrs.Models
{
         public class JRSCoa
    {
        public Guid id { get; set; }
        public string PMS_JRS_COA_CODE { get; set; }
        public string PMS_JRS_COA_DESCRIPTION { get; set; }
        public string PMS_JRS_COA_TYPE { get; set; }
        public string PMS_JRS_COA_CATEGORY { get; set; }
        public DateTime PMS_JRS_COA_NAV_UPDATE_DATE { get; set; }
         public Int16 PMS_JRS_COA_ENABLED { get; set; }

    }
        public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<JRSCoa> jrsCoa { get; set; }
         public DbSet<NavCoa> navCoa { get; set; }
    }
}