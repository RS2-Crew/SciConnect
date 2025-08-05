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
    public class InstrumentEntityTypeConfiguration : IEntityTypeConfiguration<Instrument>
    {
        public void Configure(EntityTypeBuilder<Instrument> builder)
        {
            builder.ToTable("Instrument");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                .ValueGeneratedOnAdd();

            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Konfiguracija relacije Instrument <-> Institutions (N:M)
            builder.HasMany(i => i.Institutions)
                .WithMany(i => i.Instruments)
                .UsingEntity<Dictionary<string, object>>(
                    "InstitutionInstrument",
                    j => j
                        .HasOne<Institution>()
                        .WithMany()
                        .HasForeignKey("institution_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Instrument>()
                        .WithMany()
                        .HasForeignKey("instrument_id")
                        .OnDelete(DeleteBehavior.Cascade)
                );
        }
    }
}
