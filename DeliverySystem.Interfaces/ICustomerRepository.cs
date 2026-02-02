using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Interfaces;

public interface ICustomerRepository
{
    Customer? GetById(Guid id);
    Customer? GetByEmail(string email);
    IEnumerable<Customer> GetAll();
    void Add(Customer customer);
    void Update(Customer customer);
    void Delete(Guid id);
}
