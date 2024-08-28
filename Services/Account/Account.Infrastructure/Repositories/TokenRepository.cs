using Account.Domain.Models;
using Account.Domain.Repositories;
using Account.Infrastructure.Context;
using Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Infrastructure.Repositories
{
    public class TokenRepository : BaseRepository<RefreshToken>,ITokenRepository
    {
        private readonly AccountDbContext _context;
        private readonly ILogger<TokenRepository> _logger;

        public TokenRepository(AccountDbContext context, ILogger<TokenRepository> logger)
            : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsTokenValidAsync(string tokenId)
        {
            var token = await _context.RefreshTokens
                .Where(t => t.Id == tokenId && t.Expiry > DateTime.UtcNow)
                .FirstOrDefaultAsync();
            return token != null;
        }

        public async Task<bool> RevokeToken(string tokenId)
        {
            var token =  _context.RefreshTokens
                .Where(t => t.Id == tokenId)
                .FirstOrDefault();
            if (token == null)
                return false;

            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool SaveToken(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            return _context.SaveChanges() > 0; // Ensure that changes are committed
        }
        public async Task<bool> DeleteTokensByUserIdAsync(string userId)
        {
            // Lấy tất cả các token của người dùng
            var tokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId)
                .ToListAsync();

            // Nếu không tìm thấy token nào, trả về false
            if (tokens.Count == 0)
            {
                return false;
            }

            // Xóa các token
            _context.RefreshTokens.RemoveRange(tokens);
            await _context.SaveChangesAsync();

            return true; // Trả về true nếu xóa thành công
        }
        public async Task<RefreshToken> GetTokenByCodeAsync(string tokenCode)
        {
            // Truy vấn cơ sở dữ liệu để tìm token theo mã của nó
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenCode == tokenCode);
        }


    }
}
