using DB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.Infrastructure.Persistance.EntityConfigurations
{
    public class KeywordEntityTypeConfiguration : IEntityTypeConfiguration<Keyword>
    {
        public void Configure(EntityTypeBuilder<Keyword> builder)
        {
            // TODO Configure
            builder.ToTable("Keyword");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                .ValueGeneratedOnAdd();

            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Konfiguracija relacije Keywords <-> Institutions (N:M) / samo za testiranje
            // TODO: Ovde treba odnos Keywords <-> Employee (N,M)
            builder.HasMany(i => i.Institutions)
                .WithMany(i => i.Keywords)
                .UsingEntity<Dictionary<string, object>>(
                    "InstitutionKeyword",
                    j => j
                        .HasOne<Institution>()
                        .WithMany()
                        .HasForeignKey("institution_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Keyword>()
                        .WithMany()
                        .HasForeignKey("keyword_id")
                        .OnDelete(DeleteBehavior.Cascade)
                );
        }
    }
}