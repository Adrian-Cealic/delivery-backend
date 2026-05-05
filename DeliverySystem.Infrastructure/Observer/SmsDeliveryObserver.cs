using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Interfaces.Observer;

namespace DeliverySystem.Infrastructure.Observer;

public sealed class SmsDeliveryObserver : IDeliveryObserver
{
    private readonly Action<string, string> _send;
    private readonly string _phone;

    public SmsDeliveryObserver(string phone, Action<string, string>? send = null)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone cannot be empty.", nameof(phone));

        _phone = phone;
        _send = send ?? ((to, body) => Console.WriteLine($"SMS → {to}: {body}"));
    }

    public string ChannelName => $"SMS:{_phone}";

    public void OnDeliveryStatusChanged(Delivery delivery, DeliveryStatus previousStatus)
    {
        if (delivery.Status == DeliveryStatus.Delivered ||
            delivery.Status == DeliveryStatus.Failed ||
            delivery.Status == DeliveryStatus.InTransit)
        {
            _send(_phone, $"Update: {delivery.Status} (was {previousStatus}).");
        }
    }
}
