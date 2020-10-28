﻿using System;

namespace RaspSecure.Models
{
    public sealed class ResetToken
    {
        private const int DAYS_TO_EXPIRE = 1;

        public ResetToken()
        {
            Expires = DateTime.UtcNow.AddDays(DAYS_TO_EXPIRE);
        }
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public string Token { get; set; }
        public DateTime Expires { get; private set; }

        public int UserId { get; set; }
        public UserEntity User { get; set; }

        public bool IsActive => DateTime.UtcNow <= Expires;
    }
}
