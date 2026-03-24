using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Interfaces;

public interface IAccessContext
{
    UserRole GetCurrentRole();
}
