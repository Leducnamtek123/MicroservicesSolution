using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Dtos
{
    public class RequestTokenResponseDto
    {
        public string AccessToken { get; set; }
        public long Expires { get; set; }
    }
}
