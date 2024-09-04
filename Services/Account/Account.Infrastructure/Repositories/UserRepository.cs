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
            // Khởi tạo giá trị mặc định cho filter nếu cần
            filter.PageIndex = filter.PageIndex ?? 1;
            filter.PageSize = filter.PageSize ?? 10;

            var query = _dbSet.AsQueryable();

            // Nạp dữ liệu liên quan nếu IsDeep có giá trị
            if ((bool)filter.IsDeep)
            {
                query = query
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role);
            }

            // Áp dụng bộ lọc từ khóa
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                query = query.Where(u => u.UserName.Contains(filter.Keyword));
            }

            // Áp dụng sắp xếp
            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                query = filter.IsSortDescending.HasValue && filter.IsSortDescending.Value
                    ? query.OrderByDescending(e => EF.Property<object>(e, filter.SortBy))
                    : query.OrderBy(e => EF.Property<object>(e, filter.SortBy));
            }

            // Lấy tổng số bản ghi để phân trang
            var totalCount = await query.CountAsync();

            // Áp dụng phân trang
            var items = await query
                .Skip((filter.PageIndex.Value - 1) * filter.PageSize.Value)
                .Take(filter.PageSize.Value)
                .ToListAsync();

            return new PagedDto<User>(items, totalCount, filter.PageIndex.Value, filter.PageSize.Value);
        }


    }
}
