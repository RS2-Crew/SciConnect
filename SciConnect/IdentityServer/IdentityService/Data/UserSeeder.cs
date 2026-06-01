using IdentityServer.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Data
{
    public class UserSeeder
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserSeeder> _logger;

        public UserSeeder(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger<UserSeeder> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            await SeedUserAsync(
                username: "Sloba98",
                email: "sloba98@sciconnect.com",
                firstName: "Slobodan",
                lastName: "Admin",
                password: "Srbija1389!",
                role: "Administrator"
            );

            await SeedUserAsync(
                username: "SlobaCar635",
                email: "slobacar635@gmail.com",
                firstName: "Slobodan",
                lastName: "Jovanovic",
                password: "Srbija1389!",
                role: "PM"
            );

            await SeedUserAsync(
                username: "MilanR",
                email: "milan.radovic@sciconnect.com",
                firstName: "Milan",
                lastName: "Radović",
                password: "Srbija1389!",
                role: "Administrator"
            );

            await SeedUserAsync(
                username: "JelenaM",
                email: "jelena.m@sciconnect.com",
                firstName: "Jelena",
                lastName: "Marković",
                password: "Srbija1389!",
                role: "Guest"
            );

            await SeedUserAsync(
                username: "NikolaS",
                email: "nikola.s@sciconnect.com",
                firstName: "Nikola",
                lastName: "Stojanović",
                password: "Srbija1389!",
                role: "Guest"
            );
        }

        private async Task SeedUserAsync(string username, string email, string firstName, string lastName, string password, string role)
        {
            try
            {
                if (await _roleManager.FindByNameAsync(role) == null)
                {
                    _logger.LogWarning("{Role} role not found. Make sure roles are seeded first.", role);
                    return;
                }

                var existingUser = await _userManager.FindByNameAsync(username);
                if (existingUser != null)
                {
                    _logger.LogInformation("User {Username} already exists. Skipping.", username);
                    return;
                }

                var user = new User
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);
                    _logger.LogInformation("Seeded {Role} user: {Username}", role, username);
                }
                else
                {
                    _logger.LogError("Failed to create {Username}: {Errors}", username,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding user {Username}.", username);
            }
        }
    }
}
