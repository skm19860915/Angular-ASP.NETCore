using System;

namespace BL.CustomExceptions
{
    public class ItemValidationError : Exception
    {
        public ItemValidationError()
        {
        }

        public ItemValidationError(string message)
            : base(message)
        {
        }

        public ItemValidationError(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
