using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dtos
{
    public class ResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ResponseDto(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ResponseDto<T> SuccessResult(T data, string message = "Operation Successful")
        {
            return new ResponseDto<T>(true, message, data);
        }

        public static ResponseDto<T> FailureResult(string message)
        {
            return new ResponseDto<T>(false, message, default);
        }
    }
}