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
        public string Description { get; set; } // Ví dụ thêm thuộc tính mô tả

    }
}
