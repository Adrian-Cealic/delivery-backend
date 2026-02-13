namespace DeliverySystem.Interfaces.Notifications;

public interface INotificationSender
{
    void Send(string recipient, string subject, string body);
}
