using Account.Application.Dtos;
using Account.Domain.Filters;
using Common.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public interface IUserService
    {
        Task<BaseResponse<UserResponseDto>> GetUserByIdAsync(string id);
        Task<BaseResponse<UserResponseDto>> CreateUserAsync(UserRequestDto userRequestDto);
        Task<BaseResponse<UserResponseDto>> UpdateUserAsync(string id, UserRequestDto userRequestDto);
        Task<BaseResponse<bool>> DeleteUserAsync(string id);
        Task<PagedDto<UserResponseDto>> GetPagedUsersAsync(UserFilter filter); // Đã điều chỉnh để sử dụng BaseResponse
        Task<BaseResponse<bool>> AssignRoleToUserAsync(string userId, string roleName);
        Task<BaseResponse<bool>> AssignRolesToUserAsync(string userId, IEnumerable<string> roleNames);

        Task<BaseResponse<bool>> RemoveRoleFromUserAsync(string userId, string roleName);
        Task<BaseResponse<bool>> UpdateUserRolesAsync(string userId, IEnumerable<string> roleNames);
    }
}