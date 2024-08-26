
using Microsoft.AspNetCore.Identity;
namespace Account.Domain.Models
{
    public class User : IdentityUser
    {
        public string? Initials { get; set; }
    }
}
