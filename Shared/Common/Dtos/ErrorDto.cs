using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dtos
{
    public class ErrorDto
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> Errors { get; set; }

        public ErrorDto(string errorCode, string errorMessage, List<string> errors = null)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            Errors = errors ?? new List<string>();
        }
    }
}

