using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Entities;

public abstract class Courier : EntityBase
{
    public string Name { get; private set; }
    public string Phone { get; private set; }
    public bool IsAvailable { get; private set; }

    public abstract VehicleType VehicleType { get; }
    public abstract decimal MaxWeight { get; }
    public abstract TimeSpan CalculateDeliveryTime(decimal distanceKm);

    protected Courier(string name, string phone)
    {
        SetName(name);
        SetPhone(phone);
        IsAvailable = true;
    }

    protected Courier(Guid id, string name, string phone) : base(id)
    {
        SetName(name);
        SetPhone(phone);
        IsAvailable = true;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Courier name cannot be empty.", nameof(name));

        Name = name;
    }

    public void SetPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Courier phone cannot be empty.", nameof(phone));

        Phone = phone;
    }

    public void SetAvailable()
    {
        IsAvailable = true;
    }

    public void SetUnavailable()
    {
        IsAvailable = false;
    }

    public bool CanCarry(decimal weight)
    {
        return weight <= MaxWeight;
    }

    public override string ToString()
    {
        return $"Courier: {Name} ({VehicleType}), Available: {IsAvailable}";
    }
}
