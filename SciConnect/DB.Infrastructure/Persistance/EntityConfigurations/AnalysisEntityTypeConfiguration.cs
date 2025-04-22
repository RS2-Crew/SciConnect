using DB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.Infrastructure.Persistance.EntityConfigurations
{
    public class AnalysisEntityTypeConfiguration : IEntityTypeConfiguration<Analysis>
    {
        public void Configure(EntityTypeBuilder<Analysis> builder)
        {
            builder.ToTable("Analysis");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Konfiguracija relacije Analysis <-> Institutions (N:M)
            builder.HasMany(a => a.Institutions)
                .WithMany(i => i.Analyses)
                .UsingEntity<Dictionary<string, object>>(
                    "AnalysisInstitution",
                    j => j
                        .HasOne<Institution>()
                        .WithMany()
                        .HasForeignKey("institution_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Analysis>()
                        .WithMany()
                        .HasForeignKey("analysis_id")
                        .OnDelete(DeleteBehavior.Cascade)
                );
        }
    }
}
