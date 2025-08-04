using EventBus.Messages.Entities;
using IdentityServer.Entities;
using IdentityService.Services;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Consumers.EntityCreated
{
    public class InstitutionCreatedConsumer : IConsumer<InstitutionCreatedEvent>
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<User> _userManager;

        public InstitutionCreatedConsumer(IEmailSender emailSender, UserManager<User> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public async Task Consume(ConsumeContext<InstitutionCreatedEvent> context)
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
                var subject = $"[SciConnect] New Institution Created";
                var body = $"<b>{message.Name}</b> was just added.<br/>" +
                           $"📍 {message.Street} {message.StreetNumber}, {message.City}, {message.Country}<br/>" +
                           $"📞 {message.Phone} | ✉️ {message.Email}<br/>" +
                           $"🔗 Website: {message.Website}";

                await _emailSender.SendAsync(user.Email, subject, body);
            }
        }
    }
}
