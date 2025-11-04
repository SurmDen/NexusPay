using Notification.API.Extentions;
using Notification.Application.EmailNotification.Command;
using Notification.Application.Interfaces;
using Notification.Domain.Entities;
using Notification.Domain.Events;
using Notification.Infrastructure.MessageBus.Options;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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
}
else
{
    app.UseExceptionHandler("/Error");

    app.UseHsts();

    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseSession();

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
