using Account.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Presentation
{
    public class UnitOfWork 
    {
        private readonly AccountDbContext _context;

        public UnitOfWork(AccountDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
