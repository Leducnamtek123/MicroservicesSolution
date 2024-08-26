using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dtos
{
    public class ErrorDto
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public ErrorDto(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }

}

