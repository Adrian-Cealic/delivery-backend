using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Interfaces.Observer;

namespace DeliverySystem.Infrastructure.Observer;

public sealed class DashboardDeliveryObserver : IDeliveryObserver
{
    private readonly List<DashboardEvent> _events = new();
    private readonly object _gate = new();

    public string ChannelName => "Dashboard";

    public IReadOnlyList<DashboardEvent> Events
    {
        get
        {
            lock (_gate)
                return _events.ToArray();
        }
    }

    public void OnDeliveryStatusChanged(Delivery delivery, DeliveryStatus previousStatus)
    {
        var entry = new DashboardEvent(
            DateTime.UtcNow,
            delivery.Id,
            previousStatus,
            delivery.Status);

        lock (_gate)
            _events.Add(entry);
    }
}

public sealed record DashboardEvent(
    DateTime At,
    Guid DeliveryId,
    DeliveryStatus From,
    DeliveryStatus To);
