using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public interface IEmailSender 
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendWelcomeEmailAsync(string email, string userName, string confirmationLink);
        Task SendPasswordResetCodeAsync(string email, string userName, string resetLink);

    }

}
