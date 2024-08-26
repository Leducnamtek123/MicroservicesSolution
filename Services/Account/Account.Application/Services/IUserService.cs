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
        Task<UserResponseDto> GetUserByIdAsync(string id);
        Task<UserResponseDto> CreateUserAsync(UserRequestDto userRequestDto);
        Task<UserResponseDto> UpdateUserAsync(string id, UserRequestDto userRequestDto);
        Task<bool> DeleteUserAsync(string id);
        Task<PagedDto<UserResponseDto>> GetPagedUsersAsync(UserFilter filter); // Add this method
    }
}
