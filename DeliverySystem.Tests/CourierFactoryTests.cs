using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Domain.Factories;

namespace DeliverySystem.Tests;

public class CourierFactoryTests
{
    [Fact]
    public void BikeCourierFactory_CreatesCourier_ReturnsBikeCourier()
    {
        var factory = new BikeCourierFactory();
        var parameters = new CourierCreationParams("Test Biker", "+373-69-000000");

        var courier = factory.CreateCourier(parameters);

        Assert.IsType<BikeCourier>(courier);
        Assert.Equal("Test Biker", courier.Name);
        Assert.Equal(VehicleType.Bicycle, courier.VehicleType);
    }

    [Fact]
    public void CarCourierFactory_CreatesCourier_ReturnsCarCourier()
    {
        var factory = new CarCourierFactory();
        var parameters = new CourierCreationParams("Test Driver", "+373-69-000001", licensePlate: "XY-123");

        var courier = factory.CreateCourier(parameters);

        var carCourier = Assert.IsType<CarCourier>(courier);
        Assert.Equal("Test Driver", carCourier.Name);
        Assert.Equal("XY-123", carCourier.LicensePlate);
        Assert.Equal(VehicleType.Car, carCourier.VehicleType);
    }

    [Fact]
    public void CarCourierFactory_WithoutLicensePlate_ThrowsArgumentException()
    {
        var factory = new CarCourierFactory();
        var parameters = new CourierCreationParams("Test Driver", "+373-69-000001");

        Assert.Throws<ArgumentException>(() => factory.CreateCourier(parameters));
    }

    [Fact]
    public void DroneCourierFactory_CreatesCourier_ReturnsDroneCourier()
    {
        var factory = new DroneCourierFactory();
        var parameters = new CourierCreationParams("Drone-X1", "+373-69-000002", maxFlightRangeKm: 20.0m);

        var courier = factory.CreateCourier(parameters);

        var droneCourier = Assert.IsType<DroneCourier>(courier);
        Assert.Equal("Drone-X1", droneCourier.Name);
        Assert.Equal(20.0m, droneCourier.MaxFlightRangeKm);
        Assert.Equal(VehicleType.Drone, droneCourier.VehicleType);
    }

    [Fact]
    public void DroneCourierFactory_WithoutFlightRange_ThrowsArgumentException()
    {
        var factory = new DroneCourierFactory();
        var parameters = new CourierCreationParams("Drone-X1", "+373-69-000002");

        Assert.Throws<ArgumentException>(() => factory.CreateCourier(parameters));
    }

    [Fact]
    public void CourierFactoryProvider_ReturnsCorrectFactory_ForEachVehicleType()
    {
        var bikeFactory = CourierFactoryProvider.GetFactory(VehicleType.Bicycle);
        var carFactory = CourierFactoryProvider.GetFactory(VehicleType.Car);
        var droneFactory = CourierFactoryProvider.GetFactory(VehicleType.Drone);

        Assert.IsType<BikeCourierFactory>(bikeFactory);
        Assert.IsType<CarCourierFactory>(carFactory);
        Assert.IsType<DroneCourierFactory>(droneFactory);
    }

    [Fact]
    public void CreateAndValidate_WithNullParams_ThrowsArgumentNullException()
    {
        var factory = new BikeCourierFactory();

        Assert.Throws<ArgumentNullException>(() => factory.CreateAndValidate(null!));
    }

    [Fact]
    public void CreateAndValidate_WithEmptyName_ThrowsArgumentException()
    {
        var factory = new BikeCourierFactory();
        var parameters = new CourierCreationParams("", "+373-69-000000");

        Assert.Throws<ArgumentException>(() => factory.CreateAndValidate(parameters));
    }

    [Fact]
    public void CreateAndValidate_WithValidParams_ReturnsCourier()
    {
        var factory = new BikeCourierFactory();
        var parameters = new CourierCreationParams("Valid Name", "+373-69-000000");

        var courier = factory.CreateAndValidate(parameters);

        Assert.NotNull(courier);
        Assert.Equal("Valid Name", courier.Name);
    }

    [Fact]
    public void DroneCourier_CalculateDeliveryTime_ExceedingRange_ThrowsException()
    {
        var factory = new DroneCourierFactory();
        var parameters = new CourierCreationParams("Drone-Test", "+373-69-999999", maxFlightRangeKm: 5.0m);
        var drone = factory.CreateCourier(parameters);

        Assert.Throws<InvalidOperationException>(() => drone.CalculateDeliveryTime(10.0m));
    }

    [Fact]
    public void AllFactories_ProduceCouriers_ThatAreAvailableByDefault()
    {
        var types = CourierFactoryProvider.GetSupportedTypes();

        foreach (var type in types)
        {
            var factory = CourierFactoryProvider.GetFactory(type);
            var parameters = type switch
            {
                VehicleType.Car => new CourierCreationParams("Test", "+373", licensePlate: "ABC"),
                VehicleType.Drone => new CourierCreationParams("Test", "+373", maxFlightRangeKm: 10m),
                _ => new CourierCreationParams("Test", "+373")
            };

            var courier = factory.CreateCourier(parameters);
            Assert.True(courier.IsAvailable);
        }
    }
}
