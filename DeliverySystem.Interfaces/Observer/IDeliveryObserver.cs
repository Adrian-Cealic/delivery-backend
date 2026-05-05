using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Interfaces.Observer;

public interface IDeliveryObserver
{
    string ChannelName { get; }
    void OnDeliveryStatusChanged(Delivery delivery, DeliveryStatus previousStatus);
}
