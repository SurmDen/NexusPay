using Identity.Application.Interfaces;
using Identity.Application.Services;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.MessageBus.RabbitMQ;
using Identity.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Identity.API.Extentions
{
    public static class ServiceCollectionExtention
    {
        public static void AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("Invalid connection string");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }

        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddSingleton<ITokenService, JwtTokenService>();
            services.AddSingleton<IProducer, RabbitProducer>();
            services.AddSingleton<IConsumer, RabbitConsumer>();
            services.AddSingleton<ICodeGenerator, CodeGenerator>();
        }
    }
}
