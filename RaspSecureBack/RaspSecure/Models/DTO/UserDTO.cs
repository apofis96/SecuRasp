using RaspSecure.Models.Auth;
using System.ComponentModel.DataAnnotations;

namespace RaspSecure.Models.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(1)]
        public string UserName { get; set; }
        public RolesEnum Role { get; set; }
    }
}
