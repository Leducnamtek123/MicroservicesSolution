using Account.Application.Dtos;
using Account.Application.Services;
using Account.Domain.Filters;
using Account.Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Presentation.Endpoints
{
    public static class UserEndpoints
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/users", async ([FromServices] IUserService userService, [AsParameters] UserFilter filter) =>
            {
                try
                {
                    var pagedUsers = await userService.GetPagedUsersAsync(filter);
                    return Results.Ok(pagedUsers);
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    return Results.Problem(ex.Message);
                }
            }).WithOpenApi()
.RequireAuthorization(); ;

            app.MapGet("/users/{id:guid}", async (IUserService userService, string id) =>
            {
                var user = await userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(user);
            });

            app.MapPost("/users", async (IUserService userService, [FromBody] UserRequestDto createUserDto) =>
            {
                if (createUserDto == null)
                {
                    return Results.BadRequest("User data is required.");
                }

                var user = await userService.CreateUserAsync(createUserDto);
                return Results.Created($"/users/{user.Id}", user);
            });

            app.MapPut("/users/{id:guid}", async (IUserService userService, string id, [FromBody] UserRequestDto updateUserDto) =>
            {
                if (updateUserDto == null)
                {
                    return Results.BadRequest("User data is required.");
                }

                var updatedUser = await userService.UpdateUserAsync(id, updateUserDto);
                if (updatedUser == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(updatedUser);
            });

            app.MapDelete("/users/{id:guid}", async (IUserService userService, string id) =>
            {
                var success = await userService.DeleteUserAsync(id);
                if (!success)
                {
                    return Results.NotFound();
                }
                return Results.NoContent();
            });
        }
    }
}
