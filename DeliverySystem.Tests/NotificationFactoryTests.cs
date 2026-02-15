using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.ValueObjects;
using DeliverySystem.Infrastructure.Notifications.Factories;
using DeliverySystem.Infrastructure.Notifications.Formatters;
using DeliverySystem.Infrastructure.Notifications.Senders;
using DeliverySystem.Interfaces.Notifications;

namespace DeliverySystem.Tests;

public class NotificationFactoryTests
{
    [Fact]
    public void ConsoleFactory_CreatesSender_ReturnsConsoleNotificationSender()
    {
        INotificationFactory factory = new ConsoleNotificationFactory();

        var sender = factory.CreateSender();

        Assert.IsType<ConsoleNotificationSender>(sender);
    }

    [Fact]
    public void ConsoleFactory_CreatesFormatter_ReturnsPlainTextFormatter()
    {
        INotificationFactory factory = new ConsoleNotificationFactory();

        var formatter = factory.CreateFormatter();

        Assert.IsType<PlainTextNotificationFormatter>(formatter);
    }

    [Fact]
    public void EmailFactory_CreatesSender_ReturnsEmailNotificationSender()
    {
        INotificationFactory factory = new EmailNotificationFactory();

        var sender = factory.CreateSender();

        Assert.IsType<EmailNotificationSender>(sender);
    }

    [Fact]
    public void EmailFactory_CreatesFormatter_ReturnsHtmlFormatter()
    {
        INotificationFactory factory = new EmailNotificationFactory();

        var formatter = factory.CreateFormatter();

        Assert.IsType<HtmlNotificationFormatter>(formatter);
    }

    [Fact]
    public void ConsoleFactory_FormatterOutput_DoesNotContainHtml()
    {
        INotificationFactory factory = new ConsoleNotificationFactory();
        var formatter = factory.CreateFormatter();

        var (order, customer) = CreateTestOrderAndCustomer();
        var result = formatter.FormatOrderCreated(order, customer);

        Assert.DoesNotContain("<div", result);
        Assert.DoesNotContain("<h2>", result);
    }

    [Fact]
    public void EmailFactory_FormatterOutput_ContainsHtml()
    {
        INotificationFactory factory = new EmailNotificationFactory();
        var formatter = factory.CreateFormatter();

        var (order, customer) = CreateTestOrderAndCustomer();
        var result = formatter.FormatOrderCreated(order, customer);

        Assert.Contains("<div", result);
        Assert.Contains("<h2>", result);
    }

    [Fact]
    public void BothFactories_ProduceWorkingPairs()
    {
        var factories = new INotificationFactory[]
        {
            new ConsoleNotificationFactory(),
            new EmailNotificationFactory()
        };

        foreach (var factory in factories)
        {
            var sender = factory.CreateSender();
            var formatter = factory.CreateFormatter();

            Assert.NotNull(sender);
            Assert.NotNull(formatter);

            var (order, customer) = CreateTestOrderAndCustomer();
            var body = formatter.FormatOrderCreated(order, customer);

            Assert.False(string.IsNullOrEmpty(body));
        }
    }

    [Fact]
    public void PlainTextFormatter_FormatDeliveryAssigned_ContainsCourierInfo()
    {
        var formatter = new PlainTextNotificationFormatter();
        var customer = CreateTestCustomer();
        var courier = new BikeCourier("Test Courier", "+373-69-000000");
        var delivery = new Delivery(Guid.NewGuid(), courier.Id, 5.0m);

        var result = formatter.FormatDeliveryAssigned(delivery, customer, courier);

        Assert.Contains("Test Courier", result);
        Assert.Contains("Bicycle", result);
    }

    [Fact]
    public void HtmlFormatter_FormatDeliveryCompleted_ContainsThankYou()
    {
        var formatter = new HtmlNotificationFormatter();
        var customer = CreateTestCustomer();
        var courier = new BikeCourier("Test Courier", "+373-69-000000");
        var delivery = new Delivery(Guid.NewGuid(), courier.Id, 5.0m);
        delivery.SetEstimatedDeliveryTime(TimeSpan.FromMinutes(15));
        delivery.MarkAsAssigned();
        delivery.MarkAsPickedUp();
        delivery.MarkAsInTransit();
        delivery.MarkAsDelivered();

        var result = formatter.FormatDeliveryCompleted(delivery, customer);

        Assert.Contains("Thank you", result);
        Assert.Contains("<em>", result);
    }

    private static Customer CreateTestCustomer()
    {
        var address = new Address("Test Street", "Test City", "12345", "Moldova");
        return new Customer("Test User", "test@example.com", "+373-69-000000", address);
    }

    private static (Order, Customer) CreateTestOrderAndCustomer()
    {
        var customer = CreateTestCustomer();
        var order = new Order(customer.Id);
        order.AddItem(new OrderItem("Test Product", 1, 10.0m, 0.5m));
        return (order, customer);
    }
}
