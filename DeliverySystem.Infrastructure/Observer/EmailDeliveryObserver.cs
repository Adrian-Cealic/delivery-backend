using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Interfaces.Observer;

namespace DeliverySystem.Infrastructure.Observer;

public sealed class EmailDeliveryObserver : IDeliveryObserver
{
    private readonly Action<string> _emit;
    private readonly string _customerEmail;

    public EmailDeliveryObserver(string customerEmail, Action<string>? emit = null)
    {
        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Customer email cannot be empty.", nameof(customerEmail));

        _customerEmail = customerEmail;
        _emit = emit ?? Console.WriteLine;
    }

    public string ChannelName => $"Email:{_customerEmail}";

    public void OnDeliveryStatusChanged(Delivery delivery, DeliveryStatus previousStatus)
    {
        _emit($"[EMAIL → {_customerEmail}] Delivery {delivery.Id} moved {previousStatus} → {delivery.Status}.");
    }
}
