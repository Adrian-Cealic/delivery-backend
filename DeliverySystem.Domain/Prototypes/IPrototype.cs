namespace DeliverySystem.Domain.Prototypes;

public interface IPrototype<T>
{
    T Clone();
    T DeepCopy();
}
