using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Commands;

public sealed class CompleteDeliveryCommand : IDeliveryCommand
{
    private readonly Delivery _delivery;
    private DeliveryStatus _previousStatus;
    private DateTime? _previousDeliveredAt;
    private bool _wasExecuted;

    public CompleteDeliveryCommand(Delivery delivery)
    {
        _delivery = delivery ?? throw new ArgumentNullException(nameof(delivery));
    }

    public string Description => $"Complete delivery {_delivery.Id}";

    public void Execute()
    {
        _previousStatus = _delivery.Status;
        _previousDeliveredAt = _delivery.DeliveredAt;
        _delivery.MarkAsDelivered();
        _wasExecuted = true;
    }

    public void Undo()
    {
        if (!_wasExecuted)
            throw new InvalidOperationException("Cannot undo a command that was not executed.");

        _delivery.RewindStatus(_previousStatus, deliveredAt: _previousDeliveredAt);
        _wasExecuted = false;
    }
}
