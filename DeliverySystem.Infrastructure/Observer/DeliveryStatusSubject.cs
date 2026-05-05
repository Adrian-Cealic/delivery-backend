using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Interfaces.Observer;

namespace DeliverySystem.Infrastructure.Observer;

public sealed class DeliveryStatusSubject : IDeliverySubject
{
    private readonly List<IDeliveryObserver> _observers = new();
    private readonly object _gate = new();

    public IReadOnlyCollection<string> AttachedChannels
    {
        get
        {
            lock (_gate)
                return _observers.Select(o => o.ChannelName).ToArray();
        }
    }

    public void Attach(IDeliveryObserver observer)
    {
        if (observer == null) throw new ArgumentNullException(nameof(observer));

        lock (_gate)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }
    }

    public void Detach(IDeliveryObserver observer)
    {
        if (observer == null) throw new ArgumentNullException(nameof(observer));

        lock (_gate)
            _observers.Remove(observer);
    }

    public void Notify(Delivery delivery, DeliveryStatus previousStatus)
    {
        if (delivery == null) throw new ArgumentNullException(nameof(delivery));

        IDeliveryObserver[] snapshot;
        lock (_gate)
            snapshot = _observers.ToArray();

        foreach (var observer in snapshot)
            observer.OnDeliveryStatusChanged(delivery, previousStatus);
    }
}
