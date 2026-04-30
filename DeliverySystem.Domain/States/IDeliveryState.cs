namespace DeliverySystem.Domain.States;

public interface IDeliveryState
{
    string Name { get; }
    bool IsTerminal { get; }

    void Assign(DeliveryStateContext context);
    void PickUp(DeliveryStateContext context);
    void StartTransit(DeliveryStateContext context);
    void Complete(DeliveryStateContext context);
    void Fail(DeliveryStateContext context, string reason);
}
