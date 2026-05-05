using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Domain.Iterators;

namespace DeliverySystem.Tests.Lab6;

public class IteratorTests
{
    private static CourierCollection BuildCollection()
    {
        var couriers = new Courier[]
        {
            new BikeCourier("Alex", "+37311111111"),
            new CarCourier("Bob", "+37322222222", "AB-12-CD"),
            new DroneCourier("Charlie", "+37333333333", 25m),
            new BikeCourier("Diana", "+37344444444"),
            new CarCourier("Eve", "+37355555555", "EF-34-GH")
        };
        couriers[1].SetUnavailable();
        return new CourierCollection(couriers);
    }

    [Fact]
    public void InsertionOrderIterator_VisitsAllInOrder()
    {
        var collection = BuildCollection();
        var iter = collection.CreateInsertionOrderIterator();

        var names = new List<string>();
        while (iter.HasNext()) names.Add(iter.Next().Name);

        Assert.Equal(new[] { "Alex", "Bob", "Charlie", "Diana", "Eve" }, names);
    }

    [Fact]
    public void AvailableIterator_SkipsUnavailableCouriers()
    {
        var collection = BuildCollection();
        var iter = collection.CreateAvailableIterator();

        var names = new List<string>();
        while (iter.HasNext()) names.Add(iter.Next().Name);

        Assert.DoesNotContain("Bob", names);
        Assert.Equal(4, names.Count);
    }

    [Fact]
    public void VehicleTypeIterator_OnlyYieldsMatchingType()
    {
        var collection = BuildCollection();
        var iter = collection.CreateVehicleTypeIterator(VehicleType.Bicycle);

        var names = new List<string>();
        while (iter.HasNext()) names.Add(iter.Next().Name);

        Assert.Equal(new[] { "Alex", "Diana" }, names);
    }

    [Fact]
    public void RoundRobinIterator_WrapsAroundForFixedSteps()
    {
        var collection = BuildCollection();
        var iter = collection.CreateRoundRobinIterator(totalSteps: 7);

        var names = new List<string>();
        while (iter.HasNext()) names.Add(iter.Next().Name);

        Assert.Equal(7, names.Count);
        Assert.Equal("Alex", names[0]);
        Assert.Equal("Eve", names[4]);
        // wraps around
        Assert.Equal("Alex", names[5]);
        Assert.Equal("Bob", names[6]);
    }

    [Fact]
    public void Reset_RestartsTraversalFromBeginning()
    {
        var collection = BuildCollection();
        var iter = collection.CreateInsertionOrderIterator();

        iter.Next();
        iter.Next();
        iter.Reset();

        Assert.Equal("Alex", iter.Next().Name);
    }

    [Fact]
    public void Next_AfterExhaustion_Throws()
    {
        var collection = BuildCollection();
        var iter = collection.CreateVehicleTypeIterator(VehicleType.Drone);
        iter.Next();

        Assert.Throws<InvalidOperationException>(() => iter.Next());
    }

    [Fact]
    public void HasNext_OnEmptyCollection_ReturnsFalse()
    {
        var collection = new CourierCollection(Array.Empty<Courier>());
        var iter = collection.CreateInsertionOrderIterator();

        Assert.False(iter.HasNext());
    }

    [Fact]
    public void Iterators_AreIndependentInstances()
    {
        var collection = BuildCollection();
        var iter1 = collection.CreateInsertionOrderIterator();
        var iter2 = collection.CreateInsertionOrderIterator();

        iter1.Next();
        Assert.Equal("Alex", iter2.Next().Name);
    }
}
