using System.Collections.Generic;

namespace RaspSecure.Models.DTO
{
    public sealed class UsersListD
    {
        public ICollection<UserDTO> Users { get; set; }
        public int Items { get; set; }
    }
}
