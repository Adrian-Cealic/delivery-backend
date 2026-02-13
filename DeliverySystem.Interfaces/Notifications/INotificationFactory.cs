namespace DeliverySystem.Interfaces.Notifications;

public interface INotificationFactory
{
    INotificationSender CreateSender();
    INotificationFormatter CreateFormatter();
}
