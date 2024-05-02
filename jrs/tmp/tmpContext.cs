using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace jrs.tmp
{
    public partial class tmpContext : DbContext
    {
        public tmpContext()
        {
        }

        public tmpContext(DbContextOptions<tmpContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PmsCategoryOfIntervention> PmsCategoryOfIntervention { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Password=jrs001!;Persist Security Info=True;User ID=jrsu;Initial Catalog=jrsdb;Data Source=185.56.9.137,1434");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PmsCategoryOfIntervention>(entity =>
            {
                entity.HasKey(e => e.PmsCoiId);

                entity.ToTable("PMS_CATEGORY_OF_INTERVENTION");

                entity.Property(e => e.PmsCoiId).HasColumnName("PMS_COI_ID");

                entity.Property(e => e.PmsCoiCode)
                    .HasColumnName("PMS_COI_CODE")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PmsCoiDeleted).HasColumnName("PMS_COI_DELETED");

                entity.Property(e => e.PmsCoiDescriptionLabelId).HasColumnName("PMS_COI_DESCRIPTION_LABEL_ID");

                entity.Property(e => e.PmsCoiEnabled).HasColumnName("PMS_COI_ENABLED");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
