using Account.Domain.Models;
using Common.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Account.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<IEnumerable<User>> GetAllAsync();
        Task AddAsync(User user);
        void Update(User user);
        void Remove(User user);
        Task SaveChangesAsync();
    }
}
