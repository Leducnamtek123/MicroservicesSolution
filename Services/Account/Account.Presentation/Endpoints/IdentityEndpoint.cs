using Microsoft.AspNetCore.Routing;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Account.Infrastructure.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Account.Presentation.Endpoints
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class LoginDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
    public static class IdentityEndpoints
    {
        public static void MapCustomIdentityApi<TUser>(this IEndpointRouteBuilder endpoints)
            where TUser : IdentityUser, new()
        {
            var routeGroup = endpoints.MapGroup("/api/identity");

            routeGroup.MapPost("/register", async (UserManager<TUser> userManager, RegisterDto registerDto) =>
            {
                if (!IsValidEmail(registerDto.Email))
                {
                    return Results.BadRequest("Invalid email address.");
                }

                var user = new TUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                };

                var result = await userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    return Results.Created($"/api/identity/users/{user.Id}", user.Id);
                }

                return Results.BadRequest(result.Errors);
            });

            routeGroup.MapPost("/login", async (SignInManager<TUser> signInManager, UserRedisCache userCache, LoginDto loginDto) =>
            {
                var result = await signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var cacheKey = $"User:{loginDto.UserName}";
                    var token = "generated_token"; // Tạo token của bạn tại đây

                    await userCache.SetUserDataAsync(cacheKey, token, TimeSpan.FromMinutes(30));

                    return Results.Ok("Login successful");
                }

                return Results.Unauthorized();
            });

            routeGroup.MapGet("/users/{id}", async (UserManager<TUser> userManager, UserRedisCache userCache, string id) =>
            {
                var cacheKey = $"User:{id}";
                var cachedUser = await userCache.GetUserDataAsync(cacheKey);

                if (cachedUser != null)
                {
                    return Results.Ok(cachedUser); // Lưu ý: Bạn cần deserialize dữ liệu trước khi trả về
                }

                var userFromDb = await userManager.FindByIdAsync(id);
                if (userFromDb == null)
                {
                    return Results.NotFound();
                }

                var userJson = System.Text.Json.JsonSerializer.Serialize(userFromDb);
                await userCache.SetUserDataAsync(cacheKey, userJson, TimeSpan.FromHours(1));

                return Results.Ok(userFromDb);
            });
        }

        private static bool IsValidEmail(string email)
        {
            var emailAddressAttribute = new EmailAddressAttribute();
            return emailAddressAttribute.IsValid(email);
        }
    }
}
