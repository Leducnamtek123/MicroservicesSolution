using Account.Application.Dtos;
using Account.Application.Services;
using Account.Domain.Filters;
using Common.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Account.API.Endpoints
{
    public static class PermissionEndpoints
    {

        public static void Map(WebApplication app)
        {
            #region Define
            var permissionGroup = app.MapGroup("/permissions")
                .WithTags("Permission");
                //.RequireAuthorization();
            #endregion

            #region Get all permissions
            // Endpoint để lấy danh sách các quyền phân trang
            permissionGroup.MapGet("/", async ([FromServices] IPermissionService permissionService, [AsParameters] PermissionFilter filter) =>
            {
                try
                {
                    var pagedPermissions = await permissionService.GetPagedPermissionsAsync(filter);
                    return Results.Ok(pagedPermissions);
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    return Results.Problem(ex.Message);
                }
            }).ConfigureApiResponses();
            #endregion

            #region Get permission by Id
            // Endpoint để lấy thông tin quyền theo ID
            permissionGroup.MapGet("/{id:guid}", async (IPermissionService permissionService, string id) =>
            {
                try
                {
                    var permission = await permissionService.GetPermissionByIdAsync(id);
                    if (permission == null)
                    {
                        return Results.NotFound();
                    }
                    return Results.Ok(permission);
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    return Results.Problem(ex.Message);
                }
            }).ConfigureApiResponses();
            #endregion

            #region Create Permission
            // Endpoint để tạo một quyền mới
            permissionGroup.MapPost("/", async (IPermissionService permissionService, [FromBody] PermissionRequestDto createPermissionDto) =>
            {
                if (createPermissionDto == null)
                {
                    return Results.BadRequest("Permission data is required.");
                }

                try
                {
                    var permission = await permissionService.CreatePermissionAsync(createPermissionDto);
                    if (permission == null)
                    {
                        return Results.BadRequest("Permission creation failed.");
                    }
                    return Results.Created($"/permissions/{permission.Id}", permission);
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    return Results.Problem(ex.Message);
                }
            }).ConfigureApiResponses();
            #endregion

            #region Update Permission
            // Endpoint để cập nhật thông tin quyền theo ID
            permissionGroup.MapPut("/{id:guid}", async (IPermissionService permissionService, string id, [FromBody] PermissionRequestDto updatePermissionDto) =>
            {
                if (updatePermissionDto == null)
                {
                    return Results.BadRequest("Permission data is required.");
                }

                try
                {
                    var updatedPermission = await permissionService.UpdatePermissionAsync(id, updatePermissionDto);
                    if (updatedPermission == null)
                    {
                        return Results.NotFound();
                    }
                    return Results.Ok(updatedPermission);
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    return Results.Problem(ex.Message);
                }
            }).ConfigureApiResponses();
            #endregion

            #region Delete Permission
            // Endpoint để xóa quyền theo ID
            permissionGroup.MapDelete("/{id:guid}", async (IPermissionService permissionService, string id) =>
            {
                try
                {
                    var success = await permissionService.DeletePermissionAsync(id);
                    if (!success)
                    {
                        return Results.NotFound();
                    }
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    return Results.Problem(ex.Message);
                }
            }).ConfigureApiResponses();
            #endregion
        }
    }
}
