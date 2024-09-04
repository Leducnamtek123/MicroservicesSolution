using Account.Domain.Filters;
using Account.Domain.Models;
using Account.Domain.Repositories;
using Account.Infrastructure.Context;
using Common.Data;
using Common.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>,IUserRepository
    {
        public UserRepository(AccountDbContext context, ILogger<UserRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            // Retrieve the user from the database using the provided ID
            return await _dbSet.FindAsync(id);
        }

        public async Task<PagedDto<User>> GetPagedAsync(UserFilter filter)
        {
            var query = _dbSet.AsQueryable();

            if (filter.IsDeep)
            {
                query = query
                    .Include(u => u.UserRoles) // Nạp UserRoles
                    .ThenInclude(ur => ur.Role); // Nạp Role qua UserRoles
            }

            // Apply filtering based on keyword
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                query = query.Where(u => u.UserName!.Contains(filter.Keyword)); // Example filter
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                query = filter.IsSortDescending
                    ? query.OrderByDescending(e => EF.Property<object>(e, filter.SortBy))
                    : query.OrderBy(e => EF.Property<object>(e, filter.SortBy));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply paging
            var items = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedDto<User>(items, totalCount, filter.PageIndex, filter.PageSize);
        }

    }
}
