using Microsoft.Extensions.Options;
using Notification.Application.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Notification.Infrastructure.Services
{
    public class GmailNotificationSender : IEmailNotificationSender
    {
        public GmailNotificationSender(IOptions<SmtpSettings> options, ILoggerService loggerService)
        {
            _smtpSettings = options.Value;
            _loggerService = loggerService;
        }

        private readonly SmtpSettings _smtpSettings;
        private readonly ILoggerService _loggerService;

        public async Task SendAsync(string email, string subject, string message)
        {
            try
            {

                var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    EnableSsl = _smtpSettings.EnableSsl,
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 10000
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);

                await _loggerService.LogInfo($"Message sent to email: {email}", "GmailNotificationSender.SendAsync");
            }
            catch (Exception e)
            {
                await _loggerService.LogError($"Failed to send email to {email}. Error: {e.Message}",
                    "GmailNotificationSender.SendAsync", e.GetType().FullName);

                throw;
            }
        }
    }
}
