using Account.Application.Dtos;
using Account.Domain.Filters;
using Account.Domain.Models;
using Account.Domain.Repositories;
using AutoMapper;
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
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper, RoleManager<Role> roleManager)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<RoleResponseDto> GetRoleByIdAsync(string id, bool IsDeep)
        {
            var role = await _roleRepository.GetByIdAsync(id, IsDeep);
            if (role == null)
            {
                return null; // or throw an exception if preferred
            }

            return _mapper.Map<RoleResponseDto>(role);
        }

        public async Task<RoleResponseDto> CreateRoleAsync(RoleRequestDto roleRequestDto)
        {
            if (roleRequestDto == null)
            {
                throw new ArgumentNullException(nameof(roleRequestDto));
            }

            var role = _mapper.Map<Role>(roleRequestDto);
            role.NormalizedName = role.Name.ToUpper();
            role.Id = Guid.NewGuid().ToString();
            await _roleRepository.AddAsync(role);
            return _mapper.Map<RoleResponseDto>(role);
        }

        public async Task<RoleResponseDto> UpdateRoleAsync(string id, RoleRequestDto roleRequestDto)
        {
            if (roleRequestDto == null)
            {
                throw new ArgumentNullException(nameof(roleRequestDto));
            }

            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return null; // or throw an exception if preferred
            }

            _mapper.Map(roleRequestDto, role);
            await _roleRepository.UpdateAsync(role);
            return _mapper.Map<RoleResponseDto>(role);
        }

        public async Task<bool> DeleteRoleAsync(string id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return false; // or throw an exception if preferred
            }

            await _roleRepository.DeleteAsync(role);
            return true;
        }

        public async Task<PagedDto<RoleResponseDto>> GetPagedRolesAsync(RoleFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var pagedRoles = await _roleRepository.GetPagedAsync(filter);

            var roleResponseDtos = _mapper.Map<IEnumerable<RoleResponseDto>>(pagedRoles.Items);

            var pagedResponse = new PagedDto<RoleResponseDto>(
                roleResponseDtos,
                pagedRoles.TotalCount,
                pagedRoles.PageIndex,
                pagedRoles.PageSize
            );

            return pagedResponse;
        }

        //public async Task<int> DeleteRoleList
    }
}