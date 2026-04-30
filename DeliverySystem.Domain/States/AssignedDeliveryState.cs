namespace DeliverySystem.Domain.States;

public sealed class AssignedDeliveryState : BaseDeliveryState
{
    public override string Name => "Assigned";

    public override void PickUp(DeliveryStateContext context)
    {
        context.TransitionTo(new PickedUpDeliveryState());
    }
}
