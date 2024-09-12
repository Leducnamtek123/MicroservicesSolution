using Account.Application.Dtos;
using Account.Domain.Models;
using Common.Dtos;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public interface I2faService
    {
        Task<TwoFactorAuthSetupInfoDto> LoadSharedKeyAndQrCodeUriAsync(User user);
        //Task<BaseResponse<bool>> Verify2faToken(User user, string verificationCode);
    }
}
