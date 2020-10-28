using System.ComponentModel.DataAnnotations;

namespace RaspSecure.Models.DTO
{
    public sealed class UserLoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,}")]
        public string Password { get; set; }
    }
}
