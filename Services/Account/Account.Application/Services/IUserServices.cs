using Account.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserByIdAsync(Guid id);
        Task<UserResponseDto> CreateUserAsync(UserRequestDto userRequestDto);
        Task<UserResponseDto> UpdateUserAsync(Guid id, UserRequestDto userRequestDto);
        Task<bool> DeleteUserAsync(Guid id);
    }
}