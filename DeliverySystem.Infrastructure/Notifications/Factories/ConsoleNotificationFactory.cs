using DeliverySystem.Infrastructure.Notifications.Formatters;
using DeliverySystem.Infrastructure.Notifications.Senders;
using DeliverySystem.Interfaces.Notifications;

namespace DeliverySystem.Infrastructure.Notifications.Factories;

public class ConsoleNotificationFactory : INotificationFactory
{
    public INotificationSender CreateSender()
    {
        return new ConsoleNotificationSender();
    }

    public INotificationFormatter CreateFormatter()
    {
        return new PlainTextNotificationFormatter();
    }
}
