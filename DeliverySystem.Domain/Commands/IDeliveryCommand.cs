namespace DeliverySystem.Domain.Commands;

public interface IDeliveryCommand
{
    string Description { get; }
    void Execute();
    void Undo();
}
