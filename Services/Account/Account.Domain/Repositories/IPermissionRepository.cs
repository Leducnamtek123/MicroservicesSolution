using Account.Domain.Filters;
using Account.Domain.Models;
using Common.Data;
using Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Repositories
{
    public interface IPermissionRepository : IBaseRepository<Permission>
    {
        Task<Permission?> GetByIdAsync(string id);
        Task<PagedDto<Permission>> GetPagedAsync(PermissionFilter filter);
    }

}
