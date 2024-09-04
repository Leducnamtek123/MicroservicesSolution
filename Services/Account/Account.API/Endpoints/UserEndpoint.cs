using Account.Application.Dtos;
using Account.Application.Services;
using Account.Domain.Filters;
using Common.Dtos;
using Microsoft.AspNetCore.Mvc;

using Common.Configurations; // Assuming this contains extension methods for API response configuration

namespace Account.API.Endpoints
{
    public static class UserEndpoints
    {
        public static void Map(WebApplication app)
        {
            var userGroup = app.MapGroup("/users")
                .WithTags("User");

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
                    return Results.Problem(detail: errorResponse.Errors[0], statusCode: errorResponse.StatusCode);
                }
            }).ConfigureApiResponses();

            userGroup.MapGet("/{id:guid}", async (IUserService userService, string id) =>
            {
                try
                {
                    var userResponse = await userService.GetUserByIdAsync(id);
                    if (!userResponse.IsSuccess)
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
                    return Results.Problem(detail: errorResponse.Errors[0], statusCode: errorResponse.StatusCode);
                }
            }).ConfigureApiResponses();

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
                    if (!userResponse.IsSuccess)
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
                    return Results.Problem(detail: errorResponse.Errors[0], statusCode: errorResponse.StatusCode);
                }
            }).ConfigureApiResponses();

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
                    if (!userResponse.IsSuccess)
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
                    return Results.Problem(detail: errorResponse.Errors[0], statusCode: errorResponse.StatusCode);
                }
            }).ConfigureApiResponses();

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
                    return Results.Problem(detail: errorResponse.Errors[0], statusCode: errorResponse.StatusCode);
                }
            }).ConfigureApiResponses();

            userGroup.MapPost("/{userId:guid}/roles/{roleName}", async (IUserService userService, string userId, string roleName) =>
            {
                try
                {
                    var response = await userService.AssignRoleToUserAsync(userId, roleName);
                    if (!response.IsSuccess)
                    {
                        var errorResponse = BaseResponse<bool>.Failure("Failed to assign role.");
                        return Results.BadRequest(errorResponse);
                    }
                    return Results.Ok(BaseResponse<bool>.Success(true, 200));
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    var errorResponse = BaseResponse<bool>.Failure("An error occurred while assigning the role.");
                    return Results.Problem(detail: errorResponse.Errors[0], statusCode: errorResponse.StatusCode);
                }
            }).ConfigureApiResponses();

            userGroup.MapPost("/{userId:guid}/roles", async (IUserService userService, string userId, [FromBody] IEnumerable<string> roleNames) =>
            {
                try
                {
                    var response = await userService.AssignRolesToUserAsync(userId, roleNames);
                    if (!response.IsSuccess)
                    {
                        var errorResponse = BaseResponse<bool>.Failure("Failed to assign roles.");
                        return Results.BadRequest(errorResponse);
                    }
                    return Results.Ok(BaseResponse<bool>.Success(true, 200));
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    var errorResponse = BaseResponse<bool>.Failure("An error occurred while assigning the roles.");
                    return Results.Problem(detail: errorResponse.Errors[0], statusCode: errorResponse.StatusCode);
                }
            }).ConfigureApiResponses();

            userGroup.MapDelete("/{userId:guid}/roles/{roleName}", async (IUserService userService, string userId, string roleName) =>
            {
                try
                {
                    var response = await userService.RemoveRoleFromUserAsync(userId, roleName);
                    if (!response.IsSuccess)
                    {
                        var errorResponse = BaseResponse<bool>.Failure("Failed to remove role.");
                        return Results.BadRequest(errorResponse);
                    }
                    return Results.Ok(BaseResponse<bool>.Success(true, 200));
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    var errorResponse = BaseResponse<bool>.Failure("An error occurred while removing the role.");
                    return Results.Problem(detail: errorResponse.Errors[0], statusCode: errorResponse.StatusCode);
                }
            }).ConfigureApiResponses();
        }
    }
}
