using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;
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
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Помечаем Value Objects как Complex Types
            modelBuilder.Entity<User>(userBuilder =>
            {
                userBuilder.ToTable("users");
                userBuilder.HasKey(x => x.Id);

                // Complex Type свойства
                userBuilder.ComplexProperty(u => u.UserEmail, e =>
                {
                    e.Property(p => p.Value).HasColumnName("Email").IsRequired();
                });

                userBuilder.ComplexProperty(u => u.Password, p =>
                {
                    p.Property(p => p.Hash).HasColumnName("PasswordHash").IsRequired();
                });

                userBuilder.ComplexProperty(u => u.RoleName, r =>
                {
                    r.Property(p => p.Value).HasColumnName("Role").IsRequired();
                });

                // Остальные настройки
                userBuilder.Property(u => u.UserName).IsRequired();
                userBuilder.Property(u => u.IsActive).IsRequired();
                userBuilder.Property(u => u.CreatedAt).IsRequired();
                userBuilder.Property(u => u.UpdatedAt).IsRequired();
            });
        }
    }
}
