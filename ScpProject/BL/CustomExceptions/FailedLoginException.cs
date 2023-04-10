using System;

namespace BL.CustomExceptions
{
    public class FailedLoginException : Exception
    {
        public FailedLoginException()
        {
        }

        public FailedLoginException(string message)
            : base(message)
        {
        }

        public FailedLoginException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
