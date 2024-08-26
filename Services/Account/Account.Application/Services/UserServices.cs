
using Account.Application.Dtos;
using Account.Domain.Models;
using Account.Domain.Repositories;
using AutoMapper;
using Common.Cache;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public UserService(IUserRepository userRepository, IMapper mapper, ICacheService cacheService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cacheService = cacheService;

        }

        public async Task<UserResponseDto> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                // Có thể ném ra một ngoại lệ hoặc trả về null nếu không tìm thấy người dùng
                return null;
            }

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> CreateUserAsync(UserRequestDto userRequestDto)
        {
            if (userRequestDto == null)
            {
                throw new ArgumentNullException(nameof(userRequestDto));
            }

            var user = _mapper.Map<User>(userRequestDto);
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> UpdateUserAsync(Guid id, UserRequestDto userRequestDto)
        {
            if (userRequestDto == null)
            {
                throw new ArgumentNullException(nameof(userRequestDto));
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                // Có thể ném ra một ngoại lệ hoặc trả về null nếu không tìm thấy người dùng
                return null;
            }

            _mapper.Map(userRequestDto, user);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                // Nếu không tìm thấy người dùng, trả về false
                return false;
            }

            _userRepository.Remove(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetUserTokenAsync(string userId)
        {
            return await _cacheService.GetCacheAsync(userId);
        }

        public async Task SetUserTokenAsync(string userId, string token, TimeSpan expiration)
        {
            await _cacheService.SetCacheAsync(userId, token, expiration);
        }
    }
}
