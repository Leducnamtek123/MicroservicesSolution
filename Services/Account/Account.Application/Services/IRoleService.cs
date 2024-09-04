using Account.Application.Dtos;
using Account.Domain.Filters;
using Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public interface IRoleService
    {
        Task<RoleResponseDto> GetRoleByIdAsync(string id, bool IsDeep);
        Task<RoleResponseDto> CreateRoleAsync(RoleRequestDto roleRequestDto);
        Task<RoleResponseDto> UpdateRoleAsync(string id, RoleRequestDto roleRequestDto);
        Task<bool> DeleteRoleAsync(string id);
        Task<PagedDto<RoleResponseDto>> GetPagedRolesAsync(RoleFilter filter);

    }
}
