using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Models
{
    public class Permission
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }

}
