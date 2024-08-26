using Account.Domain.Filters;
using Account.Domain.Models;
using Common.Data;
using Common.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Account.Domain.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByIdAsync(string id);
        Task<PagedDto<User>> GetPagedAsync(UserFilter filter);
    }
}
