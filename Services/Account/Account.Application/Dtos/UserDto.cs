using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public UserDto(Guid id, string userName, string email)
        {
            Id = id;
            UserName = userName;
            Email = email;
        }
    }
}
