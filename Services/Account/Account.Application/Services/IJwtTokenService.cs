using Account.Domain.Models;
using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(string userId,string email);
        string GenerateRefreshToken(string userId);
        ClaimsPrincipal GetPrincipalFromExpiredAccessToken(string token);
        Task<bool> RevokeRefreshTokenAsync(string userId);
        Task<RefreshToken> GetTokenByCodeAsync(string tokenCode);
        Task<bool> ValidateRefreshToken(string refreshTokenCode);

    }
}
