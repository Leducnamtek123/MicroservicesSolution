using Account.Domain.Models;
using Account.Domain.Repositories;
using Account.Infrastructure.Context;
using Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Infrastructure.Repositories
{
    public class RolePermissionRepository : BaseRepository<RolePermission>, IRolePermissionRepository
    {
        public RolePermissionRepository(AccountDbContext context, ILogger<RolePermissionRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<List<RolePermission>> GetByRoleIdAsync(string roleId)
        {
            _logger.LogInformation($"Getting RolePermissions with Role ID {roleId}");
            return await _dbSet
                .AsNoTracking() // Improves performance for read-only queries
                .Where(rp => rp.RoleId == roleId) // Fetch all RolePermissions with the specified RoleId
                .ToListAsync(); // Use ToListAsync to get a list
        }

        public async Task<List<RolePermission>> GetByPermissionIdAsync(string permissionId)
        {
            _logger.LogInformation($"Getting RolePermissions with Permission ID {permissionId}");
            return await _dbSet
                .AsNoTracking() // Improves performance for read-only queries
                .Where(rp => rp.PermissionId == permissionId) // Fetch all RolePermissions with the specified PermissionId
                .ToListAsync(); // Use ToListAsync to get a list
        }

        public async Task<RolePermission?> GetByRoleAndPermissionIdAsync(string roleId, string permissionId)
        {
            _logger.LogInformation($"Getting RolePermission with Role ID {roleId} and Permission ID {permissionId}");
            return await _dbSet
                .AsNoTracking() // Improves performance for read-only queries
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId); // Fetch the RolePermission with both RoleId and PermissionId
        }

        // Optionally, you can add more methods as per your application's requirements
        public async Task<List<RolePermission>> GetAllByRoleIdAsync(string roleId)
        {
            _logger.LogInformation($"Getting all RolePermissions for Role ID {roleId}");
            return await _dbSet
                .Where(rp => rp.RoleId == roleId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<RolePermission>> GetAllByPermissionIdAsync(string permissionId)
        {
            _logger.LogInformation($"Getting all RolePermissions for Permission ID {permissionId}");
            return await _dbSet
                .Where(rp => rp.PermissionId == permissionId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
