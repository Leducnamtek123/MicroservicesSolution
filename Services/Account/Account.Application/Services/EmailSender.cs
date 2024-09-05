using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Common.Configurations;

namespace Account.Application.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient
            {
                Port = _emailSettings.SmtpPort,
                Host = _emailSettings.SmtpServer,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SmtpUser),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            return client.SendMailAsync(mailMessage);
        }

        public async Task SendWelcomeEmailAsync(string email, string userName, string confirmationLink)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "WelcomeEmail.html");
            var emailBody = await File.ReadAllTextAsync(templatePath);

            // Replace placeholders in the email template
            emailBody = emailBody.Replace("{{UserName}}", userName);
            emailBody = emailBody.Replace("{{ConfirmationLink}}", confirmationLink);

            // Send the email
            await SendEmailAsync(email, "Welcome to Our Service", emailBody);
        }
        public async Task SendPasswordResetCodeAsync(string email, string userName, string resetLink)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "ResetPassword.html");
            var emailBody = await File.ReadAllTextAsync(templatePath);

            // Replace placeholders in the email template
            emailBody = emailBody.Replace("{{UserName}}", userName);
            emailBody = emailBody.Replace("{{ResetLink}}", resetLink);

            // Send the email
            await SendEmailAsync(email, "Welcome to Our Service", emailBody);
        }
    } 
}
