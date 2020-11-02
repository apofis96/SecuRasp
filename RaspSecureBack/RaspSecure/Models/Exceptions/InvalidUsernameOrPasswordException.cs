using System;

namespace RaspSecure.Models.Exceptions
{
    public sealed class InvalidUsernameOrPasswordException : Exception
    {
        public InvalidUsernameOrPasswordException() : base("Неправильная почта или пароль.") { }
    }
}
