using Account.Application.Dtos;
using Account.Application.Services;
using Account.Domain.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Account.API.Endpoints
{
    public static class RoleEndpoints
    {
        public static void Map(WebApplication app)
        {
            // Endpoint để lấy danh sách các vai trò phân trang
            app.MapGet("/roles", async ([FromServices] IRoleService roleService, [AsParameters] RoleFilter filter) =>
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
            }).WithOpenApi()
            .RequireAuthorization();

            // Endpoint để lấy thông tin vai trò theo ID
            app.MapGet("/roles/{id:guid}", async (IRoleService roleService, string id) =>
            {
                var role = await roleService.GetRoleByIdAsync(id);
                if (role == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(role);
            }).WithOpenApi()
            .RequireAuthorization();

            // Endpoint để tạo một vai trò mới
            app.MapPost("/roles", async (IRoleService roleService, [FromBody] RoleRequestDto createRoleDto) =>
            {
                if (createRoleDto == null)
                {
                    return Results.BadRequest("Role data is required.");
                }

                var role = await roleService.CreateRoleAsync(createRoleDto);
                return Results.Created($"/roles/{role}", role);
            }).WithOpenApi()
            .RequireAuthorization();

            // Endpoint để cập nhật thông tin vai trò theo ID
            app.MapPut("/roles/{id:guid}", async (IRoleService roleService, string id, [FromBody] RoleRequestDto updateRoleDto) =>
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
            }).WithOpenApi()
            .RequireAuthorization();

            // Endpoint để xóa vai trò theo ID
            app.MapDelete("/roles/{id:guid}", async (IRoleService roleService, string id) =>
            {
                var success = await roleService.DeleteRoleAsync(id);
                if (!success)
                {
                    return Results.NotFound();
                }
                return Results.NoContent();
            }).WithOpenApi()
            .RequireAuthorization();
        }
    }
}
