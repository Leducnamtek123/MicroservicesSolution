using Account.Application.Dtos;
using Account.Domain.Models;
using Common.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public class _2faService : I2faService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<_2faService> _logger;
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public _2faService(
            UserManager<User> userManager,
            ILogger<_2faService> logger,
            UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _logger = logger;
            _urlEncoder = urlEncoder;
        }

        public async Task<TwoFactorAuthSetupInfoDto> LoadSharedKeyAndQrCodeUriAsync(User user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var SharedKey = FormatKey(unformattedKey);

            var email = await _userManager.GetEmailAsync(user);
            var AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);
            TwoFactorAuthSetupInfoDto newAuth = new TwoFactorAuthSetupInfoDto(SharedKey, AuthenticatorUri);
            return newAuth;
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition));
            }

            return result.ToString();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Microsoft.AspNetCore.Identity.UI"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        //public async Task<BaseResponse<bool>> Verify2faToken(User user, string verificationCode)
        //{
        //    if (user == null)
        //    {
        //        _logger.LogWarning("Attempted to verify 2FA token for a null user.");
        //        return BaseResponse<bool>.Failure("User not found.");
        //    }

        //    if (string.IsNullOrWhiteSpace(verificationCode))
        //    {
        //        _logger.LogWarning("Attempted to verify 2FA token with an empty or whitespace code for user ID '{UserId}'.", await _userManager.GetUserIdAsync(user));
        //        return BaseResponse<bool>.Failure("Verification code is required.");
        //    }

        //    var formattedCode = verificationCode.Replace(" ", string.Empty).Replace("-", string.Empty);
        //    var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
        //        user, _userManager.Options.Tokens.AuthenticatorTokenProvider, formattedCode);

        //    if (!is2faTokenValid)
        //    {
        //        _logger.LogWarning("Invalid 2FA token attempt for user ID '{UserId}'.", await _userManager.GetUserIdAsync(user));
        //        return BaseResponse<bool>.Failure("Verification code is invalid.");
        //    }

        //    await _userManager.SetTwoFactorEnabledAsync(user, true);

        //    var userId = await _userManager.GetUserIdAsync(user);
        //    _logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);

        //    return BaseResponse<bool>.Success(true);
        //}
    }
}
