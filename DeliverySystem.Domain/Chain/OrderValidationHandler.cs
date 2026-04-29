namespace DeliverySystem.Domain.Chain;

public abstract class OrderValidationHandler
{
    private OrderValidationHandler? _next;

    public OrderValidationHandler SetNext(OrderValidationHandler next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        return next;
    }

    public void Handle(OrderValidationContext context, OrderValidationResult result)
    {
        Process(context, result);

        if (!result.IsAccepted && StopOnFailure)
            return;

        _next?.Handle(context, result);
    }

    protected abstract string HandlerName { get; }
    protected virtual bool StopOnFailure => false;

    protected abstract void Process(OrderValidationContext context, OrderValidationResult result);
}
