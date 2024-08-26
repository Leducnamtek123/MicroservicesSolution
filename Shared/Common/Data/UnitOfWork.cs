using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private bool _disposed;
        private IDbContextTransaction? _transaction;
        private readonly Dictionary<Type, Lazy<object>> _repositories = new();

        public UnitOfWork(DbContext context, ILogger<UnitOfWork> logger, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _transaction = await _context.Database.BeginTransactionAsync();
            _logger.LogInformation("Transaction started");
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                if (_transaction == null)
                {
                    throw new InvalidOperationException("No transaction in progress to commit.");
                }

                await _transaction.CommitAsync();
                _logger.LogInformation("Transaction committed");
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress to roll back.");
            }

            await _transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back");

            _transaction?.Dispose();
            _transaction = null;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Saving changes to the database");
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public IBaseRepository<T> Repository<T>() where T : class
        {
            if (!_repositories.ContainsKey(typeof(T)))
            {
                var logger = _loggerFactory.CreateLogger<BaseRepository<T>>();
                _repositories[typeof(T)] = new Lazy<object>(() => new BaseRepository<T>(_context, logger));
            }

            return (IBaseRepository<T>)_repositories[typeof(T)].Value;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                    _logger.LogInformation("Resources disposed");
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
