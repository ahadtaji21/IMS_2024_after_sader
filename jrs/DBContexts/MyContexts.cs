using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace jrs.DBContexts
{
    public partial class MyContexts : DbContext
    {
        public MyContexts()
        {
        }

        public MyContexts(DbContextOptions<MyContexts> options)
            : base(options)
        {
        }

        public virtual DbSet<Test> Test { get; set; }
        public virtual DbSet<TestDepartment> TestDepartment { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=JRSDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Test>(entity =>
            {
                entity.ToTable("TEST");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Descr)
                    .HasColumnName("DESCR")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<TestDepartment>(entity =>
            {
                entity.ToTable("TEST_DEPARTMENT");

                entity.Property(e => e.TestDepartmentId).HasColumnName("TEST_DEPARTMENT_ID");

                entity.Property(e => e.TestDepartmentDescr)
                    .IsRequired()
                    .HasColumnName("TEST_DEPARTMENT_DESCR")
                    .HasMaxLength(2500);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
