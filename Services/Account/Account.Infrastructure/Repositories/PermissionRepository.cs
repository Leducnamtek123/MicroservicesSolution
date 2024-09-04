using Account.Domain.Filters;
using Account.Domain.Models;
using Account.Domain.Repositories;
using Account.Infrastructure.Context;
using Common.Data;
using Common.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Account.Infrastructure.Repositories
{
    public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(AccountDbContext context, ILogger<PermissionRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<Permission?> GetByIdAsync(string id)
        {
            _logger.LogInformation($"Getting Permission with ID {id}");
            return await _dbSet
                .AsNoTracking() // Optional: improve performance for read-only queries
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PagedDto<Permission>> GetPagedAsync(PermissionFilter filter)
        {
            _logger.LogInformation("Getting paged Permissions");

            // Kiểm tra và khởi tạo giá trị mặc định cho filter nếu null
            filter ??= new PermissionFilter(); // Khởi tạo filter nếu nó là null

            // Sử dụng giá trị mặc định nếu PageIndex hoặc PageSize là null
            int pageIndex = filter.PageIndex ?? 1;
            int pageSize = filter.PageSize ?? 10;

            // Kiểm tra giá trị PageIndex và PageSize để đảm bảo chúng hợp lệ
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _dbSet.AsQueryable();

            // Apply filtering based on Keyword
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(p => p.Name.Contains(filter.Keyword));
            }

            // Calculate total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedDto<Permission>(items, totalCount, pageIndex, pageSize);
        }

    }
}
