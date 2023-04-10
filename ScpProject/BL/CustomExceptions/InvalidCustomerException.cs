using System;

namespace BL.CustomExceptions
{
    public class InvalidCustomerException : Exception
    {
        public InvalidCustomerException()
        {
        }

        public InvalidCustomerException(string message)
            : base(message)
        {
        }

        public InvalidCustomerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
