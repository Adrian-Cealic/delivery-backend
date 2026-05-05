using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Interfaces.Observer;

public interface IDeliverySubject
{
    void Attach(IDeliveryObserver observer);
    void Detach(IDeliveryObserver observer);
    void Notify(Delivery delivery, DeliveryStatus previousStatus);
}
