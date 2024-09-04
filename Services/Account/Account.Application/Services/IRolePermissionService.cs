using Account.Application.Dtos;
using Account.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public interface IRolePermissionService
    {
        Task<RolePermissionResponseDto?> GetRolePermissionByRoleIdAsync(string roleId);
        Task<RolePermissionResponseDto?> GetRolePermissionByPermissionIdAsync(string permissionId);
        Task<List<RolePermissionResponseDto?>> GetAllRolePermissionsByRoleIdAsync(string roleId);
        Task<List<RolePermissionResponseDto?>> GetAllRolePermissionsByPermissionIdAsync(string permissionId);
        Task<RolePermissionResponseDto?> AddRolePermissionAsync(string roleId, string permissionId);
        Task<bool> RemoveRolePermissionAsync(string roleId, string permissionId);
        Task<bool> RolePermissionExistsAsync(string roleId, string permissionId);
        
    }
}