using DeliverySystem.Interfaces.Notifications;

namespace DeliverySystem.Infrastructure.Notifications.Senders;

public class ConsoleNotificationSender : INotificationSender
{
    public void Send(string recipient, string subject, string body)
    {
        Console.WriteLine();
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║  CONSOLE NOTIFICATION");
        Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
        Console.WriteLine($"║  To: {recipient}");
        Console.WriteLine($"║  Subject: {subject}");
        Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
        Console.WriteLine(body);
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
    }
}
