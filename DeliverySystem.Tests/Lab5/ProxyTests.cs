using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Infrastructure.Repositories;
using DeliverySystem.Infrastructure.Repositories.Proxies;
using DeliverySystem.Interfaces;

namespace DeliverySystem.Tests.Lab5;

public class ProxyTests
{
    private sealed class StubAccessContext : IAccessContext
    {
        public UserRole Role { get; set; }
        public UserRole GetCurrentRole() => Role;
    }

    private static (ProtectionOrderRepositoryProxy Proxy, InMemoryOrderRepository Real, StubAccessContext Context) CreateProxy()
    {
        var real = new InMemoryOrderRepository();
        var context = new StubAccessContext { Role = UserRole.Admin };
        var proxy = new ProtectionOrderRepositoryProxy(real, context);
        return (proxy, real, context);
    }

    [Fact]
    public void Admin_CanAdd_Order()
    {
        var (proxy, real, _) = CreateProxy();
        var order = new Order(Guid.NewGuid());

        proxy.Add(order);

        Assert.NotNull(real.GetById(order.Id));
    }

    [Fact]
    public void Courier_CannotAdd_ThrowsUnauthorized()
    {
        var (proxy, _, context) = CreateProxy();
        context.Role = UserRole.Courier;

        Assert.Throws<UnauthorizedAccessException>(() =>
            proxy.Add(new Order(Guid.NewGuid())));
    }

    [Fact]
    public void Courier_CanRead_Orders()
    {
        var (proxy, real, context) = CreateProxy();
        var order = new Order(Guid.NewGuid());
        real.Add(order);
        context.Role = UserRole.Courier;

        var result = proxy.GetById(order.Id);

        Assert.NotNull(result);
        Assert.Equal(order.Id, result!.Id);
    }

    [Fact]
    public void NoneRole_CannotRead_ThrowsUnauthorized()
    {
        var (proxy, _, context) = CreateProxy();
        context.Role = UserRole.None;

        Assert.Throws<UnauthorizedAccessException>(() => proxy.GetAll());
    }

    [Fact]
    public void Proxy_DelegatesToRealRepository()
    {
        var (proxy, real, _) = CreateProxy();
        var order1 = new Order(Guid.NewGuid());
        var order2 = new Order(Guid.NewGuid());

        proxy.Add(order1);
        proxy.Add(order2);

        Assert.Equal(2, real.GetAll().Count());
        Assert.Equal(2, proxy.GetAll().Count());
    }

    [Fact]
    public void Admin_CanDelete_Courier_Cannot()
    {
        var (proxy, real, context) = CreateProxy();
        var order = new Order(Guid.NewGuid());
        real.Add(order);

        context.Role = UserRole.Courier;
        Assert.Throws<UnauthorizedAccessException>(() => proxy.Delete(order.Id));

        context.Role = UserRole.Admin;
        proxy.Delete(order.Id);

        Assert.Null(real.GetById(order.Id));
    }
}
