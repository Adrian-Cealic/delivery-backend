using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.ValueObjects;
using DeliverySystem.Infrastructure.Notifications.Decorators;
using DeliverySystem.Tests.Lab5.TestDoubles;

namespace DeliverySystem.Tests.Lab5;

public class DecoratorTests
{
    private static Customer CreateTestCustomer()
        => new("Test User", "test@example.com", "+37360000000",
            new Address("Main St", "Chisinau", "MD-2000", "Moldova"));

    private static Order CreateTestOrder()
        => new(Guid.NewGuid());

    [Fact]
    public void LoggingDecorator_LogsBeforeDelegating()
    {
        var logs = new List<string>();
        var inner = new RecordingNotificationService();
        var decorator = new LoggingNotificationDecorator(inner, msg => logs.Add(msg));

        decorator.NotifyOrderCreated(CreateTestOrder(), CreateTestCustomer());

        Assert.Single(logs);
        Assert.Contains("NotifyOrderCreated", logs[0]);
        Assert.Single(inner.RecordedCalls);
    }

    [Fact]
    public void SmsDecorator_SendsSmsBeforeDelegating()
    {
        var smsSent = new List<(string Phone, string Message)>();
        var inner = new RecordingNotificationService();
        var decorator = new SmsNotificationDecorator(inner,
            (phone, msg) => smsSent.Add((phone, msg)));

        decorator.NotifyOrderCreated(CreateTestOrder(), CreateTestCustomer());

        Assert.Single(smsSent);
        Assert.Equal("+37360000000", smsSent[0].Phone);
        Assert.Single(inner.RecordedCalls);
    }

    [Fact]
    public void ChainedDecorators_AllFireInOrder()
    {
        var callOrder = new List<string>();
        var inner = new RecordingNotificationService();

        var sms = new SmsNotificationDecorator(inner,
            (_, _) => callOrder.Add("SMS"));
        var logging = new LoggingNotificationDecorator(sms,
            _ => callOrder.Add("LOG"));

        logging.NotifyOrderCreated(CreateTestOrder(), CreateTestCustomer());

        Assert.Equal(3, callOrder.Count + inner.RecordedCalls.Count);
        Assert.Equal("LOG", callOrder[0]);
        Assert.Equal("SMS", callOrder[1]);
        Assert.Single(inner.RecordedCalls);
    }

    [Fact]
    public void Decorator_NullInner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LoggingNotificationDecorator(null!));

        Assert.Throws<ArgumentNullException>(() =>
            new SmsNotificationDecorator(null!));
    }

    [Fact]
    public void LoggingDecorator_AllMethodsDelegate()
    {
        var inner = new RecordingNotificationService();
        var decorator = new LoggingNotificationDecorator(inner, _ => { });
        var customer = CreateTestCustomer();
        var order = CreateTestOrder();
        var courier = new BikeCourier("John", "+37360000001");
        var delivery = new Delivery(order.Id, courier.Id, 5.0m);

        decorator.NotifyOrderCreated(order, customer);
        decorator.NotifyOrderStatusChanged(order, customer);
        decorator.NotifyDeliveryAssigned(delivery, customer, courier);
        decorator.NotifyDeliveryStatusChanged(delivery, customer);
        decorator.NotifyDeliveryCompleted(delivery, customer);

        Assert.Equal(5, inner.RecordedCalls.Count);
    }

    [Fact]
    public void SmsDecorator_AllMethodsDelegate()
    {
        var smsSent = new List<string>();
        var inner = new RecordingNotificationService();
        var decorator = new SmsNotificationDecorator(inner,
            (_, msg) => smsSent.Add(msg));
        var customer = CreateTestCustomer();
        var order = CreateTestOrder();
        var courier = new BikeCourier("John", "+37360000001");
        var delivery = new Delivery(order.Id, courier.Id, 5.0m);

        decorator.NotifyOrderCreated(order, customer);
        decorator.NotifyOrderStatusChanged(order, customer);
        decorator.NotifyDeliveryAssigned(delivery, customer, courier);
        decorator.NotifyDeliveryStatusChanged(delivery, customer);
        decorator.NotifyDeliveryCompleted(delivery, customer);

        Assert.Equal(5, smsSent.Count);
        Assert.Equal(5, inner.RecordedCalls.Count);
    }
}
