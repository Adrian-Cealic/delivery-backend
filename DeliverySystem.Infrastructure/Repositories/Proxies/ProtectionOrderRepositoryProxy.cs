using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Interfaces;

namespace DeliverySystem.Infrastructure.Repositories.Proxies;

public sealed class ProtectionOrderRepositoryProxy : IOrderRepository
{
    private readonly IOrderRepository _realRepository;
    private readonly IAccessContext _accessContext;

    public ProtectionOrderRepositoryProxy(IOrderRepository realRepository, IAccessContext accessContext)
    {
        _realRepository = realRepository ?? throw new ArgumentNullException(nameof(realRepository));
        _accessContext = accessContext ?? throw new ArgumentNullException(nameof(accessContext));
    }

    public Order? GetById(Guid id)
    {
        EnsureReadAccess();
        return _realRepository.GetById(id);
    }

    public IEnumerable<Order> GetAll()
    {
        EnsureReadAccess();
        return _realRepository.GetAll();
    }

    public IEnumerable<Order> GetByCustomerId(Guid customerId)
    {
        EnsureReadAccess();
        return _realRepository.GetByCustomerId(customerId);
    }

    public void Add(Order order)
    {
        EnsureWriteAccess();
        _realRepository.Add(order);
    }

    public void Update(Order order)
    {
        EnsureWriteAccess();
        _realRepository.Update(order);
    }

    public void Delete(Guid id)
    {
        EnsureWriteAccess();
        _realRepository.Delete(id);
    }

    private void EnsureReadAccess()
    {
        var role = _accessContext.GetCurrentRole();
        if (role == UserRole.None)
            throw new UnauthorizedAccessException("Authentication required to access orders.");
    }

    private void EnsureWriteAccess()
    {
        var role = _accessContext.GetCurrentRole();
        if (role != UserRole.Admin)
            throw new UnauthorizedAccessException("Admin role required for write operations.");
    }
}
