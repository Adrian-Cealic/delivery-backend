using DeliverySystem.Domain.ValueObjects;

namespace DeliverySystem.Domain.Entities;

public class Customer : EntityBase
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public Address Address { get; private set; }

    public Customer(string name, string email, string phone, Address address)
    {
        SetName(name);
        SetEmail(email);
        SetPhone(phone);
        SetAddress(address);
    }

    public Customer(Guid id, string name, string email, string phone, Address address) : base(id)
    {
        SetName(name);
        SetEmail(email);
        SetPhone(phone);
        SetAddress(address);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Customer name cannot be empty.", nameof(name));

        Name = name;
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Customer email cannot be empty.", nameof(email));

        if (!email.Contains('@'))
            throw new ArgumentException("Invalid email format.", nameof(email));

        Email = email;
    }

    public void SetPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Customer phone cannot be empty.", nameof(phone));

        Phone = phone;
    }

    public void SetAddress(Address address)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }

    public override string ToString()
    {
        return $"Customer: {Name} ({Email})";
    }
}
