using System;

namespace RaspSecure.Models
{
    public class LogEntity
    {
        public int LogEntityId { get; set; }
        public string Code { get; set; }
        public DateTimeOffset AccessTime { get; set; }
        public bool IsSucceed { get; set; }
        public int SecurityCodeId { get; set; }
    }
}
