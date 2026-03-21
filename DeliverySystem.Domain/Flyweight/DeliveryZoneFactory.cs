namespace DeliverySystem.Domain.Flyweight;

public sealed class DeliveryZoneFactory
{
    private readonly Dictionary<string, DeliveryZone> _cache = new(StringComparer.OrdinalIgnoreCase);

    public DeliveryZone GetZone(string zoneCode, string zoneName, decimal baseDeliveryFee, decimal maxWeightKg)
    {
        if (string.IsNullOrWhiteSpace(zoneCode))
            throw new ArgumentException("Zone code cannot be empty.", nameof(zoneCode));

        if (_cache.TryGetValue(zoneCode, out var existing))
            return existing;

        var zone = new DeliveryZone(zoneCode, zoneName, baseDeliveryFee, maxWeightKg);
        _cache[zoneCode] = zone;
        return zone;
    }

    public int CachedCount => _cache.Count;

    public IReadOnlyCollection<DeliveryZone> GetAllCachedZones() => _cache.Values.ToList().AsReadOnly();
}
