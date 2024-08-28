using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public interface ITokenService
    {
        public string GenerateToken(string userId);


        public bool IsTokenValid(string token);

        public void RevokeToken(string token);

        public void RevokeOldTokens(string userId);
    }

}
