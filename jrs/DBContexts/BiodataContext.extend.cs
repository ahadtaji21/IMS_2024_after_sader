// using System;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata;
// using jrs.Models;

// namespace jrs.DBContexts{
//     public partial class BiodataContext : DbContext{
//         public virtual DbSet<ImsLookup> ImsLookup { get; set; }

//         partial void OnModelCreatingPartial(ModelBuilder modelBuilder){
//              modelBuilder.Entity<HrBiodata>(entity => {

//                 entity.Property(e => e.HrBiodataGenederLookupId)
//                     .HasColumnName("HR_BIODATA_GENEDER_LOOKUP_ID");

//                 entity.HasOne(e => e.LookupGender)
//                     .WithMany()
//                     .HasForeignKey(d => d.HrBiodataGenederLookupId)
//                     .HasConstraintName("FK__HR_BIODAT__HR_BI__2AD55B43");
//             });
//         }
//     }
// }

