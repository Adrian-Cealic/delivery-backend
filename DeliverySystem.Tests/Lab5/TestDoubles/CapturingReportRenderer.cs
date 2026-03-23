using DeliverySystem.Interfaces.Reports;

namespace DeliverySystem.Tests.Lab5.TestDoubles;

public sealed class CapturingReportRenderer : IReportRenderer
{
    public string? LastTitle { get; private set; }
    public IReadOnlyDictionary<string, string>? LastData { get; private set; }

    public string Render(string title, IReadOnlyDictionary<string, string> data)
    {
        LastTitle = title;
        LastData = data;
        return $"CAPTURED:{title}";
    }
}
