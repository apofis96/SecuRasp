using RaspSecure.Models.Auth;
using System;

namespace RaspSecure.Models
{
    public sealed class UserEntity
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public RolesEnum Role { get; set; }
    }
}
