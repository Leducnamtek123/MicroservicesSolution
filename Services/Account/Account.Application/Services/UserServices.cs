using Account.Application.Dtos;
using Account.Domain.Filters;
using Account.Domain.Models;
using Account.Domain.Repositories;
using AutoMapper;
using Common.Cache;
using Common.Dtos; // Ensure you include this for PagedDto
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
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<UserResponseDto> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return null; // or throw an exception if preferred
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
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> UpdateUserAsync(string id, UserRequestDto userRequestDto)
        {
            if (userRequestDto == null)
            {
                throw new ArgumentNullException(nameof(userRequestDto));
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return null; // or throw an exception if preferred
            }

            _mapper.Map(userRequestDto, user);
            await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return false; // or throw an exception if preferred
            }

            await _userRepository.DeleteAsync(user);
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

        public async Task<PagedDto<UserResponseDto>> GetPagedUsersAsync(UserFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var pagedUsers = await _userRepository.GetPagedAsync(filter);

            var userResponseDtos = _mapper.Map<IEnumerable<UserResponseDto>>(pagedUsers.Items);

            var pagedResponse = new PagedDto<UserResponseDto>(
                userResponseDtos,
                pagedUsers.TotalCount,
                pagedUsers.PageIndex,
                pagedUsers.PageSize
            );

            return pagedResponse;
        }
    }
}
