using Account.Application.Dtos;
using Account.Domain.Filters;
using Account.Domain.Models;
using Account.Domain.Repositories;
using AutoMapper;
using Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public PermissionService(IPermissionRepository permissionRepository, IMapper mapper)
        {
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PermissionResponseDto> GetPermissionByIdAsync(string id)
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission == null)
            {
                return null; // or throw an exception if preferred
            }

            return _mapper.Map<PermissionResponseDto>(permission);
        }

        public async Task<PermissionResponseDto> CreatePermissionAsync(PermissionRequestDto permissionRequestDto)
        {
            if (permissionRequestDto == null)
            {
                throw new ArgumentNullException(nameof(permissionRequestDto));
            }

            var permission = _mapper.Map<Permission>(permissionRequestDto);
            permission.Id = Guid.NewGuid().ToString();
            await _permissionRepository.AddAsync(permission);
            return _mapper.Map<PermissionResponseDto>(permission);
        }

        public async Task<PermissionResponseDto> UpdatePermissionAsync(string id, PermissionRequestDto permissionRequestDto)
        {
            if (permissionRequestDto == null)
            {
                throw new ArgumentNullException(nameof(permissionRequestDto));
            }

            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission == null)
            {
                return null; // or throw an exception if preferred
            }

            _mapper.Map(permissionRequestDto, permission);
            await _permissionRepository.UpdateAsync(permission);
            return _mapper.Map<PermissionResponseDto>(permission);
        }

        public async Task<bool> DeletePermissionAsync(string id)
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission == null)
            {
                return false; // or throw an exception if preferred
            }

            await _permissionRepository.DeleteAsync(permission);
            return true;
        }

        public async Task<PagedDto<PermissionResponseDto>> GetPagedPermissionsAsync(PermissionFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var pagedPermissions = await _permissionRepository.GetPagedAsync(filter);

            var permissionResponseDtos = _mapper.Map<IEnumerable<PermissionResponseDto>>(pagedPermissions.Items);

            var pagedResponse = new PagedDto<PermissionResponseDto>(
                permissionResponseDtos,
                pagedPermissions.TotalCount,
                pagedPermissions.PageIndex,
                pagedPermissions.PageSize
            );

            return pagedResponse;
        }
    }
}
