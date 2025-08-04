using EventBus.Messages.Entities;
using IdentityServer.Entities;
using IdentityService.Services;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Consumers.EntityCreated
{
    public class EmployeeCreatedConsumer : IConsumer<EmployeeCreatedEvent>
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<User> _userManager;

        public EmployeeCreatedConsumer(IEmailSender emailSender, UserManager<User> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public async Task Consume(ConsumeContext<EmployeeCreatedEvent> context)
        {
            var message = context.Message;

            var allUsers = await _userManager.Users.ToListAsync();
            var adminsAndPMs = new List<User>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Administrator") || roles.Contains("PM"))
                {
                    adminsAndPMs.Add(user);
                }
            }

            foreach (var user in adminsAndPMs)
            {
                string subject = "New Employee Added";
                string body = $"A new employee has been created:<br><br>" +
                              $"<b>First Name:</b> {message.FirstName}<br>" +
                              $"<b>Last Name:</b> {message.LastName}<br>" +
                              $"<b>Institution:</b> {message.InstitutionName}<br>";

                await _emailSender.SendAsync(user.Email, subject, body);
            }
        }
    }
}
