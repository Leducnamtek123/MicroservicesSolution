using Account.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Account.Application.Dtos
{
    public class TwoFactorAuthSetupInfoDto
    {
        public TwoFactorAuthSetupInfoDto(string sharedKey, string authenticatorUri)
        {
            SharedKey = sharedKey;
            AuthenticatorUri = authenticatorUri;
        }

        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }
    }
}
