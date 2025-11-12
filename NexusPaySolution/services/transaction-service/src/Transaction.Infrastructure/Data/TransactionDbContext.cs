using Microsoft.EntityFrameworkCore;
using Transaction.Domain.Entities;

namespace Transaction.Infrastructure.Data
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<TransactionInfo> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionInfo>(transactionBuilder =>
            {
                transactionBuilder.HasKey(t => t.Id);
                transactionBuilder.ToTable("transactions");
            });
        }
    }
}
