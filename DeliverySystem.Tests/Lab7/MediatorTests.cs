using DeliverySystem.Domain.Mediator;
using DeliverySystem.Services.Mediator;

namespace DeliverySystem.Tests.Lab7;

public class MediatorTests
{
    [Fact]
    public void RequestDispatch_FlowsToAllParticipants()
    {
        var mediator = new DispatchMediator();
        var order = new OrderDispatchParticipant("ord-1");
        var courier = new CourierDispatchParticipant("Alex");
        var notifier = new NotifierDispatchParticipant();

        mediator.Register(order);
        mediator.Register(courier);
        mediator.Register(notifier);

        order.RequestDispatch();

        Assert.Contains("courier.assigned", string.Join(';', notifier.Inbox));
        Assert.NotEmpty(notifier.Emitted);
    }

    [Fact]
    public void Mediator_DoesNotEchoMessageToSender()
    {
        var mediator = new DispatchMediator();
        var order = new OrderDispatchParticipant("ord-2");
        var courier = new CourierDispatchParticipant("Bob");
        mediator.Register(order);
        mediator.Register(courier);

        order.RequestDispatch();

        Assert.DoesNotContain(order.Inbox, m => m.StartsWith("[order.ready]"));
        Assert.Contains(courier.Inbox, m => m.StartsWith("[order.ready]"));
    }

    [Fact]
    public void Participant_NotRegistered_ThrowsOnSend()
    {
        var lone = new OrderDispatchParticipant("ord-3");

        Assert.Throws<InvalidOperationException>(() => lone.RequestDispatch());
    }

    [Fact]
    public void Mediator_RoutesDirectMessageOnlyToTarget()
    {
        var mediator = new DispatchMediator();
        var courier = new CourierDispatchParticipant("Charlie");
        var notifier = new NotifierDispatchParticipant();
        mediator.Register(courier);
        mediator.Register(notifier);

        // Use a synthetic sender to push a direct message
        var sender = new OrderDispatchParticipant("ord-4");
        mediator.Register(sender);
        mediator.Send(sender, new DispatchMessage("delivery.start", "go", TargetId: courier.Id));

        Assert.Contains(courier.Inbox, m => m.Contains("delivery.start"));
        Assert.DoesNotContain(notifier.Inbox, m => m.Contains("delivery.start"));
    }

    [Fact]
    public void Mediator_LogsRegistrationsAndMessages()
    {
        var mediator = new DispatchMediator();
        var order = new OrderDispatchParticipant("ord-5");
        var courier = new CourierDispatchParticipant("Diana");
        mediator.Register(order);
        mediator.Register(courier);
        order.RequestDispatch();

        Assert.Contains(mediator.Log, e => e.StartsWith("REGISTER"));
        Assert.Contains(mediator.Log, e => e.Contains("order.ready"));
    }

    [Fact]
    public void RegisterNullParticipant_Throws()
    {
        var mediator = new DispatchMediator();
        Assert.Throws<ArgumentNullException>(() => mediator.Register(null!));
    }

    [Fact]
    public void NotifierEmits_AfterFullDispatchFlow()
    {
        var mediator = new DispatchMediator();
        var order = new OrderDispatchParticipant("ord-6");
        var courier = new CourierDispatchParticipant("Eve");
        var notifier = new NotifierDispatchParticipant();
        mediator.Register(order);
        mediator.Register(courier);
        mediator.Register(notifier);

        order.RequestDispatch();
        // Trigger delivery by sending direct to the courier
        mediator.Send(order, new DispatchMessage("delivery.start", "go", courier.Id));

        Assert.Contains(notifier.Emitted, e => e.Contains("on the way"));
        Assert.Contains(notifier.Emitted, e => e.Contains("delivery completed"));
    }
}
