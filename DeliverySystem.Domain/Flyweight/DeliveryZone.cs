namespace DeliverySystem.Domain.Flyweight;

public sealed class DeliveryZone
{
    public string ZoneCode { get; }
    public string ZoneName { get; }
    public decimal BaseDeliveryFee { get; }
    public decimal MaxWeightKg { get; }

    internal DeliveryZone(string zoneCode, string zoneName, decimal baseDeliveryFee, decimal maxWeightKg)
    {
        if (string.IsNullOrWhiteSpace(zoneCode))
            throw new ArgumentException("Zone code cannot be empty.", nameof(zoneCode));

        if (string.IsNullOrWhiteSpace(zoneName))
            throw new ArgumentException("Zone name cannot be empty.", nameof(zoneName));

        if (baseDeliveryFee < 0)
            throw new ArgumentException("Base delivery fee cannot be negative.", nameof(baseDeliveryFee));

        if (maxWeightKg <= 0)
            throw new ArgumentException("Max weight must be positive.", nameof(maxWeightKg));

        ZoneCode = zoneCode;
        ZoneName = zoneName;
        BaseDeliveryFee = baseDeliveryFee;
        MaxWeightKg = maxWeightKg;
    }

    public override string ToString() => $"[{ZoneCode}] {ZoneName} — Fee: {BaseDeliveryFee:C}, MaxWeight: {MaxWeightKg}kg";
}
