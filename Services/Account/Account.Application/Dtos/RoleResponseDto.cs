using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Dtos
{
    public class RoleResponseDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public IEnumerable<PermissionResponseDto> Permissions { get; set; } = new List<PermissionResponseDto>();

    }
}
