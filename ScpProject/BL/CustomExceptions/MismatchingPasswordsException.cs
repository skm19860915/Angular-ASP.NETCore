
using System;

namespace BL.CustomExceptions
{
    public class MismatchingPasswordsException : Exception
    {

        public MismatchingPasswordsException()
        {
        }

        public MismatchingPasswordsException(string message)
            : base(message)
        {
        }

        public MismatchingPasswordsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
