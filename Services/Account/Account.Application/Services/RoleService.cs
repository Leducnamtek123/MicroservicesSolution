using Account.Application.Dtos;
using Account.Domain.Filters;
using Account.Domain.Models;
using Common.Dtos;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleService(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<RoleResponseDto> CreateRoleAsync(RoleRequestDto roleRequestDto)
        {
            var role = new Role { Name = roleRequestDto.Name };

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return new RoleResponseDto
                {
                    Id = role.Id.ToString(), // Chuyển đổi Guid thành string
                    Name = role.Name
                };
            }

            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<bool> DeleteRoleAsync(string id)
        {
            if (!Guid.TryParse(id, out var roleId))
            {
                return false;
            }

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null) return false;

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }

        public async Task<PagedDto<RoleResponseDto>> GetPagedRolesAsync(RoleFilter filter)
        {
            var roles = _roleManager.Roles
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var totalCount = _roleManager.Roles.Count();

            var roleDtos = roles.Select(role => new RoleResponseDto
            {
                Id = role.Id.ToString(), // Chuyển đổi Guid thành string
                Name = role.Name
            });

            return new PagedDto<RoleResponseDto>(roleDtos, totalCount, filter.PageIndex, filter.PageSize);
        }

        public async Task<RoleResponseDto> GetRoleByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var roleId))
            {
                return null;
            }

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null) return null;

            return new RoleResponseDto
            {
                Id = role.Id.ToString(), // Chuyển đổi Guid thành string
                Name = role.Name
            };
        }

        public async Task<RoleResponseDto> UpdateRoleAsync(string id, RoleRequestDto roleRequestDto)
        {
            if (!Guid.TryParse(id, out var roleId))
            {
                return null;
            }

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null) return null;

            role.Name = roleRequestDto.Name;

            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return new RoleResponseDto
                {
                    Id = role.Id.ToString(), // Chuyển đổi Guid thành string
                    Name = role.Name
                };
            }

            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
