using System.ComponentModel.DataAnnotations;

namespace Account.Application.Dtos
{
    public class UserRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        // Add additional properties if needed
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters.")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters.")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }

        // Optional: Constructor for initialization
        public UserRequestDto() { }

        public UserRequestDto(string email, string password, string firstName = null, string lastName = null, string phoneNumber = null)
        {
            Email = email;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
        }
    }
}
