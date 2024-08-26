using Account.Application.Dtos;
using Account.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Account.Presentation.Endpoints
{
    public static class UserEndpoints
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/api/users/{id:guid}", async (Guid id, IUserService userService) =>
            {
                var user = await userService.GetUserByIdAsync(id);
                return user is not null ? Results.Ok(user) : Results.NotFound();
            });

            app.MapPost("/api/users", async (UserRequestDto userRequestDto, IUserService userService) =>
            {
                var user = await userService.CreateUserAsync(userRequestDto);
                return Results.Created($"/api/users/{user.Id}", user);
            });

            app.MapPut("/api/users/{id:guid}", async (Guid id, UserRequestDto userRequestDto, IUserService userService) =>
            {
                var user = await userService.UpdateUserAsync(id, userRequestDto);
                return user is not null ? Results.Ok(user) : Results.NotFound();
            });

            app.MapDelete("/api/users/{id:guid}", async (Guid id, IUserService userService) =>
            {
                var result = await userService.DeleteUserAsync(id);
                return result ? Results.NoContent() : Results.NotFound();
            });
        }
    }
}
