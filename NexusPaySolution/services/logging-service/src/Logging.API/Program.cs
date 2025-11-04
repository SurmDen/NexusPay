using Logging.API.Extentions;
using Logging.Application.Interfaces;
using Logging.Application.Log.Commands;
using Logging.Domain.Events;
using Logging.Infrastructure.MessageBus.Options;
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
    config.RegisterServicesFromAssembly(typeof(DeleteLogsCommand).Assembly);
    config.RegisterServicesFromAssembly(typeof(LogReceivedEvent).Assembly);
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

var consumer = app.Services.GetRequiredService<IConsumer>();

await Task.Delay(10000);

await consumer.Subscribe("logging.identity", "identity-logs-queue");
await consumer.Subscribe("logging.notification", "notification-logs-queue");

app.Run();