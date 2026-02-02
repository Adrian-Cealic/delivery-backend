using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Interfaces;

public interface IOrderRepository
{
    Order? GetById(Guid id);
    IEnumerable<Order> GetAll();
    IEnumerable<Order> GetByCustomerId(Guid customerId);
    void Add(Order order);
    void Update(Order order);
    void Delete(Guid id);
}
