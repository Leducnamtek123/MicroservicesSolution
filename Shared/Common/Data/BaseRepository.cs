using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Data
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly ILogger<BaseRepository<T>> _logger;

        public BaseRepository(DbContext context, ILogger<BaseRepository<T>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            _logger.LogInformation($"Getting all entities of type {typeof(T).Name}");
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            _logger.LogInformation($"Adding entity of type {typeof(T).Name}");
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _logger.LogInformation($"Updating entity of type {typeof(T).Name}");
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _logger.LogInformation($"Deleting entity of type {typeof(T).Name}");
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            _logger.LogInformation($"Getting entity of type {typeof(T).Name} with ID {id}");
            return await _dbSet.FindAsync(id);
        }

        public async Task<int> DeleteListAsync(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
            {
                throw new ArgumentException("The list cannot be null or empty", nameof(entities));
            }

            _dbSet.RemoveRange(entities);
            int count = await _context.SaveChangesAsync();
            return count;
        }
    }
}
