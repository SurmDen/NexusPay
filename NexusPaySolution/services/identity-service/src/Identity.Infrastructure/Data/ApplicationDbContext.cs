using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(userBuilder =>
            {
                userBuilder.ToTable("users");
                userBuilder.HasKey(x => x.Id);
                userBuilder.HasAlternateKey(x => x.UserEmail);
                userBuilder.HasIndex(u => u.UserEmail);
            });
        }
    }
}
