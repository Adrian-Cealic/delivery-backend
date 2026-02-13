using DeliverySystem.Infrastructure.Notifications.Formatters;
using DeliverySystem.Infrastructure.Notifications.Senders;
using DeliverySystem.Interfaces.Notifications;

namespace DeliverySystem.Infrastructure.Notifications.Factories;

public class EmailNotificationFactory : INotificationFactory
{
    public INotificationSender CreateSender()
    {
        return new EmailNotificationSender();
    }

    public INotificationFormatter CreateFormatter()
    {
        return new HtmlNotificationFormatter();
    }
}
