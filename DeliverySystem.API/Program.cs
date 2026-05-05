using DeliverySystem.Domain.Flyweight;
using DeliverySystem.Domain.Memento;
using DeliverySystem.Infrastructure.AccessContext;
using DeliverySystem.Infrastructure.Configuration;
using DeliverySystem.Infrastructure.Notifications;
using DeliverySystem.Infrastructure.Notifications.Decorators;
using DeliverySystem.Infrastructure.Notifications.Factories;
using DeliverySystem.Infrastructure.Observer;
using DeliverySystem.Infrastructure.Payments;
using DeliverySystem.Infrastructure.Payments.Adapters;
using DeliverySystem.Infrastructure.Payments.ExternalApis;
using DeliverySystem.Infrastructure.Repositories;
using DeliverySystem.Infrastructure.Repositories.Proxies;
using DeliverySystem.Interfaces;
using DeliverySystem.Interfaces.Notifications;
using DeliverySystem.Interfaces.Payments;
using DeliverySystem.Services;
using DeliverySystem.Services.Commands;
using DeliverySystem.Services.Memento;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Delivery Management System API",
        Version = "v1",
        Description = "REST API for the Delivery Management System"
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
builder.Services.AddSingleton<INotificationService>(sp =>
{
    var factory = sp.GetRequiredService<INotificationFactory>();
    INotificationService inner = new ConsoleNotificationService(factory);
    inner = new SmsNotificationDecorator(inner);
    return new LoggingNotificationDecorator(inner);
});

builder.Services.AddSingleton(DeliverySystemConfiguration.Instance);

builder.Services.AddSingleton<PayPalApi>();
builder.Services.AddSingleton<StripeApi>();
builder.Services.AddSingleton<GooglePayApi>();
builder.Services.AddSingleton<PayPalPaymentAdapter>(sp =>
{
    var api = sp.GetRequiredService<PayPalApi>();
    return new PayPalPaymentAdapter(api);
});
builder.Services.AddSingleton<StripePaymentAdapter>(sp =>
{
    var api = sp.GetRequiredService<StripeApi>();
    return new StripePaymentAdapter(api);
});
builder.Services.AddSingleton<GooglePayPaymentAdapter>(sp =>
{
    var api = sp.GetRequiredService<GooglePayApi>();
    return new GooglePayPaymentAdapter(api);
});
builder.Services.AddSingleton<IPaymentGatewayProvider, PaymentGatewayProvider>();

builder.Services.AddSingleton<CatalogProvider>();
builder.Services.AddSingleton<DeliveryZoneFactory>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAccessContext, HttpHeaderAccessContext>();
builder.Services.AddScoped<ProtectionOrderRepositoryProxy>(sp =>
{
    var realRepo = sp.GetRequiredService<IOrderRepository>();
    var accessContext = sp.GetRequiredService<IAccessContext>();
    return new ProtectionOrderRepositoryProxy(realRepo, accessContext);
});

builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<DeliveryService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<OrderPlacementFacade>();

// Lab 6 — Behavioral patterns
builder.Services.AddSingleton<DeliveryStatusSubject>();
builder.Services.AddSingleton<DashboardDeliveryObserver>();
builder.Services.AddSingleton<DeliveryCommandInvoker>();
builder.Services.AddSingleton<OrderDraft>();
builder.Services.AddSingleton<OrderDraftCaretaker>();

var app = builder.Build();

// Attach the dashboard observer at startup so the events feed begins recording.
var subject = app.Services.GetRequiredService<DeliveryStatusSubject>();
var dashboard = app.Services.GetRequiredService<DashboardDeliveryObserver>();
subject.Attach(dashboard);

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Delivery System API v1");
});

app.UseCors("AllowReactApp");

app.UseMiddleware<DeliverySystem.API.Middleware.ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
