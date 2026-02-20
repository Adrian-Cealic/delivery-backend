using DeliverySystem.Infrastructure.Configuration;

namespace DeliverySystem.Tests.Lab3;

public class SingletonTests
{
    [Fact]
    public void Instance_ReturnsSameInstance()
    {
        var instance1 = DeliverySystemConfiguration.Instance;
        var instance2 = DeliverySystemConfiguration.Instance;

        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void Instance_IsNotNull()
    {
        var instance = DeliverySystemConfiguration.Instance;

        Assert.NotNull(instance);
    }

    [Fact]
    public void Instance_HasDefaultValues()
    {
        var config = DeliverySystemConfiguration.Instance;

        Assert.Equal(100.0m, config.MaxDeliveryDistanceKm);
        Assert.Equal("MDL", config.DefaultCurrency);
        Assert.Equal(50, config.MaxOrderItems);
        Assert.Equal("Delivery Management System", config.SystemName);
    }

    [Fact]
    public void SetMaxDeliveryDistance_UpdatesValue()
    {
        var config = DeliverySystemConfiguration.Instance;
        var originalValue = config.MaxDeliveryDistanceKm;

        config.SetMaxDeliveryDistance(200.0m);

        Assert.Equal(200.0m, config.MaxDeliveryDistanceKm);

        config.SetMaxDeliveryDistance(originalValue);
    }

    [Fact]
    public void SetMaxDeliveryDistance_NegativeValue_ThrowsException()
    {
        var config = DeliverySystemConfiguration.Instance;

        Assert.Throws<ArgumentException>(() => config.SetMaxDeliveryDistance(-10.0m));
    }

    [Fact]
    public void SetDefaultCurrency_UpdatesValue()
    {
        var config = DeliverySystemConfiguration.Instance;
        var originalValue = config.DefaultCurrency;

        config.SetDefaultCurrency("EUR");

        Assert.Equal("EUR", config.DefaultCurrency);

        config.SetDefaultCurrency(originalValue);
    }

    [Fact]
    public void SetDefaultCurrency_EmptyValue_ThrowsException()
    {
        var config = DeliverySystemConfiguration.Instance;

        Assert.Throws<ArgumentException>(() => config.SetDefaultCurrency(""));
    }

    [Fact]
    public void SetMaxOrderItems_UpdatesValue()
    {
        var config = DeliverySystemConfiguration.Instance;
        var originalValue = config.MaxOrderItems;

        config.SetMaxOrderItems(100);

        Assert.Equal(100, config.MaxOrderItems);

        config.SetMaxOrderItems(originalValue);
    }

    [Fact]
    public void ThreadSafety_ConcurrentAccess_ReturnsSameInstance()
    {
        var instances = new DeliverySystemConfiguration[10];
        var tasks = new Task[10];

        for (int i = 0; i < 10; i++)
        {
            int index = i;
            tasks[i] = Task.Run(() =>
            {
                instances[index] = DeliverySystemConfiguration.Instance;
            });
        }

        Task.WaitAll(tasks);

        for (int i = 1; i < 10; i++)
        {
            Assert.Same(instances[0], instances[i]);
        }
    }

    [Fact]
    public void SettingsPersistAcrossReferences()
    {
        var config1 = DeliverySystemConfiguration.Instance;
        var originalName = config1.SystemName;

        config1.SetSystemName("Modified System");

        var config2 = DeliverySystemConfiguration.Instance;

        Assert.Equal("Modified System", config2.SystemName);

        config1.SetSystemName(originalName);
    }
}
