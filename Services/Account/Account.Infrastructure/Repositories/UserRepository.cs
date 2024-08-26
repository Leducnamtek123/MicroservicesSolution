
using Account.Domain.Models;
using Account.Domain.Repositories;
using Account.Infrastructure.Context;
using Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AccountDbContext context) : base(context)
        {
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);  // Sử dụng _dbSet từ BaseRepository
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();  // Sử dụng _dbSet từ BaseRepository
        }

        public async Task AddAsync(User user)
        {
            await _dbSet.AddAsync(user);  // Sử dụng _dbSet từ BaseRepository
            await SaveChangesAsync();
        }

        public void Update(User user)
        {
            _dbSet.Update(user);  // Sử dụng _dbSet từ BaseRepository
            SaveChangesAsync().Wait();  // Đợi để đảm bảo thay đổi được lưu
        }

        public void Remove(User user)
        {
            _dbSet.Remove(user);  // Sử dụng _dbSet từ BaseRepository
            SaveChangesAsync().Wait();  // Đợi để đảm bảo thay đổi được lưu
        }
    }
}
