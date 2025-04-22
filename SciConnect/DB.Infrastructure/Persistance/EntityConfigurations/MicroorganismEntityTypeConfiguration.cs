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
    public class MicroorganismEntityTypeConfiguration : IEntityTypeConfiguration<Microorganism>
    {
        public void Configure(EntityTypeBuilder<Microorganism> builder)
        {

            // TODO Configure
            builder.ToTable("Microorganism");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                .ValueGeneratedOnAdd();

            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Konfiguracija relacije Microorganisms <-> Institutions (N:M) / samo za testiranje
            // TODO: Ovde treba Microorganisms <-> Analisys (N,M)
            builder.HasMany(i => i.Institutions)
                .WithMany(i => i.Microorganisms)
                .UsingEntity<Dictionary<string, object>>(
                    "InstitutionMicroorganism",
                    j => j
                        .HasOne<Institution>()
                        .WithMany()
                        .HasForeignKey("institution_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Microorganism>()
                        .WithMany()
                        .HasForeignKey("microorganism_id")
                        .OnDelete(DeleteBehavior.Cascade)
                );
        }
    }
}
