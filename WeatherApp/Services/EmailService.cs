using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace WeatherApp.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var host = _config["EmailNotifications:Smtp:Host"];
            var port = int.Parse(_config["EmailNotifications:Smtp:Port"] ?? "587");
            var fromEmail = _config["EmailNotifications:Smtp:Email"];
            var password = _config["EmailNotifications:Smtp:Password"];

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail, password)
            };

            using var message = new MailMessage(fromEmail, toEmail, subject, body);
            await client.SendMailAsync(message);
        }
    }
}