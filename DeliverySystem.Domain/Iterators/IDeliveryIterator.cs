namespace DeliverySystem.Domain.Iterators;

public interface IDeliveryIterator<T>
{
    bool HasNext();
    T Next();
    void Reset();
}
