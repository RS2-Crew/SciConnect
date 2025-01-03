using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.Data.EntityTypeConfiguration
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Name = "Guest",
                    NormalizedName = "Users"
                },
                new IdentityRole
                {
                    Name = "Administrator",
                    NormalizedName = "Admin"
                },
                new IdentityRole
                {
                    Name = "PM",
                    NormalizedName = "PackageManager"
                }
            );
        }
    }
}
