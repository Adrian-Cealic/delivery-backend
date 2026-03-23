using DeliverySystem.Interfaces.Reports;

namespace DeliverySystem.Services.Reports;

public abstract class DeliveryReport
{
    protected IReportRenderer Renderer { get; }

    protected DeliveryReport(IReportRenderer renderer)
    {
        Renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
    }

    public abstract string Generate();
}
