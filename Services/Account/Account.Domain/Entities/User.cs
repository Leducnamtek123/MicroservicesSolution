using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }

        // Constructor
        public User(string userName, string email)
        {
            Id = Guid.NewGuid();
            UserName = userName;
            Email = email;
        }

        // Business logic methods
        public void UpdateEmail(string newEmail)
        {
            // Business rules for updating email
            Email = newEmail;
        }
    }
}
