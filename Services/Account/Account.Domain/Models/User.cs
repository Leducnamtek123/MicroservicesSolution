
using Microsoft.AspNetCore.Identity;
namespace Account.Domain.Models
{
    public class User : IdentityUser<string>
    {
        public string? Initials { get; set; }
    }
}
