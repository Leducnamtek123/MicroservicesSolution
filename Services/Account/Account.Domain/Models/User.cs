
using Microsoft.AspNetCore.Identity;
namespace Account.Domain.Models
{
    public class User : IdentityUser
    {
        public string? Initials { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

    }
}
