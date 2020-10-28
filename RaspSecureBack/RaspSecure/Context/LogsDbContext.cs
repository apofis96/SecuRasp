using Microsoft.EntityFrameworkCore;
using RaspSecure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaspSecure.Context
{
    public class LogsDbContext: DbContext
    {
        public LogsDbContext(DbContextOptions<LogsDbContext> options) : base(options) { }
        public DbSet<LogEntity> LogEntitys { get; set; }

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
