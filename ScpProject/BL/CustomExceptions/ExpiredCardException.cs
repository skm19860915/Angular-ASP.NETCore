using System;

namespace BL.CustomExceptions
{
    public class ExpiredCardException : Exception
    {
        public ExpiredCardException()
        {
        }

        public ExpiredCardException(string message)
            : base(message)
        {
        }

        public ExpiredCardException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
