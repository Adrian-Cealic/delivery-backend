using DeliverySystem.Infrastructure.Notifications;
using DeliverySystem.Infrastructure.Notifications.Factories;
using DeliverySystem.Infrastructure.Repositories;
using DeliverySystem.Interfaces;
using DeliverySystem.Interfaces.Notifications;
using DeliverySystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Delivery Management System API",
        Version = "v1",
        Description = "REST API for the Delivery Management System - Laborator 2"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<ICourierRepository, InMemoryCourierRepository>();
builder.Services.AddSingleton<IDeliveryRepository, InMemoryDeliveryRepository>();

builder.Services.AddSingleton<INotificationFactory, ConsoleNotificationFactory>();
builder.Services.AddSingleton<INotificationService, ConsoleNotificationService>();

builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<DeliveryService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Delivery System API v1");
});

app.UseCors("AllowReactApp");

app.UseMiddleware<DeliverySystem.API.Middleware.ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
