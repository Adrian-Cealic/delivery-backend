using DeliverySystem.Domain.Composite;

namespace DeliverySystem.Services;

public sealed class CatalogProvider
{
    public IProductCatalogComponent GetRootCatalog()
    {
        var burger = new ProductCatalogItem("Burger", 12.50m, 0.4m);
        var fries = new ProductCatalogItem("Fries", 4.00m, 0.2m);
        var drink = new ProductCatalogItem("Soft Drink", 2.50m, 0.3m);

        var combo = new ProductBundle("Combo Meal");
        combo.Add(burger);
        combo.Add(fries);
        combo.Add(drink);

        var pizza = new ProductCatalogItem("Pizza", 18.00m, 0.6m);
        var salad = new ProductCatalogItem("Salad", 8.00m, 0.25m);

        var dailyMenu = new ProductBundle("Daily Menu");
        dailyMenu.Add(combo);
        dailyMenu.Add(pizza);
        dailyMenu.Add(salad);

        var root = new ProductBundle("Delivery Catalog");
        root.Add(dailyMenu);
        root.Add(new ProductCatalogItem("Coffee", 3.50m, 0.15m));
        root.Add(new ProductCatalogItem("Dessert", 6.00m, 0.2m));

        return root;
    }
}
