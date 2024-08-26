using Account.Domain.Filters;
using Account.Domain.Models;
using Account.Domain.Repositories;
using Account.Infrastructure.Context;
using Common.Data;
using Common.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Infrastructure.Repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(AccountDbContext context, ILogger<RoleRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<Role?> GetByIdAsync(string id)
        {
            // Retrieve the role from the database using the provided ID
            return await _dbSet.FindAsync(id);
        }

        public async Task<PagedDto<Role>> GetPagedAsync(RoleFilter filter)
        {
            var query = _dbSet.AsQueryable();

            // Apply filtering based on keyword (if necessary)
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                query = query.Where(r => r.Name.Contains(filter.Keyword)); // Example filter
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

            return new PagedDto<Role>(items, totalCount, filter.PageIndex, filter.PageSize);
        }
    }
}
