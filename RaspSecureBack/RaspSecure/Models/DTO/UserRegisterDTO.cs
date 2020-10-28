using System.ComponentModel.DataAnnotations;

namespace RaspSecure.Models.DTO
{
    public sealed class UserRegisterDTO : UserDTO
    {
        [Required]
        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,}")]
        public string Password { get; set; }
    }
}
