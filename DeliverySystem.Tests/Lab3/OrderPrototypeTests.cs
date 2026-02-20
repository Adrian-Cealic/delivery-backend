using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Tests.Lab3;

public class OrderPrototypeTests
{
    private Order CreateSampleOrder()
    {
        var order = new Order(Guid.NewGuid());
        order.SetPriority(OrderPriority.Express);
        order.SetDeliveryNotes("Handle with care");
        order.AddItem(new OrderItem("Laptop", 1, 999.99m, 2.5m));
        order.AddItem(new OrderItem("Mouse", 2, 25.00m, 0.1m));
        return order;
    }

    [Fact]
    public void Clone_CreatesNewOrderWithDifferentId()
    {
        var original = CreateSampleOrder();

        var clone = original.Clone();

        Assert.NotEqual(original.Id, clone.Id);
    }

    [Fact]
    public void Clone_PreservesCustomerId()
    {
        var original = CreateSampleOrder();

        var clone = original.Clone();

        Assert.Equal(original.CustomerId, clone.CustomerId);
    }

    [Fact]
    public void Clone_PreservesPriority()
    {
        var original = CreateSampleOrder();

        var clone = original.Clone();

        Assert.Equal(original.Priority, clone.Priority);
    }

    [Fact]
    public void Clone_PreservesDeliveryNotes()
    {
        var original = CreateSampleOrder();

        var clone = original.Clone();

        Assert.Equal(original.DeliveryNotes, clone.DeliveryNotes);
    }

    [Fact]
    public void Clone_PreservesItemCount()
    {
        var original = CreateSampleOrder();

        var clone = original.Clone();

        Assert.Equal(original.Items.Count, clone.Items.Count);
    }

    [Fact]
    public void Clone_ResetsStatusToCreated()
    {
        var original = CreateSampleOrder();

        var clone = original.Clone();

        Assert.Equal(OrderStatus.Created, clone.Status);
    }

    [Fact]
    public void Clone_SharesItemReferences_ShallowCopy()
    {
        var original = CreateSampleOrder();

        var clone = original.Clone();

        Assert.Same(original.Items[0], clone.Items[0]);
        Assert.Same(original.Items[1], clone.Items[1]);
    }

    [Fact]
    public void DeepCopy_CreatesNewOrderWithDifferentId()
    {
        var original = CreateSampleOrder();

        var deepCopy = original.DeepCopy();

        Assert.NotEqual(original.Id, deepCopy.Id);
    }

    [Fact]
    public void DeepCopy_CreatesNewItemInstances()
    {
        var original = CreateSampleOrder();

        var deepCopy = original.DeepCopy();

        Assert.NotSame(original.Items[0], deepCopy.Items[0]);
        Assert.NotSame(original.Items[1], deepCopy.Items[1]);
    }

    [Fact]
    public void DeepCopy_ItemValuesAreEqual()
    {
        var original = CreateSampleOrder();

        var deepCopy = original.DeepCopy();

        Assert.Equal(original.Items[0].ProductName, deepCopy.Items[0].ProductName);
        Assert.Equal(original.Items[0].Quantity, deepCopy.Items[0].Quantity);
        Assert.Equal(original.Items[0].UnitPrice, deepCopy.Items[0].UnitPrice);
        Assert.Equal(original.Items[0].Weight, deepCopy.Items[0].Weight);
    }

    [Fact]
    public void DeepCopy_ModifyingCloneDoesNotAffectOriginal()
    {
        var original = CreateSampleOrder();
        var originalItemCount = original.Items.Count;

        var deepCopy = original.DeepCopy();
        deepCopy.AddItem(new OrderItem("Extra Item", 1, 5.0m, 0.1m));

        Assert.Equal(originalItemCount, original.Items.Count);
        Assert.Equal(originalItemCount + 1, deepCopy.Items.Count);
    }

    [Fact]
    public void Clone_PreservesTotalPrice()
    {
        var original = CreateSampleOrder();

        var clone = original.Clone();

        Assert.Equal(original.GetTotalPrice(), clone.GetTotalPrice());
    }
}
