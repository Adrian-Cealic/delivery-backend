using DeliverySystem.Interfaces.Notifications;

namespace DeliverySystem.Infrastructure.Notifications.Senders;

public class EmailNotificationSender : INotificationSender
{
    public void Send(string recipient, string subject, string body)
    {
        Console.WriteLine();
        Console.WriteLine($"--- EMAIL SENT ---");
        Console.WriteLine($"From: noreply@deliverysystem.md");
        Console.WriteLine($"To: {recipient}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Content-Type: text/html");
        Console.WriteLine($"---");
        Console.WriteLine(body);
        Console.WriteLine($"--- END EMAIL ---");
    }
}
