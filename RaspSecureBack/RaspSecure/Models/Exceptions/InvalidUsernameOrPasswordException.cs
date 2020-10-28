using System;

namespace RaspSecure.Models.Exceptions
{
    public sealed class InvalidUsernameOrPasswordException : Exception
    {
        public InvalidUsernameOrPasswordException() : base("Invalid username or password.") { }
    }
}
