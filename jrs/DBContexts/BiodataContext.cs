using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using jrs.Models;

namespace jrs.DBContexts
{
    public partial class BiodataContext : DbContext
    {
        public BiodataContext()
        {
        }

        public BiodataContext(DbContextOptions<BiodataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<HrBiodata> HrBiodata { get; set; }
        public virtual DbSet<HrGender> HrGender { get; set; }
        public virtual DbSet<HrPersonalTitle> HrPersonalTitle { get; set; }

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
            modelBuilder.Entity<HrBiodata>(entity =>
            {
                entity.ToTable("HR_BIODATA");

                entity.Property(e => e.HrBiodataId).HasColumnName("HR_BIODATA_ID");

                entity.Property(e => e.HrBiodataBirthDate)
                    .HasColumnName("HR_BIODATA_BIRTH_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.HrBiodataBirthPlace)
                    .IsRequired()
                    .HasColumnName("HR_BIODATA_BIRTH_PLACE")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                // entity.Property(e => e.HrBiodataGenderId).HasColumnName("HR_BIODATA_GENDER_ID");

                entity.Property(e => e.HrBiodataGenederLookupId).HasColumnName("HR_BIODATA_GENEDER_LOOKUP_ID");

                entity.Property(e => e.HrBiodataIdentificationDocuments)
                    .HasColumnName("HR_BIODATA_IDENTIFICATION_DOCUMENTS")
                    .HasComment("JSON list of objects with identification document information : doc. type, doc. number, doc. expiry date");

                entity.Property(e => e.HrBiodataMiddleName)
                    .HasColumnName("HR_BIODATA_MIDDLE_NAME")
                    .HasMaxLength(150);

                entity.Property(e => e.HrBiodataName)
                    .IsRequired()
                    .HasColumnName("HR_BIODATA_NAME")
                    .HasMaxLength(150);

                entity.Property(e => e.HrBiodataNationality)
                    .HasColumnName("HR_BIODATA_NATIONALITY")
                    .HasComment("JSON list of objects with nationality information");

                entity.Property(e => e.HrBiodataPermanentLocationId).HasColumnName("HR_BIODATA_PERMANENT_LOCATION_ID");

                // entity.Property(e => e.HrBiodataPersonalTitleId).HasColumnName("HR_BIODATA_PERSONAL_TITLE_ID");

                entity.Property(e => e.HrBiodataPhoto).HasColumnName("HR_BIODATA_PHOTO");

                entity.Property(e => e.HrBiodataRegnumber)
                    .HasColumnName("HR_BIODATA_REGNUMBER")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("Internal JRS registration code");

                entity.Property(e => e.HrBiodataReligious).HasColumnName("HR_BIODATA_RELIGIOUS");

                entity.Property(e => e.HrBiodataSurname)
                    .IsRequired()
                    .HasColumnName("HR_BIODATA_SURNAME")
                    .HasMaxLength(150);

                entity.Property(e => e.HrBiodataUserUid).HasColumnName("HR_BIODATA_USER_UID");

                // entity.HasOne(d => d.HrBiodataGender)
                //     .WithMany(p => p.HrBiodata)
                //     .HasForeignKey(d => d.HrBiodataGenderId)
                //     .OnDelete(DeleteBehavior.ClientSetNull)
                //     .HasConstraintName("FK__HR_BIODAT__HR_BI__498EEC8D");

                // entity.HasOne(d => d.HrBiodataPersonalTitle)
                //     .WithMany(p => p.HrBiodata)
                //     .HasForeignKey(d => d.HrBiodataPersonalTitleId)
                //     .HasConstraintName("FK__HR_BIODAT__HR_BI__489AC854");

                entity.Property(e => e.HrBiodataGenederLookupId)
                    .HasColumnName("HR_BIODATA_GENEDER_LOOKUP_ID");

                entity.Property( e => e.HrBiodataPersonalTitleLookupId)
                    .HasColumnName("HR_BIODATA_PERSONAL_TITLE_LOOKUP_ID");

                entity.HasOne(e => e.LookupGender)
                    .WithMany()
                    .HasForeignKey(d => d.HrBiodataGenederLookupId)
                    .HasConstraintName("FK__HR_BIODAT__HR_BI__2AD55B43");
                
                entity.HasOne( e => e.LookupPersonalTtitle)
                    .WithMany()
                    .HasForeignKey( e => e.HrBiodataPersonalTitleLookupId);
            });

            modelBuilder.Entity<HrGender>(entity =>
            {
                entity.ToTable("HR_GENDER");

                entity.Property(e => e.HrGenderId).HasColumnName("HR_GENDER_ID");

                entity.Property(e => e.HrGenderCode)
                    .IsRequired()
                    .HasColumnName("HR_GENDER_CODE")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.HrGenderDescr)
                    .IsRequired()
                    .HasColumnName("HR_GENDER_DESCR")
                    .HasMaxLength(25)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HrPersonalTitle>(entity =>
            {
                entity.ToTable("HR_PERSONAL_TITLE");

                entity.Property(e => e.HrPersonalTitleId).HasColumnName("HR_PERSONAL_TITLE_ID");

                entity.Property(e => e.HrPersonalTitleDescr)
                    .IsRequired()
                    .HasColumnName("HR_PERSONAL_TITLE_DESCR")
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
