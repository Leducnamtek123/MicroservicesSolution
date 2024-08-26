using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Presentation
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
