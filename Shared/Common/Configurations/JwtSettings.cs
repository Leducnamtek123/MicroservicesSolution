using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Configurations
{
    public class JwtSettings
    {
        public string ConnectionString { get; set; }
        public string JwtSecret { get; set; }
    }
}
