using Microsoft.EntityFrameworkCore;
using Notification.Application.Interfaces;
using Notification.Application.Services;
using Notification.Domain.Repositories;
using Notification.Infrastructure.Data;
using Notification.Infrastructure.MessageBus.RabbitMQ;
using Notification.Infrastructure.Repositories;
using Notification.Infrastructure.Services;

namespace Notification.API.Extentions
{
    public static class ServiceCollectionExtention
    {
        public static void AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("Invalid connection string");

            services.AddDbContext<NotificationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }

        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<IEmailNotificationRepository, EmailNotificationRepository>();

            services.AddScoped<IEmailNotificationSender, GmailNotificationSender>();

            services.AddSingleton<IProducer, RabbitProducer>();
            services.AddSingleton<IConsumer, RabbitConsumer>();
            services.AddSingleton<ILoggerService, LoggerService>();
        }
    }
}
