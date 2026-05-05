using DeliverySystem.Domain.Commands;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Services.Commands;

namespace DeliverySystem.Tests.Lab6;

public class CommandTests
{
    private static Delivery NewDelivery() => new(Guid.NewGuid(), Guid.NewGuid(), 8m);

    [Fact]
    public void AssignCourier_ExecutesAndUpdatesStatus()
    {
        var delivery = NewDelivery();
        var command = new AssignCourierCommand(delivery);

        command.Execute();

        Assert.Equal(DeliveryStatus.Assigned, delivery.Status);
    }

    [Fact]
    public void AssignCourier_Undo_RevertsToPending()
    {
        var delivery = NewDelivery();
        var command = new AssignCourierCommand(delivery);
        command.Execute();

        command.Undo();

        Assert.Equal(DeliveryStatus.Pending, delivery.Status);
    }

    [Fact]
    public void PickUpCommand_Undo_RestoresPickedUpAt()
    {
        var delivery = NewDelivery();
        new AssignCourierCommand(delivery).Execute();
        var pickUp = new PickUpDeliveryCommand(delivery);
        pickUp.Execute();
        Assert.NotNull(delivery.PickedUpAt);

        pickUp.Undo();

        Assert.Equal(DeliveryStatus.Assigned, delivery.Status);
        Assert.Null(delivery.PickedUpAt);
    }

    [Fact]
    public void Invoker_UndoRedo_RestoresStateAcrossCommands()
    {
        var delivery = NewDelivery();
        var invoker = new DeliveryCommandInvoker();

        invoker.Execute(new AssignCourierCommand(delivery));
        invoker.Execute(new PickUpDeliveryCommand(delivery));
        invoker.Execute(new StartTransitCommand(delivery));

        Assert.Equal(DeliveryStatus.InTransit, delivery.Status);

        invoker.Undo();
        Assert.Equal(DeliveryStatus.PickedUp, delivery.Status);
        invoker.Undo();
        Assert.Equal(DeliveryStatus.Assigned, delivery.Status);

        invoker.Redo();
        Assert.Equal(DeliveryStatus.PickedUp, delivery.Status);
    }

    [Fact]
    public void Invoker_NewExecuteAfterUndo_ClearsRedoStack()
    {
        var delivery = NewDelivery();
        var invoker = new DeliveryCommandInvoker();
        invoker.Execute(new AssignCourierCommand(delivery));
        invoker.Undo();

        invoker.Execute(new AssignCourierCommand(delivery));

        Assert.False(invoker.CanRedo);
    }

    [Fact]
    public void Invoker_UndoWithoutHistory_Throws()
    {
        var invoker = new DeliveryCommandInvoker();

        Assert.Throws<InvalidOperationException>(() => invoker.Undo());
    }

    [Fact]
    public void MacroCommand_ExecutesAllInOrder()
    {
        var delivery = NewDelivery();
        var commands = new IDeliveryCommand[]
        {
            new AssignCourierCommand(delivery),
            new PickUpDeliveryCommand(delivery),
            new StartTransitCommand(delivery)
        };
        var macro = new MacroDeliveryCommand("dispatch", commands);

        macro.Execute();

        Assert.Equal(DeliveryStatus.InTransit, delivery.Status);
    }

    [Fact]
    public void MacroCommand_Undo_RewindsAllCommands()
    {
        var delivery = NewDelivery();
        var macro = new MacroDeliveryCommand("dispatch", new IDeliveryCommand[]
        {
            new AssignCourierCommand(delivery),
            new PickUpDeliveryCommand(delivery)
        });
        macro.Execute();

        macro.Undo();

        Assert.Equal(DeliveryStatus.Pending, delivery.Status);
    }

    [Fact]
    public void Invoker_HistoryReflectsActions()
    {
        var delivery = NewDelivery();
        var invoker = new DeliveryCommandInvoker();
        invoker.Execute(new AssignCourierCommand(delivery));
        invoker.Undo();
        invoker.Redo();

        Assert.Equal(3, invoker.History.Count);
        Assert.StartsWith("EXEC", invoker.History[0]);
        Assert.StartsWith("UNDO", invoker.History[1]);
        Assert.StartsWith("REDO", invoker.History[2]);
    }
}
