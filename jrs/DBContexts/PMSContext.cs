using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using jrs.Models;

namespace jrs.DBContexts
{
    public partial class PMSContext : DbContext
    {
        public PMSContext()
        {
        }

        public PMSContext(DbContextOptions<PMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PmsCategoryOfIntervention> PmsCategoryOfIntervention { get; set; }
        public virtual DbSet<PmsCoiTosRel> PmsCoiTosRel { get; set; }
        public virtual DbSet<PmsTypeOfService> PmsTypeOfService { get; set; }

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

            modelBuilder.Entity<PmsCoiTosRel>(entity =>
            {
                entity.HasKey(e => e.PmsCoiTosId);

                entity.ToTable("PMS_COI_TOS_REL");

                entity.Property(e => e.PmsCoiTosId).HasColumnName("PMS_COI_TOS_ID");

                entity.Property(e => e.PmsCoiId).HasColumnName("PMS_COI_ID");

                entity.Property(e => e.PmsTosId).HasColumnName("PMS_TOS_ID");



                //entity.HasOne(d => d.PmsCoi)
                //    .WithMany(p => p.PmsCoiTosRel)
                //    .HasForeignKey(d => d.PmsCoiId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_PMS_COI_TOS_REL_PMS_COI_TOS_REL");

                //entity.HasOne(d => d.PmsTos)
                //    .WithMany(p => p.PmsCoiTosRel)
                //    .HasForeignKey(d => d.PmsTosId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_PMS_COI_TOS_REL_PMS_TYPE_OF_SERVICE");
            });

            modelBuilder.Entity<PmsTypeOfService>(entity =>
            {
                entity.HasKey(e => e.PmsTosId);

                entity.ToTable("PMS_TYPE_OF_SERVICE");

                entity.Property(e => e.PmsTosId).HasColumnName("PMS_TOS_ID");

                entity.Property(e => e.PmsTosCode)
                    .HasColumnName("PMS_TOS_CODE")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PmsTosDeleted).HasColumnName("PMS_TOS_DELETED");

                entity.Property(e => e.PmsTosDescription)
                    .HasColumnName("PMS_TOS_DESCRIPTION")
                    .IsUnicode(false);

                entity.Property(e => e.PmsTosEnabled)
                    .IsRequired()
                    .HasColumnName("PMS_TOS_ENABLED")
                    .HasDefaultValueSql("((1))");
            });
            modelBuilder.Entity<PmsCategoryOfIntervention>().HasQueryFilter(p => !p.PmsCoiDeleted);
            modelBuilder.Entity<PmsTypeOfService>().HasQueryFilter(p => !p.PmsTosDeleted);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
