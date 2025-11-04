using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Infrastructure.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base (options)
        {
            Database.EnsureCreated();
        }

        public DbSet<EmailNotification> EmailNotification { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailNotification>(builder =>
            {
                builder.ToTable("email_notifications");
                builder.HasKey(e => e.Id);
            });
        }
    }
}
