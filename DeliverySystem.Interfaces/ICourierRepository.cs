using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Interfaces;

public interface ICourierRepository
{
    Courier? GetById(Guid id);
    IEnumerable<Courier> GetAll();
    IEnumerable<Courier> GetAvailable();
    IEnumerable<Courier> GetAvailableForWeight(decimal weight);
    void Add(Courier courier);
    void Update(Courier courier);
    void Delete(Guid id);
}
