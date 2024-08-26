

using Account.Domain.Repositories;
using Account.Infrastructure.Context;
using Account.Infrastructure.Repositories;

namespace Account.Infrastructure.Factories
{
    public class RepositoryFactory
    {
        private readonly AccountDbContext _context;

        public RepositoryFactory(AccountDbContext context)
        {
            _context = context;
        }

        public IUserRepository CreateUserRepository()
        {
            return new UserRepository(_context);
        }
    }
}
