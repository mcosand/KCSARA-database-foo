using System;

namespace Kcsar.Database.Web
{
    public class InvalidPasswordException : ApplicationException
    {
        public InvalidPasswordException(string message)
            : base(message)
        {
        }
    }
}