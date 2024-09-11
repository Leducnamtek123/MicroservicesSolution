using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Dtos
{
    public class Login2faRequestDto
    {
        /// <summary>
        /// The user's email address which acts as a user name.
        /// </summary>
        public required string Email { get; init; }

        /// <summary>
        /// The 2fa verification code.
        /// </summary>
        public required string VerificationCode { get; init; }
    }
}
