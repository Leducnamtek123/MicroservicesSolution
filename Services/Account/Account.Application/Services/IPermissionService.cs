using Account.Application.Dtos;
using Account.Domain.Filters;
using Common.Dtos;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public interface IPermissionService
    {
        Task<PermissionResponseDto> GetPermissionByIdAsync(string id);
        Task<PermissionResponseDto> CreatePermissionAsync(PermissionRequestDto permissionRequestDto);
        Task<PermissionResponseDto> UpdatePermissionAsync(string id, PermissionRequestDto permissionRequestDto);
        Task<bool> DeletePermissionAsync(string id);
        Task<PagedDto<PermissionResponseDto>> GetPagedPermissionsAsync(PermissionFilter filter);
    }
}
