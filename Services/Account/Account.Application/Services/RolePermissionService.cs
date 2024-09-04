using Account.Application.Dtos;
using Account.Domain.Models;
using Account.Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public class RolePermissionService:IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly ILogger<RolePermissionService> _logger;
        private readonly IMapper _mapper;

        public RolePermissionService(IRolePermissionRepository rolePermissionRepository, ILogger<RolePermissionService> logger, IMapper mapper)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<RolePermissionResponseDto?> GetRolePermissionByRoleIdAsync(string roleId)
        {
            _logger.LogInformation($"Getting RolePermission for Role ID: {roleId}");
            var rp = await _rolePermissionRepository.GetByRoleIdAsync(roleId);
            return _mapper.Map<RolePermissionResponseDto?>( rp );
        }

        public async Task<RolePermissionResponseDto?> GetRolePermissionByPermissionIdAsync(string permissionId)
        {
            _logger.LogInformation($"Getting RolePermission for Permission ID: {permissionId}");
            var rp = await _rolePermissionRepository.GetByPermissionIdAsync(permissionId);
            return _mapper.Map<RolePermissionResponseDto?>(rp);
        }

        public async Task<List<RolePermissionResponseDto?>> GetAllRolePermissionsByRoleIdAsync(string roleId)
        {
            _logger.LogInformation($"Getting all RolePermissions for Role ID: {roleId}");
            var rp = await _rolePermissionRepository.GetAllByRoleIdAsync(roleId);
            return _mapper.Map<List<RolePermissionResponseDto?>>(rp);
        }

        public async Task<List<RolePermissionResponseDto>> GetAllRolePermissionsByPermissionIdAsync(string permissionId)
        {
            _logger.LogInformation($"Getting all RolePermissions for Permission ID: {permissionId}");
            var rp = await _rolePermissionRepository.GetAllByPermissionIdAsync(permissionId);
            return _mapper.Map<List<RolePermissionResponseDto>>(rp);
        }

        public async Task<RolePermissionResponseDto> AddRolePermissionAsync(string roleId, string permissionId)
        {
            _logger.LogInformation($"Adding RolePermission for Role ID: {roleId} and Permission ID: {permissionId}");

            var existingRolePermission = await _rolePermissionRepository.GetByRoleAndPermissionIdAsync(roleId,permissionId);
            if (existingRolePermission != null)
            {
                _logger.LogWarning($"RolePermission already exists for Role ID: {roleId} and Permission ID: {permissionId}");
                throw new InvalidOperationException("RolePermission already exists for the given Role ID and Permission ID."); // Throw exception with custom message
            }

            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            };

            await _rolePermissionRepository.AddAsync(rolePermission);
            _logger.LogInformation($"RolePermission added successfully for Role ID: {roleId} and Permission ID: {permissionId}");
            return _mapper.Map<RolePermissionResponseDto>(rolePermission);
        }

        public async Task<bool> RemoveRolePermissionAsync(string roleId, string permissionId)
        {
            _logger.LogInformation($"Removing RolePermission for Role ID: {roleId} and Permission ID: {permissionId}");

            var rolePermission = await _rolePermissionRepository.GetByRoleAndPermissionIdAsync(roleId, permissionId);

            if (rolePermission != null)
            {
                await _rolePermissionRepository.DeleteAsync(rolePermission);
                _logger.LogInformation($"RolePermission removed successfully for Role ID: {roleId} and Permission ID: {permissionId}");
                return true;
            }
            else
            {
                _logger.LogWarning($"RolePermission not found for Role ID: {roleId} and Permission ID: {permissionId}");
                return false;
            }
        }

        public async Task<bool> RolePermissionExistsAsync(string roleId, string permissionId)
        {
            _logger.LogInformation($"Checking if RolePermission exists for Role ID: {roleId} and Permission ID: {permissionId}");

            var rolePermission = await _rolePermissionRepository.GetByRoleAndPermissionIdAsync(roleId,permissionId);
            return rolePermission != null;
        }
    }
}
