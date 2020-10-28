namespace RaspSecure.Models.Exceptions
{
    public enum ErrorCode
    {
        General,
        NotFound,
        InvalidUsernameOrPassword,
        InvalidToken,
        ExpiredRefreshToken
    }
}
