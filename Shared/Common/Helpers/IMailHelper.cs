using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public interface IMailHelper
    {
        Task SendWelcomeEmailAsync(string email, string userName, string confirmationLink);
        Task SendPasswordResetCodeAsync(string email, string userName, string resetLink);
    }
}
