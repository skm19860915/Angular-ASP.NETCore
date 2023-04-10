using System;

namespace BL.CustomExceptions
{
    public class ItemAlreadyExistsException : Exception
    {
        public ItemAlreadyExistsException()
        {
        }

        public ItemAlreadyExistsException(string message)
            : base(message)
        {
        }

        public ItemAlreadyExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
