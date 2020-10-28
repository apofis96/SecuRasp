using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaspSecure.Models
{
    public class SecurityCode
    {
        public int SecurityCodeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public UserEntity Issuer { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ExpiredAt { get; set; }
    }
}
