using Account.Domain.Models;
using Common.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Repositories
{
    public interface ITokenRepository : IBaseRepository<RefreshToken>
    {
        Task<bool> IsTokenValidAsync(string tokenId);
        Task<bool> RevokeToken(string tokenId);
        bool SaveToken(RefreshToken token);
        Task<bool> DeleteTokensByUserIdAsync(string userId);
        Task<RefreshToken> GetTokenByCodeAsync(string tokenCode);

    }
}
