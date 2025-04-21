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
                .WithMany() 
                .OnDelete(DeleteBehavior.Restrict); 

            
            builder
                .HasMany(e => e.Keywords)
                .WithOne() 
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
