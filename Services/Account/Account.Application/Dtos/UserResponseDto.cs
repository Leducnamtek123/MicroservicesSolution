using System;

namespace Account.Application.Dtos
{
    public class UserResponseDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? NormalizedEmail { get; set; }
        public IEnumerable<RoleResponseDto> Roles { get; set; } // Ensure this matches

    }
}
