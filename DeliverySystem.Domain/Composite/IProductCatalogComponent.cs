using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Composite;

public interface IProductCatalogComponent
{
    string Name { get; }
    decimal GetTotalPrice();
    decimal GetTotalWeight();
    IReadOnlyList<IProductCatalogComponent> GetChildren();
    IEnumerable<OrderItem> ToOrderItems(int quantity = 1);
}
