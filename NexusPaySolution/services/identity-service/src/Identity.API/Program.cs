using Identity.API.Extentions;
using Identity.Application.User.Commands;
using Identity.Domain.Events;
using Identity.Infrastructure.MessageBus.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddLogging();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Identity Service - NexusPay",
        Version = "v1",
        Description = "Microservice for authentication and user management - NexusPay",
        Contact = new OpenApiContact
        {
            Name = "NexusPay Development Team",
            Email = "surmanidzedenis609@gmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "NexusPay License"
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddLogging();

builder.Services.AddHttpClient();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    config.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
    config.RegisterServicesFromAssembly(typeof(NotificationEvent).Assembly);
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddApplicationDbContext(builder.Configuration);

builder.Services.AddCustomServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Service API v1");
        options.DocumentTitle = "NexusPay Identity Service";
        options.RoutePrefix = "api-docs";
        options.DisplayRequestDuration();
        options.EnablePersistAuthorization();
        options.EnableDeepLinking();
        options.EnableFilter();
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCaching();

app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});

app.MapControllers();

app.Run();