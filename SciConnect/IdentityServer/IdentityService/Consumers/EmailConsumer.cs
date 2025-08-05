using MassTransit;
using IdentityService.DTOs;
using IdentityService.Services;

namespace IdentityService.Consumers
{
    public class EmailConsumer : IConsumer<EmailMessage>
    {
        private readonly ILogger<EmailConsumer> _logger;
        private readonly IEmailSender _emailSender;

        public EmailConsumer(ILogger<EmailConsumer> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }


        //public EmailConsumer(ILogger<EmailConsumer> logger)
        //{
        //    _logger = logger;
        //    _logger.LogInformation("EmailConsumer is ACTIVE and listening!");

        //}

        public async Task Consume(ConsumeContext<EmailMessage> context)
        {
            var msg = context.Message;

            _logger.LogInformation($"📧 Sending email to: {msg.To}");
            _logger.LogInformation($"Subject: {msg.Subject}");
            _logger.LogInformation($"Body: {msg.Body}");

            // (Optional) Call your IEmailSender here if you wire up SMTP later
            await _emailSender.SendAsync(msg.To, msg.Subject, msg.Body);

            _logger.LogInformation($"✅ Email sent to {msg.To}");
        }
    }
}
