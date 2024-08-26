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
            app.MapGroup("/Auth").MapIdentityApi<User>();


            //        // Register new user
            //        routeGroup.MapPost("/register", async (UserManager<TUser> userManager, RegisterDto registerDto) =>
            //        {
            //            if (!IsValidUsername(registerDto.UserName))
            //            {
            //                return Results.BadRequest("Invalid username.");
            //            }

            //            var user = new TUser
            //            {
            //                UserName = registerDto.UserName,
            //                Email = registerDto.Email // Assuming email is still stored but not validated here
            //            };

            //            var result = await userManager.CreateAsync(user, registerDto.Password);

            //            if (result.Succeeded)
            //            {
            //                return Results.Created($"/api/identity/users/{user.Id}", user.Id);
            //            }

            //            return Results.BadRequest(result.Errors);
            //        });

            //        // User login
            //        routeGroup.MapPost("/login", async (SignInManager<TUser> signInManager, UserRedisCache userCache, LoginDto loginDto) =>
            //        {
            //            var result = await signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: false);

            //            if (result.Succeeded)
            //            {
            //                var cacheKey = $"User:{loginDto.UserName}";
            //                var token = "generated_token"; // Generate your token here

            //                await userCache.SetUserDataAsync(cacheKey, token, TimeSpan.FromMinutes(30));

            //                return Results.Ok("Login successful");
            //            }

            //            return Results.Unauthorized();
            //        });

            //        // Get user by ID
            //        routeGroup.MapGet("/users/{id}", async (UserManager<TUser> userManager, UserRedisCache userCache, string id) =>
            //        {
            //            var cacheKey = $"User:{id}";
            //            var cachedUser = await userCache.GetUserDataAsync(cacheKey);

            //            if (cachedUser != null)
            //            {
            //                return Results.Ok(cachedUser); // Note: You may need to deserialize data before returning
            //            }

            //            var userFromDb = await userManager.FindByIdAsync(id);
            //            if (userFromDb == null)
            //            {
            //                return Results.NotFound();
            //            }

            //            var userJson = System.Text.Json.JsonSerializer.Serialize(userFromDb);
            //            await userCache.SetUserDataAsync(cacheKey, userJson, TimeSpan.FromHours(1));

            //            return Results.Ok(userFromDb);
            //        });
            //    }

            //    private static bool IsValidUsername(string username)
            //    {
            //        // Example validation logic: username should not be empty and have a minimum length
            //        return !string.IsNullOrWhiteSpace(username) && username.Length >= 3;
            //    }
            //}

            app.MapPost("/Auth/logout", async (SignInManager<User> signInManager,
    [FromBody] object empty) =>
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
    }
}
