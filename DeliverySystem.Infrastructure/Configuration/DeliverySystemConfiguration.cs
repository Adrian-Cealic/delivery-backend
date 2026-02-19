namespace DeliverySystem.Infrastructure.Configuration;

public sealed class DeliverySystemConfiguration
{
    private static readonly Lazy<DeliverySystemConfiguration> _instance =
        new(() => new DeliverySystemConfiguration());

    private readonly object _lock = new();

    public static DeliverySystemConfiguration Instance => _instance.Value;

    public decimal MaxDeliveryDistanceKm { get; private set; }
    public string DefaultCurrency { get; private set; }
    public int MaxOrderItems { get; private set; }
    public string SystemName { get; private set; }

    private DeliverySystemConfiguration()
    {
        MaxDeliveryDistanceKm = 100.0m;
        DefaultCurrency = "MDL";
        MaxOrderItems = 50;
        SystemName = "Delivery Management System";
    }

    public void SetMaxDeliveryDistance(decimal distance)
    {
        if (distance <= 0)
            throw new ArgumentException("Max delivery distance must be positive.", nameof(distance));

        lock (_lock)
        {
            MaxDeliveryDistanceKm = distance;
        }
    }

    public void SetDefaultCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty.", nameof(currency));

        lock (_lock)
        {
            DefaultCurrency = currency;
        }
    }

    public void SetMaxOrderItems(int maxItems)
    {
        if (maxItems <= 0)
            throw new ArgumentException("Max order items must be positive.", nameof(maxItems));

        lock (_lock)
        {
            MaxOrderItems = maxItems;
        }
    }

    public void SetSystemName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("System name cannot be empty.", nameof(name));

        lock (_lock)
        {
            SystemName = name;
        }
    }

    public override string ToString()
    {
        return $"[{SystemName}] MaxDistance: {MaxDeliveryDistanceKm}km, Currency: {DefaultCurrency}, MaxItems: {MaxOrderItems}";
    }
}
