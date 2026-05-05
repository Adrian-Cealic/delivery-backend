using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Commands;

public sealed class AssignCourierCommand : IDeliveryCommand
{
    private readonly Delivery _delivery;
    private DeliveryStatus _previousStatus;
    private bool _wasExecuted;

    public AssignCourierCommand(Delivery delivery)
    {
        _delivery = delivery ?? throw new ArgumentNullException(nameof(delivery));
    }

    public string Description => $"Assign courier to delivery {_delivery.Id}";

    public void Execute()
    {
        _previousStatus = _delivery.Status;
        _delivery.MarkAsAssigned();
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
