using EventBus.Messages.Entities;
using IdentityServer.Entities;
using IdentityService.Services;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Consumers.EntityCreated
{
    public class SimpleEntityCreatedConsumer : IConsumer<SimpleEntityCreatedEvent>
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<SimpleEntityCreatedConsumer> _logger;

        public SimpleEntityCreatedConsumer(IEmailSender emailSender, UserManager<User> userManager, ILogger<SimpleEntityCreatedConsumer> logger)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SimpleEntityCreatedEvent> context)
        {
            var message = context.Message;
            if (message == null) return;


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

            _logger.LogInformation($"Broadcasting entity creation to {adminsAndPMs.Count} Admins/PMs: {message.EntityType} - {message.Name}");


            string subject = $"New {message.EntityType} Created";
            string body = $"A new {message.EntityType.ToLower()} has been created:<br><br>" +
                          $"<b>Name:</b> {message.Name}<br>";

            foreach (var user in adminsAndPMs)
            {
                await _emailSender.SendAsync(user.Email, subject, body);
            }
        }
    }
}
