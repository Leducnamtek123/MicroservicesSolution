using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException(string message) : base(message) { }
    }

    public class NotFoundException : CustomException
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class ValidationException : CustomException
    {
        public ValidationException(string message) : base(message) { }
    }
}
