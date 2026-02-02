using DeliverySystem.Domain.Entities;
using DeliverySystem.Interfaces;

namespace DeliverySystem.Infrastructure.Repositories;

public class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly Dictionary<Guid, Customer> _customers = new();

    public Customer? GetById(Guid id)
    {
        _customers.TryGetValue(id, out var customer);
        return customer;
    }

    public Customer? GetByEmail(string email)
    {
        return _customers.Values.FirstOrDefault(c => 
            c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Customer> GetAll()
    {
        return _customers.Values.ToList();
    }

    public void Add(Customer customer)
    {
        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        if (_customers.ContainsKey(customer.Id))
            throw new InvalidOperationException($"Customer with ID {customer.Id} already exists.");

        _customers[customer.Id] = customer;
    }

    public void Update(Customer customer)
    {
        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        if (!_customers.ContainsKey(customer.Id))
            throw new InvalidOperationException($"Customer with ID {customer.Id} not found.");

        _customers[customer.Id] = customer;
    }

    public void Delete(Guid id)
    {
        if (!_customers.ContainsKey(id))
            throw new InvalidOperationException($"Customer with ID {id} not found.");

        _customers.Remove(id);
    }
}
