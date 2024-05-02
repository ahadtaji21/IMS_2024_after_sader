using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using jrs.Models;
namespace jrs.DBContexts
{
    public partial class IMSContext : DbContext
    {
        public IMSContext()
        {
        }

        public IMSContext(DbContextOptions<IMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ImsLabels> ImsLabels { get; set; }
        public virtual DbSet<ImsLanguage> ImsLanguage { get; set; }

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
            modelBuilder.Entity<ImsLabels>(entity =>
            {
                entity.ToTable("IMS_LABELS");

                entity.HasIndex(e => new { e.ImsLabelsTableName, e.ImsLabelsNumber, e.ImsLabelsLanguageId })
                    .HasName("unique_table_number_language")
                    .IsUnique();

                entity.Property(e => e.ImsLabelsId).HasColumnName("IMS_LABELS_ID");

                entity.Property(e => e.ImsLabelsLanguageId).HasColumnName("IMS_LABELS_LANGUAGE_ID");

                entity.Property(e => e.ImsLabelsNumber)
                    .HasColumnName("IMS_LABELS_NUMBER")
                    .HasComment("Groupes Labels for one element over multiple languages. This is what FKs will reference.");

                entity.Property(e => e.ImsLabelsTableName)
                    .IsRequired()
                    .HasColumnName("IMS_LABELS_TABLE_NAME")
                    .HasMaxLength(50);

                entity.Property(e => e.ImsLabelsValue)
                    .IsRequired()
                    .HasColumnName("IMS_LABELS_VALUE");

                entity.HasOne(d => d.ImsLabelsLanguage)
                    .WithMany(p => p.ImsLabels)
                    .HasForeignKey(d => d.ImsLabelsLanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__IMS_LABEL__IMS_L__1E6F845E");
            });

            modelBuilder.Entity<ImsLanguage>(entity =>
            {
                entity.ToTable("IMS_LANGUAGE");

                entity.Property(e => e.ImsLanguageId).HasColumnName("IMS_LANGUAGE_ID");

                entity.Property(e => e.ImsLanguageFamily)
                    .HasColumnName("IMS_LANGUAGE_FAMILY")
                    .HasMaxLength(500);

                entity.Property(e => e.ImsLanguageIso6391Code)
                    .IsRequired()
                    .HasColumnName("IMS_LANGUAGE_ISO_639_1_CODE")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.ImsLanguageLocale)
                    .HasColumnName("IMS_LANGUAGE_LOCALE")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.ImsLanguageName)
                    .IsRequired()
                    .HasColumnName("IMS_LANGUAGE_NAME")
                    .HasMaxLength(500);

                entity.Property(e => e.ImsLnaguageNotes)
                    .HasColumnName("IMS_LNAGUAGE_NOTES")
                    .HasMaxLength(2000);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
