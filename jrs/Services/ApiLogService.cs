using jrs.DBContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jrs.Services
{
    public class ApiLogService
    {
        private readonly IConfiguration _config;

        public ApiLogService(IConfiguration config)
        {
            _config = config;
        }

        public async Task Log(JRSApiLog apiLogItem)
        {
            using (var db = new ApiLogDbContext(getDbConfig()))
            {
                db.JRSApiLogs.Add(apiLogItem);
                try
                {
                     await db.SaveChangesAsync();
                }
                catch (System.Exception)
                {
                    
                   // throw;
                }
               
            }
        }

        public async Task<IEnumerable<JRSApiLog>> Get()
        {
            using (var db = new ApiLogDbContext(getDbConfig()))
            {
                var items = from i in db.JRSApiLogs
                            orderby i.Id descending
                            select i;

                return await items.ToListAsync();
            }
        }
        private DbContextOptions<ApiLogDbContext> getDbConfig()
        {
            var dbContextBuilder = new DbContextOptionsBuilder<ApiLogDbContext>();

            dbContextBuilder.UseSqlServer(_config.GetConnectionString("jrsdb"));

            return dbContextBuilder.Options;
        }
    }
}
