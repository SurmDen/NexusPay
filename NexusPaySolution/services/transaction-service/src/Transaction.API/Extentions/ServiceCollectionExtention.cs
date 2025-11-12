using Microsoft.EntityFrameworkCore;
using Transaction.Application.Interfaces;
using Transaction.Application.Services;
using Transaction.Domain.Repositories;
using Transaction.Infrastructure.Data;
using Transaction.Infrastructure.MessageBus.RabbitMQ;
using Transaction.Infrastructure.Repositories;

namespace Wallet.API.Extentions
{
    public static class ServiceCollectionExtention
    {
        public static void AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("Invalid connection string");

            services.AddDbContext<TransactionDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }

        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<ITransactionRepository, TransactionRepository>();

            services.AddSingleton<IProducer, RabbitProducer>();
            services.AddSingleton<IConsumer, RabbitConsumer>();
            services.AddSingleton<ILoggerService, LoggerService>();
        }
    }
}
