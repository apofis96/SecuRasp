using Newtonsoft.Json;
using System;

namespace RaspSecure.Models.DTO
{
    public sealed class RefreshTokenDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
