using Account.Application.Dtos;
using Account.Application.Services;
using Account.Domain.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace Account.API.Endpoints
{
    public static class RoleEndpoints
    {
        public static void Map(WebApplication app)
        {
            #region Define
            var userGroup = app.MapGroup("/roles").WithTags("Role");
            //.RequireAuthorization();
            #endregion

            #region Get all roles
            // Endpoint để lấy danh sách các vai trò phân trang
            userGroup.MapGet("/", async ([FromServices] IRoleService roleService, [AsParameters] RoleFilter filter) =>
            {
                try
                {
                    var pagedRoles = await roleService.GetPagedRolesAsync(filter);
                    return Results.Ok(pagedRoles);
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    return Results.Problem(ex.Message);
                }
            });
            #endregion

            #region Get role by Id
            // Endpoint để lấy thông tin vai trò theo ID
            userGroup.MapGet("/{id:guid}", async (IRoleService roleService, string id, bool? IsDeep = false) =>
            {
                var role = await roleService.GetRoleByIdAsync(id, (bool)IsDeep);
                if (role == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(role);
            });
            #endregion

            #region Create Role
            // Endpoint để tạo một vai trò mới
            userGroup.MapPost("/", async (IRoleService roleService, [FromBody] RoleRequestDto createRoleDto) =>
            {
                if (createRoleDto == null)
                {
                    return Results.BadRequest("Role data is required.");
                }

                var role = await roleService.CreateRoleAsync(createRoleDto);
                return Results.Created($"/{role}", role);
            });
            #endregion

            #region Update role
            // Endpoint để cập nhật thông tin vai trò theo ID
            userGroup.MapPut("/{id:guid}", async (IRoleService roleService, string id, [FromBody] RoleRequestDto updateRoleDto) =>
            {
                if (updateRoleDto == null)
                {
                    return Results.BadRequest("Role data is required.");
                }

                var updatedRole = await roleService.UpdateRoleAsync(id, updateRoleDto);
                if (updatedRole == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(updatedRole);
            });
            #endregion

            #region Delete role
            // Endpoint để xóa vai trò theo ID
            userGroup.MapDelete("/{id:guid}", async (IRoleService roleService, string id) =>
            {
                var success = await roleService.DeleteRoleAsync(id);
                if (!success)
                {
                    return Results.NotFound();
                }
                return Results.NoContent();
            });
            #endregion

            #region Assign Permission
            userGroup.MapPost("/{roleId:guid}/assign-permission", async (IRolePermissionService rolePermissionService, string roleId, string permisisonId) =>
            {
                if (permisisonId.IsNullOrEmpty())
                    return Results.BadRequest("Permission ID is required.");
                var newRolePermission = await rolePermissionService.AddRolePermissionAsync(roleId, permisisonId);
                if (newRolePermission == null)
                    return Results.NotFound();
                return Results.Ok(newRolePermission);
            });
            #endregion

            #region Revoke Permission
            userGroup.MapDelete("/{roleId:guid}/revoke-permission", async (IRolePermissionService rolePermissionService, string roleId, string permisisonId) =>
            {
                if (permisisonId.IsNullOrEmpty())
                    return Results.BadRequest("Permission ID is required.");
                var newRolePermission = await rolePermissionService.RemoveRolePermissionAsync(roleId, permisisonId);
                if (!newRolePermission)
                    return Results.NotFound();
                return Results.Ok();
            });
            #endregion
        }
    }
}
