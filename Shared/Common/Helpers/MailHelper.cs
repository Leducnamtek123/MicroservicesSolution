using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public MailHelper(IEmailSender emailSender, IConfiguration configuration)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task SendWelcomeEmailAsync(string email, string userName, string confirmationLink)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "WelcomeEmail.html");
            var emailBody = await File.ReadAllTextAsync(templatePath);

            // Thay thế các biến
            emailBody = emailBody.Replace("{{UserName}}", userName);
            emailBody = emailBody.Replace("{{ConfirmationLink}}", confirmationLink);

            // Gửi email sử dụng IEmailSender
            await _emailSender.SendEmailAsync(email, "Welcome to Our Service", emailBody);
        }
        public async Task SendPasswordResetCodeAsync(string email, string userName, string resetLink)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "ResetPassword.html");
            var emailBody = await File.ReadAllTextAsync(templatePath);

            // Replace placeholders in the email template
            emailBody = emailBody.Replace("{{UserName}}", userName);
            emailBody = emailBody.Replace("{{ResetLink}}", resetLink);

            // Send the email
            await _emailSender.SendEmailAsync(email, "Welcome to Our Service", emailBody);
        }
    }
}
