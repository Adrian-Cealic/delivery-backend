using DeliverySystem.Domain.Builders;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Tests.Lab3;

public class OrderBuilderTests
{
    private readonly Guid _validCustomerId = Guid.NewGuid();

    private OrderItem CreateTestItem(string name = "Test Product", int qty = 1, decimal price = 10.0m, decimal weight = 0.5m)
    {
        return new OrderItem(name, qty, price, weight);
    }

    [Fact]
    public void Build_WithAllFields_CreatesOrderCorrectly()
    {
        var builder = new StandardOrderBuilder();
        var item = CreateTestItem();

        var order = builder
            .SetCustomerId(_validCustomerId)
            .AddItem(item)
            .SetPriority(OrderPriority.Express)
            .SetDeliveryNotes("Leave at door")
            .Build();

        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(_validCustomerId, order.CustomerId);
        Assert.Single(order.Items);
        Assert.Equal(OrderPriority.Express, order.Priority);
        Assert.Equal("Leave at door", order.DeliveryNotes);
        Assert.Equal(OrderStatus.Created, order.Status);
    }

    [Fact]
    public void Build_WithMultipleItems_AddsAllItems()
    {
        var builder = new StandardOrderBuilder();

        var order = builder
            .SetCustomerId(_validCustomerId)
            .AddItem(CreateTestItem("Item A"))
            .AddItem(CreateTestItem("Item B"))
            .AddItem(CreateTestItem("Item C"))
            .Build();

        Assert.Equal(3, order.Items.Count);
    }

    [Fact]
    public void Build_DefaultPriority_IsNormal()
    {
        var builder = new StandardOrderBuilder();

        var order = builder
            .SetCustomerId(_validCustomerId)
            .AddItem(CreateTestItem())
            .Build();

        Assert.Equal(OrderPriority.Normal, order.Priority);
    }

    [Fact]
    public void Build_WithoutCustomerId_ThrowsException()
    {
        var builder = new StandardOrderBuilder();
        builder.AddItem(CreateTestItem());

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void Build_WithoutItems_ThrowsException()
    {
        var builder = new StandardOrderBuilder();
        builder.SetCustomerId(_validCustomerId);

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void Reset_ClearsAllFields()
    {
        var builder = new StandardOrderBuilder();

        builder
            .SetCustomerId(_validCustomerId)
            .AddItem(CreateTestItem())
            .SetPriority(OrderPriority.Express)
            .SetDeliveryNotes("Notes")
            .Reset();

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void FluentApi_ReturnsSameBuilderInstance()
    {
        var builder = new StandardOrderBuilder();

        var result = builder
            .SetCustomerId(_validCustomerId)
            .AddItem(CreateTestItem())
            .SetPriority(OrderPriority.Economy)
            .SetDeliveryNotes("Test");

        Assert.Same(builder, result);
    }

    [Fact]
    public void Director_BuildStandardOrder_SetsNormalPriority()
    {
        var builder = new StandardOrderBuilder();
        var director = new OrderDirector(builder);
        var items = new[] { CreateTestItem() };

        var order = director.BuildStandardOrder(_validCustomerId, items);

        Assert.Equal(OrderPriority.Normal, order.Priority);
        Assert.Single(order.Items);
    }

    [Fact]
    public void Director_BuildExpressOrder_SetsExpressPriority()
    {
        var builder = new StandardOrderBuilder();
        var director = new OrderDirector(builder);
        var items = new[] { CreateTestItem() };

        var order = director.BuildExpressOrder(_validCustomerId, items);

        Assert.Equal(OrderPriority.Express, order.Priority);
        Assert.NotNull(order.DeliveryNotes);
    }

    [Fact]
    public void Director_BuildEconomyOrder_SetsEconomyPriority()
    {
        var builder = new StandardOrderBuilder();
        var director = new OrderDirector(builder);
        var items = new[] { CreateTestItem() };

        var order = director.BuildEconomyOrder(_validCustomerId, items);

        Assert.Equal(OrderPriority.Economy, order.Priority);
        Assert.Contains("Economy", order.DeliveryNotes);
    }

    [Fact]
    public void Director_BuildMultipleOrders_ProducesDifferentIds()
    {
        var builder = new StandardOrderBuilder();
        var director = new OrderDirector(builder);
        var items = new[] { CreateTestItem() };

        var order1 = director.BuildStandardOrder(_validCustomerId, items);
        var order2 = director.BuildExpressOrder(_validCustomerId, items);

        Assert.NotEqual(order1.Id, order2.Id);
    }
}
