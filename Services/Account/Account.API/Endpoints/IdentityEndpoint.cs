using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Http.Metadata;
using System.Diagnostics;
using Account.Application.Services;
using Account.Infrastructure.Cache;
using Account.Application.Dtos;
using Account.Domain.Models;
using Common.Dtos;
using Common.Configurations;
using Azure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Account.API.Endpoints
{
    public static class IdentityEndpoints
    {

        // Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.
        private static readonly EmailAddressAttribute _emailAddressAttribute = new();

        public static IEndpointConventionBuilder MapIdentityApi<TUser>(this IEndpointRouteBuilder endpoints)
            where TUser : class, new()
        {
            #region Define 
            ArgumentNullException.ThrowIfNull(endpoints);

            var timeProvider = endpoints.ServiceProvider.GetRequiredService<TimeProvider>();
            var bearerTokenOptions = endpoints.ServiceProvider.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>();
            var emailSender = endpoints.ServiceProvider.GetRequiredService<IEmailSender<TUser>>();
            var linkGenerator = endpoints.ServiceProvider.GetRequiredService<LinkGenerator>();

            // We'll figure out a unique endpoint name based on the final route pattern during endpoint generation.
            string? confirmEmailEndpointName = null;

            #endregion

            #region Route Groups
            var routeGroup = endpoints.MapGroup("Auth").WithTags("Auth");
            var accountGroup = routeGroup.MapGroup("/manage").RequireAuthorization();
            var tfaGroup = routeGroup.MapGroup("/2fa").RequireAuthorization().WithTags("2FA");
            #endregion

            #region Register
            routeGroup.MapPost("/register", async Task<Results<Ok<BaseResponse<UserResponseDto>>, BadRequest<BaseResponse<UserResponseDto>>>>
       ([FromBody] UserRequestDto userRequestDto, HttpContext context, [FromServices] IUserService userService, IUserRedisCache userRedisCache, IEmailSender emailSender) =>
            {
                // Attempt to create the user
                var createUserResponse = await userService.CreateUserAsync(userRequestDto);

                if (!createUserResponse.IsSuccess)
                {
                    // Return BadRequest with the errors from the BaseResponse
                    return TypedResults.BadRequest(createUserResponse);
                }

                // Return Ok with the user data
                return TypedResults.Ok(createUserResponse);
            }).ConfigureApiResponses();
            #endregion

            #region Login 
            routeGroup.MapPost("/login", async Task<IResult>
            ([FromBody] LoginRequestDto login, [FromServices] IServiceProvider sp) =>
            {
                var userManager = sp.GetRequiredService<UserManager<User>>();
                var jwtTokenService = sp.GetRequiredService<IJwtTokenService>();
                var userRedisCache = sp.GetRequiredService<IUserRedisCache>(); // Inject cache service
                var configuration = sp.GetRequiredService<IConfiguration>();

                // Tìm người dùng theo email
                var user = await userManager.FindByEmailAsync(login.Email);
                if (user == null || !await userManager.CheckPasswordAsync(user, login.Password))
                {
                    // Trả về phản hồi thất bại với thông báo lỗi chuỗi
                    var errorResponse = BaseResponse<string>.Failure("Invalid login attempt");
                    return Results.BadRequest(errorResponse);
                }

                if (await userManager.GetTwoFactorEnabledAsync(user))
                {
                    var tfaResponse = BaseResponse<string>.Accepted("Two-factor authentication is enabled.");
                    return Results.Ok(tfaResponse);
                }
                

                var expiryInMinutes = Convert.ToDouble(configuration.GetSection("Jwt")["ExpiryInMinutes"]);
                var tokenKey = $"jwt_token_{user.Id}";
                var cachedToken = await userRedisCache.GetUserDataAsync(tokenKey);

                AccessTokenResponse response;

                if (cachedToken != null)
                {
                    // Revoke the old refresh token
                    await jwtTokenService.RevokeRefreshTokenAsync(user.Id);

                    // Generate new refresh token
                    var refreshToken = jwtTokenService.GenerateRefreshToken(user.Id);

                    response = new AccessTokenResponse
                    {
                        AccessToken = cachedToken,
                        // Chuyển đổi thời gian hết hạn thành số ticks
                        ExpiresIn = TimeSpan.FromMinutes(expiryInMinutes).Ticks,
                        RefreshToken = refreshToken
                    };
                }
                else
                {
                    var token = jwtTokenService.GenerateAccessToken(user.Id, user.Email);
                    var refreshToken = jwtTokenService.GenerateRefreshToken(user.Id);
                    response = new AccessTokenResponse
                    {
                        AccessToken = token,
                        // Chuyển đổi thời gian hết hạn thành số ticks
                        ExpiresIn = TimeSpan.FromMinutes(expiryInMinutes).Ticks,
                        RefreshToken = refreshToken // Provide a mechanism to handle refresh tokens if needed
                    };

                    // Store token in cache
                    await userRedisCache.SetUserDataAsync(tokenKey, token, TimeSpan.FromMinutes(expiryInMinutes));
                }

                var successResponse = BaseResponse<AccessTokenResponse>.Success(response);
                return Results.Ok(successResponse);
            }).ConfigureApiResponses();
            #endregion

            #region Verify 2fa
            tfaGroup.MapPost("/verify", async Task<IResult>
            ([FromBody] Login2faRequestDto login, [FromServices] IServiceProvider sp) =>
            {
                var userManager = sp.GetRequiredService<UserManager<User>>();
                var jwtTokenService = sp.GetRequiredService<IJwtTokenService>();
                var userRedisCache = sp.GetRequiredService<IUserRedisCache>(); // Inject cache service
                var configuration = sp.GetRequiredService<IConfiguration>();

                // Tìm người dùng theo email
                var user = await userManager.FindByEmailAsync(login.Email);

                if(user == null)
                {
                    var errorResponse = BaseResponse<string>.Failure("User not found");
                    return Results.BadRequest(errorResponse);
                }

                if (!await userManager.GetTwoFactorEnabledAsync(user))
                {
                    var errorResponse = BaseResponse<string>.Failure("Invalid two-factor verification code.");
                    return Results.BadRequest(errorResponse);
                }

                if (!await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, login.VerificationCode))
                {
                    var tfaResponse = BaseResponse<string>.Failure("Two-factor authentication is not enabled.");
                    return Results.BadRequest(tfaResponse);
                }

                var expiryInMinutes = Convert.ToDouble(configuration.GetSection("Jwt")["ExpiryInMinutes"]);
                var tokenKey = $"jwt_token_{user.Id}";
                var cachedToken = await userRedisCache.GetUserDataAsync(tokenKey);

                AccessTokenResponse response;

                if (cachedToken != null)
                {
                    // Revoke the old refresh token
                    await jwtTokenService.RevokeRefreshTokenAsync(user.Id);

                    // Generate new refresh token
                    var refreshToken = jwtTokenService.GenerateRefreshToken(user.Id);

                    response = new AccessTokenResponse
                    {
                        AccessToken = cachedToken,
                        // Chuyển đổi thời gian hết hạn thành số ticks
                        ExpiresIn = TimeSpan.FromMinutes(expiryInMinutes).Ticks,
                        RefreshToken = refreshToken
                    };
                }
                else
                {
                    var token = jwtTokenService.GenerateAccessToken(user.Id, user.Email);
                    var refreshToken = jwtTokenService.GenerateRefreshToken(user.Id);
                    response = new AccessTokenResponse
                    {
                        AccessToken = token,
                        // Chuyển đổi thời gian hết hạn thành số ticks
                        ExpiresIn = TimeSpan.FromMinutes(expiryInMinutes).Ticks,
                        RefreshToken = refreshToken // Provide a mechanism to handle refresh tokens if needed
                    };

                    // Store token in cache
                    await userRedisCache.SetUserDataAsync(tokenKey, token, TimeSpan.FromMinutes(expiryInMinutes));
                }

                var successResponse = BaseResponse<AccessTokenResponse>.Success(response);
                return Results.Ok(successResponse);
            })
            .ConfigureApiResponses()
            .AllowAnonymous();
            #endregion

            #region Logout
            routeGroup.MapPost("/logout", async (
                [FromServices] SignInManager<User> signInManager,
                [FromServices] IUserRedisCache userRedisCache,
                [FromBody] object empty) =>
            {
                if (empty != null)
                {
                    // Lấy thông tin người dùng hiện tại
                    var user = await signInManager.UserManager.GetUserAsync(signInManager.Context.User);
                    if (user == null)
                    {
                        return Results.Unauthorized();
                    }

                    var userId = user.Id; // Giả sử User có thuộc tính Id là userId

                    // Xóa cache của người dùng
                    var tokenKey = $"jwt_token_{userId}";
                    await userRedisCache.RemoveUserDataAsync(tokenKey);

                    // Thực hiện đăng xuất
                    await signInManager.SignOutAsync();
                    return Results.Ok();
                }
                return Results.Unauthorized();

            })
            .RequireAuthorization();
            #endregion

            #region Refresh Token
            routeGroup.MapPost("/refresh", async Task<IResult> (
                [FromServices] IServiceProvider sp,
                [FromBody] RefreshRequest refreshRequest) =>
            {
                var userManager = sp.GetRequiredService<UserManager<User>>();
                var jwtTokenService = sp.GetRequiredService<IJwtTokenService>();
                var userRedisCache = sp.GetRequiredService<IUserRedisCache>();
                var configuration = sp.GetRequiredService<IConfiguration>();

                // Lấy refresh token từ yêu cầu
                var refreshTokenCode = refreshRequest.RefreshToken;

                // Kiểm tra tính hợp lệ của refresh token
                if (!await jwtTokenService.ValidateRefreshToken(refreshTokenCode))
                {
                    var errorResponse = BaseResponse<string>.Failure("Invalid or expired refresh token");
                    return Results.BadRequest(errorResponse);
                }

                // Lấy thông tin từ refresh token
                var token = await jwtTokenService.GetTokenByCodeAsync(refreshTokenCode);
                if (token == null || token.Expiry < DateTime.UtcNow)
                {
                    var errorResponse = BaseResponse<string>.Failure("Invalid or expired refresh token");
                    return Results.BadRequest(errorResponse);
                }

                // Xác thực người dùng bằng UserManager
                var user = await userManager.FindByIdAsync(token.UserId);
                if (user == null)
                {
                    var errorResponse = BaseResponse<string>.Failure("User validation failed");
                    return Results.BadRequest(errorResponse);
                }

                // Tạo access token mới
                var newAccessToken = jwtTokenService.GenerateAccessToken(token.UserId, user.Email);

                // Lưu token mới vào cache (nếu cần)
                var tokenKey = $"jwt_token_{token.UserId}";
                var expiryInMinutes = Convert.ToDouble(configuration.GetSection("Jwt")["ExpiryInMinutes"]);
                await userRedisCache.SetUserDataAsync(tokenKey, newAccessToken, TimeSpan.FromMinutes(expiryInMinutes));

                // Tính toán thời gian hết hạn (ticks)
                var expiresInTicks = TimeSpan.FromMinutes(expiryInMinutes).Ticks;

                var response = new RequestTokenResponseDto
                {
                    AccessToken = newAccessToken,
                    Expires = expiresInTicks // Sử dụng ticks thay vì giây
                };

                var successResponse = BaseResponse<RequestTokenResponseDto>.Success(response);
                return Results.Ok(successResponse);
            }).ConfigureApiResponses();
            #endregion


            #region Confirm Email
            routeGroup.MapGet("/confirmEmail", async Task<Results<ContentHttpResult, UnauthorizedHttpResult>>
                ([FromQuery] string userId, [FromQuery] string code, [FromQuery] string? changedEmail, [FromServices] IServiceProvider sp) =>
            {
                var userManager = sp.GetRequiredService<UserManager<TUser>>();
                if (await userManager.FindByIdAsync(userId) is not { } user)
                {
                    // We could respond with a 404 instead of a 401 like Identity UI, but that feels like unnecessary information.
                    return TypedResults.Unauthorized();
                }

                try
                {
                    code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                }
                catch (FormatException)
                {
                    return TypedResults.Unauthorized();
                }

                IdentityResult result;

                if (string.IsNullOrEmpty(changedEmail))
                {
                    result = await userManager.ConfirmEmailAsync(user, code);
                }
                else
                {
                    // As with Identity UI, email and user name are one and the same. So when we update the email,
                    // we need to update the user name.
                    result = await userManager.ChangeEmailAsync(user, changedEmail, code);

                    if (result.Succeeded)
                    {
                        result = await userManager.SetUserNameAsync(user, changedEmail);
                    }
                }

                if (!result.Succeeded)
                {
                    return TypedResults.Unauthorized();
                }

                return TypedResults.Text("Thank you for confirming your email.");
            })
            .Add(endpointBuilder =>
            {
                var finalPattern = ((RouteEndpointBuilder)endpointBuilder).RoutePattern.RawText;
                confirmEmailEndpointName = $"{nameof(MapIdentityApi)}-{finalPattern}";
                endpointBuilder.Metadata.Add(new EndpointNameMetadata(confirmEmailEndpointName));
            });
            #endregion

            #region Resend confirm email
            routeGroup.MapPost("/resendConfirmationEmail", async Task<Ok>
                ([FromBody] ResendConfirmationEmailRequest resendRequest, HttpContext context, [FromServices] IServiceProvider sp) =>
            {
                var emailSender = endpoints.ServiceProvider.GetRequiredService<IEmailSender>();

                var userManager = sp.GetRequiredService<UserManager<User>>();
                if (await userManager.FindByEmailAsync(resendRequest.Email) is not { } user)
                {
                    return TypedResults.Ok();
                }
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodeToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var activationLink = $"http://localhost:5165/Auth/ConfirmEmail?userId={user.Id}&code={encodeToken}";

                // Gửi email chào mừng với liên kết kích hoạt
                await emailSender.SendWelcomeEmailAsync(user.Email, user.UserName, activationLink);
                return TypedResults.Ok();
            });
            #endregion

            #region Forgot Password
            routeGroup.MapPost("/forgotPassword", async Task<Results<Ok, ValidationProblem>>
                ([FromBody] ForgotPasswordRequest resetRequest, [FromServices] IServiceProvider sp, [FromServices] IEmailSender emailSender) =>
            {
                var userManager = sp.GetRequiredService<UserManager<User>>();
                var user = await userManager.FindByEmailAsync(resetRequest.Email);

                if (user is not null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var code = await userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var resetLink = $"http://localhost:5165/Auth/ResetPassword?Email={user.Email}&code={code}";

                    //await emailSender.SendPasswordResetCodeAsync(user, resetRequest.Email, HtmlEncoder.Default.Encode(code));
                    await emailSender.SendPasswordResetCodeAsync(user.Email, user.UserName, resetLink);
                }

                // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
                // returned a 400 for an invalid code given a valid user email.
                return TypedResults.Ok();
            });
            #endregion

            #region Reset password
            routeGroup.MapPost("/resetPassword", async Task<Results<Ok, ValidationProblem>>
                ([FromBody] ResetPasswordRequest resetRequest, [FromServices] IServiceProvider sp) =>
            {
                var userManager = sp.GetRequiredService<UserManager<User>>();

                var user = await userManager.FindByEmailAsync(resetRequest.Email);

                if (user is null || !(await userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
                    // returned a 400 for an invalid code given a valid user email.
                    return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));
                }

                IdentityResult result;
                try
                {
                    var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetRequest.ResetCode));
                    result = await userManager.ResetPasswordAsync(user, code, resetRequest.NewPassword);
                }
                catch (FormatException)
                {
                    result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
                }

                if (!result.Succeeded)
                {
                    return CreateValidationProblem(result);
                }

                return TypedResults.Ok();
            });
            #endregion

            #region 2fa
            accountGroup.MapPost("/2fa", async Task<Results<Ok<TwoFactorResponse>, ValidationProblem, NotFound>>
                (ClaimsPrincipal claimsPrincipal, [FromBody] TwoFactorRequest tfaRequest, [FromServices] IServiceProvider sp) =>
            {
                var signInManager = sp.GetRequiredService<SignInManager<User>>();
                var userManager = signInManager.UserManager;
                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                {
                    return TypedResults.NotFound();
                }

                if (tfaRequest.Enable == true)
                {
                    if (tfaRequest.ResetSharedKey)
                    {
                        return CreateValidationProblem("CannotResetSharedKeyAndEnable",
                            "Resetting the 2fa shared key must disable 2fa until a 2fa token based on the new shared key is validated.");
                    }
                    else if (string.IsNullOrEmpty(tfaRequest.TwoFactorCode))
                    {
                        return CreateValidationProblem("RequiresTwoFactor",
                            "No 2fa token was provided by the request. A valid 2fa token is required to enable 2fa.");
                    }
                    else if (!await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, tfaRequest.TwoFactorCode))
                    {
                        return CreateValidationProblem("InvalidTwoFactorCode",
                            "The 2fa token provided by the request was invalid. A valid 2fa token is required to enable 2fa.");
                    }

                    await userManager.SetTwoFactorEnabledAsync(user, true);
                }
                else if (tfaRequest.Enable == false || tfaRequest.ResetSharedKey)
                {
                    await userManager.SetTwoFactorEnabledAsync(user, false);
                }

                if (tfaRequest.ResetSharedKey)
                {
                    await userManager.ResetAuthenticatorKeyAsync(user);
                }

                string[]? recoveryCodes = null;
                if (tfaRequest.ResetRecoveryCodes || (tfaRequest.Enable == true && await userManager.CountRecoveryCodesAsync(user) == 0))
                {
                    var recoveryCodesEnumerable = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                    recoveryCodes = recoveryCodesEnumerable?.ToArray();
                }

                if (tfaRequest.ForgetMachine)
                {
                    await signInManager.ForgetTwoFactorClientAsync();
                }

                var key = await userManager.GetAuthenticatorKeyAsync(user);
                if (string.IsNullOrEmpty(key))
                {
                    await userManager.ResetAuthenticatorKeyAsync(user);
                    key = await userManager.GetAuthenticatorKeyAsync(user);

                    if (string.IsNullOrEmpty(key))
                    {
                        throw new NotSupportedException("The user manager must produce an authenticator key after reset.");
                    }
                }

                return TypedResults.Ok(new TwoFactorResponse
                {
                    SharedKey = key,
                    RecoveryCodes = recoveryCodes,
                    RecoveryCodesLeft = recoveryCodes?.Length ?? await userManager.CountRecoveryCodesAsync(user),
                    IsTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user),
                    IsMachineRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user),
                });
            });

            #endregion

            #region Setup information for 2fa
            accountGroup.MapGet("/2fa/setup-info", async Task<Results<Ok<BaseResponse<TwoFactorAuthSetupInfoDto>>, ValidationProblem, NotFound>>
                (ClaimsPrincipal claimsPrincipal, [FromServices] IServiceProvider serviceProvider) =>
            {
                var i2faService = serviceProvider.GetRequiredService<I2faService>();
                var userManager = serviceProvider.GetRequiredService<UserManager<User>>();


                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                {
                    return TypedResults.NotFound();
                }

                var enableAuthenInfo = await i2faService.LoadSharedKeyAndQrCodeUriAsync(user);
                var successResponse = BaseResponse<TwoFactorAuthSetupInfoDto>.Success(enableAuthenInfo);
                return TypedResults.Ok(successResponse);
            })
            .ConfigureApiResponses();
            #endregion

            #region Enable 2fa
            tfaGroup.MapPost("/enable2fa", async Task<Results<Ok<BaseResponse<TwoFactorResponse>>, ValidationProblem, NotFound>>
                (ClaimsPrincipal claimsPrincipal, [FromServices] IServiceProvider sp, [FromBody] TwoFactorAuthSetupRequestDto tfaRequest) =>
            {
                var userManager = sp.GetRequiredService<UserManager<User>>();
                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                {
                    return TypedResults.NotFound();
                }

                if (tfaRequest.verificationCode == null)
                {
                    return CreateValidationProblem("RequiresTwoFactor",
                            "No 2fa token was provided by the request. A valid 2fa token is required to enable 2fa.");
                }

                if (!await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, tfaRequest.verificationCode))
                {
                    return CreateValidationProblem("InvalidTwoFactorCode",
                            "The 2fa token provided by the request was invalid. A valid 2fa token is required to enable 2fa.");
                }

                //await userManager.SetTwoFactorEnabledAsync(user, true);

                //string[]? recoveryCodes = null;
                //if (tfaRequest.ResetRecoveryCodes || (tfaRequest.Enable == true && await userManager.CountRecoveryCodesAsync(user) == 0))
                //{
                //    var recoveryCodesEnumerable = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                //    recoveryCodes = recoveryCodesEnumerable?.ToArray();
                //}

                //return TypedResults.Ok(new TwoFactorResponse
                //{
                //    SharedKey = key,
                //    RecoveryCodes = recoveryCodes,
                //    RecoveryCodesLeft = recoveryCodes?.Length ?? await userManager.CountRecoveryCodesAsync(user),
                //    IsTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user),
                //});
                return TypedResults.NotFound();
            })
            .ConfigureApiResponses();
            #endregion

            #region Get Info
            accountGroup.MapGet("/info", async Task<Results<Ok<BaseResponse<InfoResponse>>, ValidationProblem, NotFound>>
                (ClaimsPrincipal claimsPrincipal, [FromServices] IServiceProvider sp) =>
            {
                var userManager = sp.GetRequiredService<UserManager<TUser>>();
                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                {
                    return TypedResults.NotFound();
                }

                var infoResponse = await CreateInfoResponseAsync(user, userManager);

                // Wrap the infoResponse in BaseResponse
                var successResponse = BaseResponse<InfoResponse>.Success(infoResponse);

                return TypedResults.Ok(successResponse);
            });
            #endregion

            #region Post info
            accountGroup.MapPost("/info", async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>>
                (ClaimsPrincipal claimsPrincipal, [FromBody] InfoRequest infoRequest, HttpContext context, [FromServices] IServiceProvider sp) =>
            {
                var userManager = sp.GetRequiredService<UserManager<TUser>>();
                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                {
                    return TypedResults.NotFound();
                }

                if (!string.IsNullOrEmpty(infoRequest.NewEmail) && !_emailAddressAttribute.IsValid(infoRequest.NewEmail))
                {
                    return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(infoRequest.NewEmail)));
                }

                if (!string.IsNullOrEmpty(infoRequest.NewPassword))
                {
                    if (string.IsNullOrEmpty(infoRequest.OldPassword))
                    {
                        return CreateValidationProblem("OldPasswordRequired",
                            "The old password is required to set a new password. If the old password is forgotten, use /resetPassword.");
                    }

                    var changePasswordResult = await userManager.ChangePasswordAsync(user, infoRequest.OldPassword, infoRequest.NewPassword);
                    if (!changePasswordResult.Succeeded)
                    {
                        return CreateValidationProblem(changePasswordResult);
                    }
                }

                if (!string.IsNullOrEmpty(infoRequest.NewEmail))
                {
                    var email = await userManager.GetEmailAsync(user);

                    if (email != infoRequest.NewEmail)
                    {
                        await SendConfirmationEmailAsync(user, userManager, context, infoRequest.NewEmail, isChange: true);
                    }
                }

                return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
            });
            #endregion

            #region Send email func
            async Task SendConfirmationEmailAsync(TUser user, UserManager<TUser> userManager, HttpContext context, string email, bool isChange = false)
            {
                if (confirmEmailEndpointName is null)
                {
                    throw new NotSupportedException("No email confirmation endpoint was registered!");
                }

                var code = isChange
                    ? await userManager.GenerateChangeEmailTokenAsync(user, email)
                    : await userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var userId = await userManager.GetUserIdAsync(user);
                var routeValues = new RouteValueDictionary()
                {
                    ["userId"] = userId,
                    ["code"] = code,
                };

                if (isChange)
                {
                    // This is validated by the /confirmEmail endpoint on change.
                    routeValues.Add("changedEmail", email);
                }

                var confirmEmailUrl = linkGenerator.GetUriByName(context, confirmEmailEndpointName, routeValues)
                    ?? throw new NotSupportedException($"Could not find endpoint named '{confirmEmailEndpointName}'.");

                await emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(confirmEmailUrl));
            }
            #endregion

            return new IdentityEndpointsConventionBuilder(routeGroup);
        }
        private static ValidationProblem CreateValidationProblem(string errorCode, string errorDescription) =>
    TypedResults.ValidationProblem(new Dictionary<string, string[]> {
            { errorCode, [errorDescription] }
    });

        private static ValidationProblem CreateValidationProblem(IdentityResult result)
        {
            // We expect a single error code and description in the normal case.
            // This could be golfed with GroupBy and ToDictionary, but perf! :P
            Debug.Assert(!result.Succeeded);
            var errorDictionary = new Dictionary<string, string[]>(1);

            foreach (var error in result.Errors)
            {
                string[] newDescriptions;

                if (errorDictionary.TryGetValue(error.Code, out var descriptions))
                {
                    newDescriptions = new string[descriptions.Length + 1];
                    Array.Copy(descriptions, newDescriptions, descriptions.Length);
                    newDescriptions[descriptions.Length] = error.Description;
                }
                else
                {
                    newDescriptions = [error.Description];
                }

                errorDictionary[error.Code] = newDescriptions;
            }

            return TypedResults.ValidationProblem(errorDictionary);
        }

        private static async Task<InfoResponse> CreateInfoResponseAsync<TUser>(TUser user, UserManager<TUser> userManager)
            where TUser : class
        {
            return new()
            {
                Email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
                IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
            };
        }

        // Wrap RouteGroupBuilder with a non-public type to avoid a potential future behavioral breaking change.
        private sealed class IdentityEndpointsConventionBuilder(RouteGroupBuilder inner) : IEndpointConventionBuilder
        {
            private IEndpointConventionBuilder InnerAsConventionBuilder => inner;

            public void Add(Action<EndpointBuilder> convention) => InnerAsConventionBuilder.Add(convention);
            public void Finally(Action<EndpointBuilder> finallyConvention) => InnerAsConventionBuilder.Finally(finallyConvention);
        }

        [AttributeUsage(AttributeTargets.Parameter)]
        private sealed class FromBodyAttribute : Attribute, IFromBodyMetadata
        {
        }

        [AttributeUsage(AttributeTargets.Parameter)]
        private sealed class FromServicesAttribute : Attribute, IFromServiceMetadata
        {
        }

        [AttributeUsage(AttributeTargets.Parameter)]
        private sealed class FromQueryAttribute : Attribute, IFromQueryMetadata
        {
            public string? Name => null;
        }
    }
}
