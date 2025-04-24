using DB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.Infrastructure.Persistance.EntityConfigurations
{
    public class EmployeeEntityTypeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employee");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            // Jedan Employee pripada jednoj Institution (1:1 ili N:1 ako više zaposlenih može u istoj ustanovi)
            builder
                .HasOne(e => e.Institution)
                .WithMany(i => i.Employees) 
                .HasForeignKey("institution_id")
                .OnDelete(DeleteBehavior.Restrict);


            // Konfiguracija relacije Instrument <-> Institutions (N:M)
            builder.HasMany(i => i.Keywords)
                .WithMany(i => i.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeKeyword",
                    j => j
                        .HasOne<Keyword>()
                        .WithMany()
                        .HasForeignKey("keyword_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Employee>()
                        .WithMany()
                        .HasForeignKey("employee_id")
                        .OnDelete(DeleteBehavior.Cascade)
                );
        }
    }
}
