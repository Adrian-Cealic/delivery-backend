using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Infrastructure.Observer;
using DeliverySystem.Interfaces.Observer;

namespace DeliverySystem.Tests.Lab6;

public class ObserverTests
{
    private static Delivery NewDelivery() => new(Guid.NewGuid(), Guid.NewGuid(), 5m);

    [Fact]
    public void Subject_NotifiesAllAttachedObservers()
    {
        var subject = new DeliveryStatusSubject();
        var dashboard = new DashboardDeliveryObserver();
        var emailSink = new List<string>();
        var email = new EmailDeliveryObserver("user@example.com", emailSink.Add);

        subject.Attach(dashboard);
        subject.Attach(email);

        var delivery = NewDelivery();
        delivery.MarkAsAssigned();
        subject.Notify(delivery, DeliveryStatus.Pending);

        Assert.Single(dashboard.Events);
        Assert.Equal(DeliveryStatus.Assigned, dashboard.Events[0].To);
        Assert.Single(emailSink);
        Assert.Contains("user@example.com", emailSink[0]);
    }

    [Fact]
    public void DetachedObserver_DoesNotReceiveNotifications()
    {
        var subject = new DeliveryStatusSubject();
        var dashboard = new DashboardDeliveryObserver();
        subject.Attach(dashboard);
        subject.Detach(dashboard);

        var delivery = NewDelivery();
        delivery.MarkAsAssigned();
        subject.Notify(delivery, DeliveryStatus.Pending);

        Assert.Empty(dashboard.Events);
    }

    [Fact]
    public void Attach_SameObserverTwice_OnlyAddsOnce()
    {
        var subject = new DeliveryStatusSubject();
        var dashboard = new DashboardDeliveryObserver();

        subject.Attach(dashboard);
        subject.Attach(dashboard);

        Assert.Single(subject.AttachedChannels);
    }

    [Fact]
    public void SmsObserver_OnlyEmitsForCriticalStatuses()
    {
        var captured = new List<(string To, string Body)>();
        var sms = new SmsDeliveryObserver("+37312345678", (t, b) => captured.Add((t, b)));
        var subject = new DeliveryStatusSubject();
        subject.Attach(sms);

        var delivery = NewDelivery();
        delivery.MarkAsAssigned();
        subject.Notify(delivery, DeliveryStatus.Pending);

        Assert.Empty(captured);

        delivery.MarkAsPickedUp();
        delivery.MarkAsInTransit();
        subject.Notify(delivery, DeliveryStatus.PickedUp);

        Assert.Single(captured);
        Assert.Equal("+37312345678", captured[0].To);
    }

    [Fact]
    public void DashboardObserver_RecordsTransitionInOrder()
    {
        var subject = new DeliveryStatusSubject();
        var dashboard = new DashboardDeliveryObserver();
        subject.Attach(dashboard);

        var delivery = NewDelivery();
        delivery.MarkAsAssigned();
        subject.Notify(delivery, DeliveryStatus.Pending);
        delivery.MarkAsPickedUp();
        subject.Notify(delivery, DeliveryStatus.Assigned);

        Assert.Equal(2, dashboard.Events.Count);
        Assert.Equal(DeliveryStatus.Pending, dashboard.Events[0].From);
        Assert.Equal(DeliveryStatus.Assigned, dashboard.Events[1].From);
        Assert.Equal(DeliveryStatus.PickedUp, dashboard.Events[1].To);
    }

    [Fact]
    public void Notify_WithNoObservers_DoesNotThrow()
    {
        var subject = new DeliveryStatusSubject();
        var delivery = NewDelivery();

        var ex = Record.Exception(() => subject.Notify(delivery, DeliveryStatus.Pending));

        Assert.Null(ex);
    }

    [Fact]
    public void Attach_NullObserver_Throws()
    {
        var subject = new DeliveryStatusSubject();

        Assert.Throws<ArgumentNullException>(() => subject.Attach(null!));
    }
}
