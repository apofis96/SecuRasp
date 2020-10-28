using System;

namespace RaspSecure.Models.DTO
{
    public class SecurityCodeCreateDTO
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset ExpiredAt { get; set; }
    }
}
