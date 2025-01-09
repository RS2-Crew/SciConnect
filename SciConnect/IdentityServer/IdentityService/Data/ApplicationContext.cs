using IdentityServer.Entities;
using IdentityService.Data.EntityTypeConfiguration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new RoleConfiguration());

            var pmUserId1 = Guid.NewGuid().ToString();

            var hasher = new PasswordHasher<User>();
            builder.Entity<User>().HasData(
                new User
                {
                    Id = pmUserId1,
                    UserName = "pm1@example.com",
                    NormalizedUserName = "PM1@EXAMPLE.COM",
                    Email = "pm1@example.com",
                    NormalizedEmail = "PM1@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "PMPassword123!"),
                    FirstName = "PM",
                    LastName = "User"
                }
            );
        }
    }
}
