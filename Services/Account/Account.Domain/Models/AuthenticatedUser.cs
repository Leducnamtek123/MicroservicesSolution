using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Models
{
    public class AuthenticatedUser
    {
        public User User { get; set; }
        public Token Token { get; set; }
    }
}
