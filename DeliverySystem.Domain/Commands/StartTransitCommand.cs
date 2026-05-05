using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Commands;

public sealed class StartTransitCommand : IDeliveryCommand
{
    private readonly Delivery _delivery;
    private DeliveryStatus _previousStatus;
    private bool _wasExecuted;

    public StartTransitCommand(Delivery delivery)
    {
        _delivery = delivery ?? throw new ArgumentNullException(nameof(delivery));
    }

    public string Description => $"Start transit for delivery {_delivery.Id}";

    public void Execute()
    {
        _previousStatus = _delivery.Status;
        _delivery.MarkAsInTransit();
        _wasExecuted = true;
    }

    public void Undo()
    {
        if (!_wasExecuted)
            throw new InvalidOperationException("Cannot undo a command that was not executed.");

        _delivery.RewindStatus(_previousStatus);
        _wasExecuted = false;
    }
}
