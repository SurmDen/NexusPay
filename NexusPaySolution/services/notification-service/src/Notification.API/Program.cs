using Notification.API.Extentions;
using Notification.Application.EmailNotification.Command;
using Notification.Application.Interfaces;
using Notification.Domain.Entities;
using Notification.Domain.Events;
using Notification.Infrastructure.MessageBus.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Notification Service - NexusPay",
        Version = "v1",
        Description = "Microservice for notifications and newsletters NexusPay",
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
    config.RegisterServicesFromAssembly(typeof(CreateNotificationCommand).Assembly);
    config.RegisterServicesFromAssembly(typeof(EmailNotification).Assembly);
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
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification Service API v1");
        options.DocumentTitle = "NexusPay Notification Service";
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

int retry = 0;

while (retry < 5)
{
    try
    {
        var consumer = app.Services.GetRequiredService<IConsumer>();

        await consumer.Subscribe<EmailNotificationEvent>("notification.email.confirm", "email-confirmation-queue");

        break;
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);

        retry++;

        await Task.Delay(5000);
    }
}

app.Run();