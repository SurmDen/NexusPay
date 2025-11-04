using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Ket"]))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("UserOrAdmin", policy =>
        policy.RequireRole("User", "Admin"));
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("identity", new OpenApiInfo
    {
        Title = "Identity Service",
        Version = "v1",
        Description = "Управление пользователями, аутентификацией и авторизацией",
        Contact = new OpenApiContact { Name = "NexusPay Team", Email = "support@nexuspay.com" }
    });

    c.SwaggerDoc("logging", new OpenApiInfo
    {
        Title = "Logging Service",
        Version = "v1",
        Description = "Централизованное логирование и аудит системы",
        Contact = new OpenApiContact { Name = "NexusPay Team", Email = "support@nexuspay.com" }
    });

    c.SwaggerDoc("notifications", new OpenApiInfo
    {
        Title = "Notification Service",
        Version = "v1",
        Description = "Отправка email уведомлений и управление коммуникациями",
        Contact = new OpenApiContact { Name = "NexusPay Team", Email = "support@nexuspay.com" }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    c.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/identity/swagger/v1/swagger.json", "Identity Service v1");
        c.SwaggerEndpoint("/logging/swagger/v1/swagger.json", "Logging Service v1");
        c.SwaggerEndpoint("/notifications/swagger/v1/swagger.json", "Notification Service v1");

        c.RoutePrefix = "docs";
        c.DocumentTitle = "NexusPay API Documentation";
        c.DisplayOperationId();
        c.DisplayRequestDuration();
    });
}

app.UseCors("AllowAll");;

app.UseAuthentication();

app.UseAuthorization();

await app.UseOcelot();

app.Run();
