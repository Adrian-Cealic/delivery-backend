namespace DeliverySystem.Domain.States;

public sealed class PendingDeliveryState : BaseDeliveryState
{
    public override string Name => "Pending";

    public override void Assign(DeliveryStateContext context)
    {
        context.TransitionTo(new AssignedDeliveryState());
    }
}
