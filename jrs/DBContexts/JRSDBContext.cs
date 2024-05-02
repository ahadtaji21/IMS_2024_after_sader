using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jrs.DBContexts
{
    public class JRSDBContext : DbContext
    {
        public JRSDBContext(DbContextOptions<JRSDBContext> options) : base(options)
        {

        }
        public DbSet<Test> Tests { get; set; }

        //List of departments
        public DbSet<TestDepartment> TestDepartments { get; set; }
    }
}
