using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.Infrastructure.Persistance.EntityConfigurations
{
    public class InstitutionEntityTypeConfiguration : IEntityTypeConfiguration<Institution>
    {
        public void Configure(EntityTypeBuilder<Institution> builder)
        {
            builder.ToTable("Institution");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).UseHiLo("institutionseq");

            //builder.Property(o => o.Id).UseIdentityColumn();

            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Street)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.StreetNumber)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(o => o.City)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.Country)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Phone)
                .HasMaxLength(20);

            builder.Property(o => o.Email)
                .HasMaxLength(50);

            builder.Property(o => o.Website)
                .HasMaxLength(100);

            builder.HasIndex(o => o.Name).IsUnique();

            builder.HasMany(i => i.Instruments)
            .WithMany(i => i.Institutions)
            .UsingEntity<Dictionary<string, object>>(
                "UstanovaInstrument",
                j => j
                    .HasOne<Instrument>()
                    .WithMany()
                    .HasForeignKey("instrument_id")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Institution>()
                    .WithMany()
                    .HasForeignKey("ustanova_id")
                    .OnDelete(DeleteBehavior.Cascade)
            );

        }
    }
}
