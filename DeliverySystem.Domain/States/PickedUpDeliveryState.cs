namespace DeliverySystem.Domain.States;

public sealed class PickedUpDeliveryState : BaseDeliveryState
{
    public override string Name => "PickedUp";

    public override void StartTransit(DeliveryStateContext context)
    {
        context.TransitionTo(new InTransitDeliveryState());
    }
}
