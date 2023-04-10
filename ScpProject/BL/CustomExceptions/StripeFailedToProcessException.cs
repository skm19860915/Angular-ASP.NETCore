using System;

namespace BL.CustomExceptions
{
    public class StripeFailedToProcessException : Exception
    {
        public StripeFailedToProcessException()
        {
        }

        public StripeFailedToProcessException(string message) : base(message)
        {
        }

        public StripeFailedToProcessException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
