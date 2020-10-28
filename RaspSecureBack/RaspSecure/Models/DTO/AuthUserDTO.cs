namespace RaspSecure.Models.DTO
{
    public sealed class AuthUserDTO
    {
        public UserDTO User { get; set; }
        public AccessTokenDTO Token { get; set; }
    }
}
