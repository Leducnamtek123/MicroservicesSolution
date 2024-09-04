
using Microsoft.AspNetCore.Identity;
namespace Account.Domain.Models
{
    public class User : IdentityUser
    {
        public User() : base() { }

        public User(string name)
            : base(name)
        {
        }
        public string? Initials { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }

    }
}
