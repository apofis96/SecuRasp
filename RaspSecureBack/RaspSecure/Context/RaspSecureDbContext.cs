using Microsoft.EntityFrameworkCore;
using RaspSecure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaspSecure.Context
{
    public class RaspSecureDbContext: DbContext
    {
        public RaspSecureDbContext(DbContextOptions<RaspSecureDbContext> options) : base(options) { }
        public DbSet<SecurityCode> SecurityCodes { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ResetToken> ResetToken { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Contact>()
            //    .HasOne(a => a.PinnedMessage)
            //    .WithMany();
            //.WithOne(a => a.Contact)
            //.HasForeignKey<DirectMessage>(d => d.ContactId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
