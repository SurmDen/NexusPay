using Microsoft.EntityFrameworkCore;
using Wallet.Application.Interfaces;
using Wallet.Application.Services;
using Wallet.Domain.Repositories;
using Wallet.Infrastructure.Data;
using Wallet.Infrastructure.MessageBus.RabbitMQ;
using Wallet.Infrastructure.Repositories;

namespace Wallet.API.Extentions
{
    public static class ServiceCollectionExtention
    {
        public static void AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("Invalid connection string");

            services.AddDbContext<WalletDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }

        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<IWalletRepository, WalletRepository>();

            services.AddSingleton<IProducer, RabbitProducer>();
            services.AddSingleton<IConsumer, RabbitConsumer>();
            services.AddSingleton<ILoggerService, LoggerService>();
        }
    }
}
