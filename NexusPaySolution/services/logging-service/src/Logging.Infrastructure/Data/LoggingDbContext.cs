using Logging.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Infrastructure.Data
{
    public class LoggingDbContext : DbContext
    {
        public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<LogMessage> LogMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogMessage>(logBuilder =>
            {
                logBuilder.ToTable("logs");
                logBuilder.HasKey(l => l.Id);
            });
        }
    }
}
