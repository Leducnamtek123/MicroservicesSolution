using Microsoft.AspNetCore.Authentication.BearerToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Dtos
{
    public class AccessTokenWithRecoveryCodesResponseDto
    {
        public required AccessTokenResponse AccessTokenResponse { get; set; }
        public required RecoveryCodesResponseDto RecoveryCodesResponse { get; set; }
    }
}
