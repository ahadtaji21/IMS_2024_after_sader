using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using jrs.Models;

namespace jrs.DBContexts
{
    public partial class IMSLogContext : DbContext
    {
        public IMSLogContext()
        {
        }

        public IMSLogContext(DbContextOptions<IMSLogContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ImsLogerrors> ImsLogerrors { get; set; }
        public virtual DbSet<ImsLogevents> ImsLogevents { get; set; }

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
            modelBuilder.Entity<ImsLogerrors>(entity =>
            {
                entity.ToTable("IMS_LOGERRORS");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Class)
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.Property(e => e.InsertDate).HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(15)
                    .IsFixedLength();

                entity.Property(e => e.Parameters).IsUnicode(false);

                entity.Property(e => e.Procedure)
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.Property(e => e.ServerName).HasMaxLength(255);

                entity.Property(e => e.Source)
                    .HasMaxLength(255)
                    .IsFixedLength();

                entity.Property(e => e.StackTrace).HasColumnType("text");

                entity.Property(e => e.UserId)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<ImsLogevents>(entity =>
            {
                entity.ToTable("IMS_LOGEVENTS");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Class)
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.Property(e => e.InsertDate).HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(15)
                    .IsFixedLength();

                entity.Property(e => e.Message).HasColumnType("text");

                entity.Property(e => e.Parameters).IsUnicode(false);

                entity.Property(e => e.Procedure)
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
