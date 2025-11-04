using Logging.Application.Interfaces;
using Logging.Application.Services;
using Logging.Domain.Repositories;
using Logging.Infrastructure.Data;
using Logging.Infrastructure.MessageBus.RabbitMQ;
using Logging.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Logging.API.Extentions
{
    public static class ServiceCollectionExtention
    {
        public static void AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("Invalid connection string");

            services.AddDbContext<LoggingDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }

        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<ILoggingRepository, LoggingRepository>();
            services.AddSingleton<IConsumer, RabbitConsumer>();
            services.AddSingleton<ILoggerService, LoggerService>();
        }
    }
}
