
using Microsoft.AspNetCore.Identity;
namespace AccountService.Domain.Models
{
    public class User : IdentityUser
    {
        public string? Initials { get; set; }
    }
}
