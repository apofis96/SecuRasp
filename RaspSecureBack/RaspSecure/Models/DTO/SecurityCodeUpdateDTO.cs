using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaspSecure.Models.DTO
{
    public class SecurityCodeUpdateDTO
    {
        public int SecurityCodeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset ExpiredAt { get; set; }
    }
}
