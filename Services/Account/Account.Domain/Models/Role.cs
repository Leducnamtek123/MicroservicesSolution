using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Models
{
    public class Role : IdentityRole<string>
    {
        public Role() : base() { }

        public Role(string name)
            : base(name)
        {
        }
        public virtual ICollection<UserRole> UserRoles { get; set; }

        public string Description { get; set; } // Ví dụ thêm thuộc tính mô tả

    }
}
