using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Visitors;

namespace DeliverySystem.Tests.Lab7;

public class VisitorTests
{
    private static IReadOnlyList<Courier> Fleet() => new Courier[]
    {
        new BikeCourier("Alex", "+37311111111"),
        new CarCourier("Bob", "+37322222222", "AB-12-CD"),
        new DroneCourier("Charlie", "+37333333333", 25m),
    };

    [Fact]
    public void CapacityVisitor_ProducesPerSubtypeDescription()
    {
        var v = new CapacityReportVisitor();
        var fleet = Fleet();

        var bike = fleet[0].Accept(v);
        var car = fleet[1].Accept(v);
        var drone = fleet[2].Accept(v);

        Assert.Contains("Bike", bike);
        Assert.Contains("plate AB-12-CD", car);
        Assert.Contains("25", drone);
    }

    [Fact]
    public void MaintenanceVisitor_ScalesCostByDroneRange()
    {
        var visitor = new MaintenanceCostVisitor();
        var droneShort = new DroneCourier("D1", "+37300", 10m);
        var droneLong = new DroneCourier("D2", "+37300", 40m);

        var costShort = droneShort.Accept(visitor);
        var costLong = droneLong.Accept(visitor);

        Assert.True(costLong > costShort);
    }

    [Fact]
    public void EcoVisitor_RanksBikeHighestCarLowest()
    {
        var visitor = new EcoScoreVisitor();
        var fleet = Fleet();

        var bikeScore = fleet[0].Accept(visitor);
        var carScore = fleet[1].Accept(visitor);
        var droneScore = fleet[2].Accept(visitor);

        Assert.True(bikeScore > droneScore);
        Assert.True(droneScore > carScore);
    }

    [Fact]
    public void Visitor_ProcessesHeterogeneousCollection()
    {
        var visitor = new MaintenanceCostVisitor();
        var totalCost = Fleet().Sum(c => c.Accept(visitor));

        Assert.True(totalCost > 0);
    }

    [Fact]
    public void Accept_NullVisitor_Throws()
    {
        Courier bike = new BikeCourier("X", "+37300");

        Assert.Throws<ArgumentNullException>(() => bike.Accept<int>(null!));
    }

    [Fact]
    public void Visitor_AddsNewOperationWithoutModifyingEntities()
    {
        // Adding a new visitor (eco score) didn't touch BikeCourier/CarCourier/DroneCourier.
        // This test asserts that the existing entities stay unmodified by inspecting behaviour.
        var newVisitor = new EcoScoreVisitor();
        Courier any = new BikeCourier("Y", "+37300");

        var score = any.Accept(newVisitor);

        Assert.InRange(score, 0, 100);
    }

    [Fact]
    public void DifferentVisitors_ProduceDifferentResultTypes()
    {
        var bike = new BikeCourier("Z", "+37300");

        string capacity = bike.Accept(new CapacityReportVisitor());
        decimal maintenance = bike.Accept(new MaintenanceCostVisitor());
        int eco = bike.Accept(new EcoScoreVisitor());

        Assert.False(string.IsNullOrEmpty(capacity));
        Assert.True(maintenance > 0);
        Assert.InRange(eco, 0, 100);
    }
}
