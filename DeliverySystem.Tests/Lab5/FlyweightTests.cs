using DeliverySystem.Domain.Flyweight;

namespace DeliverySystem.Tests.Lab5;

public class FlyweightTests
{
    [Fact]
    public void GetZone_SameCode_ReturnsSameInstance()
    {
        var factory = new DeliveryZoneFactory();

        var zone1 = factory.GetZone("CTR", "Center", 5.00m, 30m);
        var zone2 = factory.GetZone("CTR", "Center", 5.00m, 30m);

        Assert.Same(zone1, zone2);
    }

    [Fact]
    public void GetZone_DifferentCodes_ReturnsDifferentInstances()
    {
        var factory = new DeliveryZoneFactory();

        var center = factory.GetZone("CTR", "Center", 5.00m, 30m);
        var suburb = factory.GetZone("SUB", "Suburb", 8.50m, 25m);

        Assert.NotSame(center, suburb);
        Assert.Equal("CTR", center.ZoneCode);
        Assert.Equal("SUB", suburb.ZoneCode);
    }

    [Fact]
    public void CachedCount_ReflectsUniqueZones()
    {
        var factory = new DeliveryZoneFactory();

        factory.GetZone("CTR", "Center", 5.00m, 30m);
        factory.GetZone("SUB", "Suburb", 8.50m, 25m);
        factory.GetZone("CTR", "Center", 5.00m, 30m);
        factory.GetZone("RUR", "Rural", 12.00m, 20m);

        Assert.Equal(3, factory.CachedCount);
    }

    [Fact]
    public void GetZone_PreservesIntrinsicState()
    {
        var factory = new DeliveryZoneFactory();

        var zone = factory.GetZone("CTR", "Center", 5.00m, 30m);

        Assert.Equal("CTR", zone.ZoneCode);
        Assert.Equal("Center", zone.ZoneName);
        Assert.Equal(5.00m, zone.BaseDeliveryFee);
        Assert.Equal(30m, zone.MaxWeightKg);
    }

    [Fact]
    public void GetZone_CaseInsensitiveLookup_ReturnsSameInstance()
    {
        var factory = new DeliveryZoneFactory();

        var zone1 = factory.GetZone("CTR", "Center", 5.00m, 30m);
        var zone2 = factory.GetZone("ctr", "Center", 5.00m, 30m);

        Assert.Same(zone1, zone2);
        Assert.Equal(1, factory.CachedCount);
    }
}
