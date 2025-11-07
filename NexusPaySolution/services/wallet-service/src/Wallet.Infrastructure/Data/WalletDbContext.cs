using Microsoft.EntityFrameworkCore;
using Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Data
{
    public class WalletDbContext : DbContext
    {
        public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<WalletModel> Wallets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WalletModel>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.HasAlternateKey(x => x.UserId);
                builder.ToTable("wallets");
            });
        }
    }
}
