using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Response
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ApiResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }

    public class ErrorResponse : ApiResponse
    {
        public List<string> Errors { get; set; }

        public ErrorResponse(List<string> errors)
            : base(false, "Validation Errors")
        {
            Errors = errors;
        }
    }
}
