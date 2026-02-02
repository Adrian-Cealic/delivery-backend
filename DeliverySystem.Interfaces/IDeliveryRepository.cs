using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Interfaces;

public interface IDeliveryRepository
{
    Delivery? GetById(Guid id);
    Delivery? GetByOrderId(Guid orderId);
    IEnumerable<Delivery> GetAll();
    IEnumerable<Delivery> GetByCourierId(Guid courierId);
    void Add(Delivery delivery);
    void Update(Delivery delivery);
    void Delete(Guid id);
}
