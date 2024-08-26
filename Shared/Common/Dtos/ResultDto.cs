using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dtos
{
    public class ResultDto<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

        public static ResultDto<T> Success(T data, string message = null)
        {
            return new ResultDto<T> { IsSuccess = true, Data = data, Message = message };
        }

        public static ResultDto<T> Failure(string message)
        {
            return new ResultDto<T> { IsSuccess = false, Message = message };
        }
    }
}