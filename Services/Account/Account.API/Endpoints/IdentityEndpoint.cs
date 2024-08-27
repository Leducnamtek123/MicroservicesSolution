using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Account.Infrastructure.Cache;
using Account.Application.Dtos;
using Account.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Account.Presentation.Endpoints
{
    public static class IdentityEndpoints
    {
        public static void Map(WebApplication app)
        {
            // Đăng ký các điểm cuối API Identity
            app.MapGroup("/Auth").MapIdentityApi<User>();

            // Điểm cuối tùy chỉnh của bạn
            app.MapPost("/Auth/custom-register", async (UserManager<User> userManager, RegisterDto registerDto) =>
            {
                // Logic tùy chỉnh của bạn
                if (!IsValidUsername(registerDto.UserName))
                {
                    return Results.BadRequest("Invalid username.");
                }

                var user = new User
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                };

                var result = await userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    return Results.Created($"/Auth/users/{user.Id}", user.Id);
                }

                return Results.BadRequest(result.Errors);
            });

            app.MapPost("/Auth/custom-login", async (SignInManager<User> signInManager, UserRedisCache userCache, LoginDto loginDto) =>
            {
                var result = await signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var cacheKey = $"User:{loginDto.UserName}";
                    var token = "generated_token"; // Tạo token của bạn tại đây

                    await userCache.SetUserDataAsync(cacheKey, token, TimeSpan.FromMinutes(30));

                    return Results.Ok(new { Token = token });
                }

                return Results.Unauthorized();
            });

            app.MapPost("/Auth/logout", async (SignInManager<User> signInManager, [FromBody] object empty) =>
            {
                if (empty != null)
                {
                    await signInManager.SignOutAsync();
                    return Results.Ok();
                }
                return Results.Unauthorized();
            })
            .WithOpenApi()
            .RequireAuthorization();
        }

        private static bool IsValidUsername(string username)
        {
            return !string.IsNullOrWhiteSpace(username) && username.Length >= 3;
        }
    }
}
