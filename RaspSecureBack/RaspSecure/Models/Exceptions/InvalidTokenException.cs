using System;

namespace RaspSecure.Models.Exceptions
{
    public sealed class InvalidTokenException : Exception
    {
        public InvalidTokenException(string tokenName) : base($"Invalid {tokenName} token.") { }
    }
}
