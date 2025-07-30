using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

namespace IdentityService.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _config["EmailSettings:DisplayName"],
                _config["EmailSettings:From"]
            ));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();

            var server = _config["EmailSettings:SmtpServer"];
            var port = int.Parse(_config["EmailSettings:Port"]);
            var from = _config["EmailSettings:From"];
            var password = _config["EmailSettings:Password"];
            var useSSL = bool.Parse(_config["EmailSettings:UseSSL"]);
            var useStartTls = bool.Parse(_config["EmailSettings:UseStartTls"]);

            SecureSocketOptions options = useStartTls ? SecureSocketOptions.StartTls :
                                            useSSL ? SecureSocketOptions.SslOnConnect :
                                            SecureSocketOptions.None;

            await smtp.ConnectAsync(server, port, options);
            await smtp.AuthenticateAsync(from, password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
