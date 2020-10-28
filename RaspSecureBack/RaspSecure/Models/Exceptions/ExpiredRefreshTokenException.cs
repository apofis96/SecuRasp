using System;

namespace RaspSecure.Models.Exceptions
{
    public sealed class ExpiredRefreshTokenException : Exception
    {
        public ExpiredRefreshTokenException() : base("Refresh token expired.") { }
    }
}
