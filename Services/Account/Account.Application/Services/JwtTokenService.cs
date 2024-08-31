using Account.Domain.Models;
using Account.Domain.Repositories;
using Account.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenRepository _tokenRepository;
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _accessTokenExpiryInMinutes;
        private readonly int _refreshTokenExpiryInDays;

        public JwtTokenService(IConfiguration configuration, ITokenRepository tokenRepository)
        {
            _configuration = configuration;
            _tokenRepository = tokenRepository;
            _key = _configuration["Jwt:Key"];
            _issuer = _configuration["Jwt:Issuer"];
            _audience = _configuration["Jwt:Audience"];
            _accessTokenExpiryInMinutes = int.Parse(_configuration["Jwt:ExpiryInMinutes"]);
            _refreshTokenExpiryInDays = int.Parse(_configuration["Jwt:ExpiryInDays"]);
        }

        public string GenerateAccessToken(string userId, string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email), // Thêm email vào claim
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        }),
                Expires = DateTime.UtcNow.AddMinutes(_accessTokenExpiryInMinutes),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public string GenerateRefreshToken(string userId)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var refreshToken = Convert.ToBase64String(randomNumber);

                // Tính toán thời gian hết hạn của RefreshToken (ví dụ: 7 ngày)
                var expiryDate = DateTime.UtcNow.AddDays(_refreshTokenExpiryInDays);

                var token = new RefreshToken
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Expiry = expiryDate,
                    CreatedAt = DateTime.UtcNow,
                    TokenCode = refreshToken
                };

                // Lưu RefreshToken vào cơ sở dữ liệu
                var success = _tokenRepository.SaveToken(token);
                if (!success)
                {
                    throw new Exception("Failed to save refresh token to the database.");
                }

                return refreshToken;
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false, // Allow token to be validated even if expired
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out var securityToken);

                return (securityToken is JwtSecurityToken jwtSecurityToken && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
                    ? principal
                    : null;
            }
            catch
            {
                return null;
            }
        }

        public Task<bool> RevokeRefreshTokenAsync(string userId)
        {
            return _tokenRepository.DeleteTokensByUserIdAsync(userId);
        }

        public async Task<RefreshToken> GetTokenByCodeAsync(string tokenCode)
        {
            // Truy vấn cơ sở dữ liệu để tìm token theo mã của nó
            var token = await _tokenRepository.GetTokenByCodeAsync(tokenCode);
            return token;
        }

        public int AccessTokenExpiryInMinutesGetter => _accessTokenExpiryInMinutes;


        public async Task<bool> ValidateRefreshToken(string refreshTokenCode)
        {
            var token = await GetTokenByCodeAsync(refreshTokenCode);
            return token != null && token.Expiry >= DateTime.UtcNow;
        }
    }
}
