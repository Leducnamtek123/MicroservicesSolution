using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Models
{
    public class RefreshToken
    {
        public string Id { get; set; } // ID của token (có thể là mã hash của token)
        public string UserId { get; set; } // ID của người dùng
        public DateTime CreatedAt { get; set; } // Thời điểm token được tạo
        public string TokenCode { get; set; } // Thời điểm token được tạo
        public DateTime Expiry { get; set; } // Thời điểm hết hạn của token
    }
}
