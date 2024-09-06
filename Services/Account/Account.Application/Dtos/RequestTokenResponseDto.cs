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
        public void SetExpiresFromDateTime(DateTime expirationTime)
        {
            Expires = expirationTime.Ticks;
        }

        // Phương thức để lấy lại DateTime từ giá trị số tick
        public DateTime GetExpirationDateTime()
        {
            return new DateTime(Expires);
        }
    }
}
