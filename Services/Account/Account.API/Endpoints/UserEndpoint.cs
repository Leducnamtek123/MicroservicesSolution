using Account.Application.Dtos;
using Account.Application.Services;
using Account.Domain.Filters;
using Account.Domain.Models;
using Common.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Account.API.Endpoints
{
    public static class UserEndpoints
    {
        public static void Map(WebApplication app)
        {
            #region Define
            var userGroup = app.MapGroup("/users")
                .WithTags("User");
            #endregion

            #region GetPagedUsers
            userGroup.MapGet("/", async ([FromServices] IUserService userService, [AsParameters] UserFilter filter) =>
            {
                try
                {
                    filter ??= new UserFilter();

                    var pagedUsers = await userService.GetPagedUsersAsync(filter);
                    var response = BaseResponse<PagedDto<UserResponseDto>>.Success(pagedUsers);
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    var errorResponse = BaseResponse<PagedDto<UserResponseDto>>.Failure("An error occurred while retrieving users.");
                    return Results.Problem(detail: errorResponse.Errors[0]);
                }
            });
            #endregion

            #region GetUserById
            userGroup.MapGet("/{id:guid}", async (IUserService userService, string id) =>
            {
                try
                {
                    var userResponse = await userService.GetUserByIdAsync(id);
                    if (userResponse == null)
                    {
                        var errorResponse = BaseResponse<UserResponseDto>.Failure("User not found.");
                        return Results.NotFound(errorResponse);
                    }
                    return Results.Ok(userResponse);
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    var errorResponse = BaseResponse<UserResponseDto>.Failure("An error occurred while retrieving the user.");
                    return Results.Problem(detail: errorResponse.Errors[0]);
                }
            });
            #endregion

            #region CreateUser
            userGroup.MapPost("/", async (IUserService userService, [FromBody] UserRequestDto createUserDto) =>
            {
                if (createUserDto == null)
                {
                    var errorResponse = BaseResponse<UserResponseDto>.Failure("User data is required.");
                    return Results.BadRequest(errorResponse);
                }

                try
                {
                    var userResponse = await userService.CreateUserAsync(createUserDto);
                    if (userResponse == null)
                    {
                        var errorResponse = BaseResponse<UserResponseDto>.Failure("User creation failed.");
                        return Results.BadRequest(errorResponse);
                    }
                    return Results.Ok(userResponse);
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    var errorResponse = BaseResponse<UserResponseDto>.Failure("An error occurred while creating the user.");
                    return Results.Problem(detail: errorResponse.Errors[0]);
                }
            });
            #endregion

            #region UpdateUser
            userGroup.MapPut("/{id:guid}", async (IUserService userService, string id, [FromBody] UserRequestDto updateUserDto) =>
            {
                if (updateUserDto == null)
                {
                    var errorResponse = BaseResponse<UserResponseDto>.Failure("User data is required.");
                    return Results.BadRequest(errorResponse);
                }

                try
                {
                    var userResponse = await userService.UpdateUserAsync(id, updateUserDto);
                    if (userResponse == null)
                    {
                        var errorResponse = BaseResponse<UserResponseDto>.Failure("User not found.");
                        return Results.NotFound(errorResponse);
                    }
                    return Results.Ok(userResponse);
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    var errorResponse = BaseResponse<UserResponseDto>.Failure("An error occurred while updating the user.");
                    return Results.Problem(detail: errorResponse.Errors[0]);
                }
            });
            #endregion

            #region DeleteUser
            userGroup.MapDelete("/{id:guid}", async (IUserService userService, string id) =>
            {
                try
                {
                    var response = await userService.DeleteUserAsync(id);
                    if (!response.IsSuccess)
                    {
                        var errorResponse = BaseResponse<UserResponseDto>.Failure("User not found.");
                        return Results.NotFound(errorResponse);
                    }
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    var errorResponse = BaseResponse<UserResponseDto>.Failure("An error occurred while deleting the user.");
                    return Results.Problem(detail: errorResponse.Errors[0]);
                }
            });
            #endregion
        }
    }
}
