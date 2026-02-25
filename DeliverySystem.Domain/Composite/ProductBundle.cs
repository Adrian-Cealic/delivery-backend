using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Composite;

public sealed class ProductBundle : IProductCatalogComponent
{
    public string Name { get; }
    private readonly List<IProductCatalogComponent> _children;

    public ProductBundle(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        Name = name;
        _children = new List<IProductCatalogComponent>();
    }

    public ProductBundle(string name, IEnumerable<IProductCatalogComponent> children)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        Name = name;
        _children = new List<IProductCatalogComponent>(children ?? []);
    }

    public void Add(IProductCatalogComponent component)
    {
        if (component == null)
            throw new ArgumentNullException(nameof(component));
        _children.Add(component);
    }

    public void Remove(IProductCatalogComponent component)
    {
        _children.Remove(component);
    }

    public decimal GetTotalPrice() => _children.Sum(c => c.GetTotalPrice());

    public decimal GetTotalWeight() => _children.Sum(c => c.GetTotalWeight());

    public IReadOnlyList<IProductCatalogComponent> GetChildren() => _children.AsReadOnly();

    public IEnumerable<OrderItem> ToOrderItems(int quantity = 1)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        foreach (var child in _children)
        {
            foreach (var item in child.ToOrderItems(quantity))
                yield return item;
        }
    }
}
