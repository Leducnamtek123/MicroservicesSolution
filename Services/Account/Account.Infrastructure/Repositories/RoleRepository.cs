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

        public async Task<Role?> GetByIdAsync(string id, bool IsDeep = false)
        {
            // Retrieve the role from the database using the provided ID
            if (!IsDeep)
            return await _dbSet.FindAsync(id);
            return await _dbSet
                .AsNoTracking()
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<PagedDto<Role>> GetPagedAsync(RoleFilter filter)
        {
            // Đảm bảo filter không phải là null và khởi tạo giá trị mặc định nếu cần
            filter.PageIndex = filter.PageIndex ?? 1;
            filter.PageSize = filter.PageSize ?? 10;
            if (!filter.IsDeep.HasValue)
                filter.IsDeep = false;

            var query = _dbSet.AsQueryable();

            // Nạp dữ liệu liên quan nếu IsDeep là true
            if ((bool)filter.IsDeep)
            {
                query = query
                    .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission);
            }

            // Áp dụng bộ lọc từ khóa
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                query = query.Where(r => r.Name.Contains(filter.Keyword));
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

            return new PagedDto<Role>(items, totalCount, filter.PageIndex.Value, filter.PageSize.Value);
        }
    }
}
