using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
