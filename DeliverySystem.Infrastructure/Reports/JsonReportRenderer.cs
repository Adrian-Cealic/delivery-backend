using System.Text.Json;
using DeliverySystem.Interfaces.Reports;

namespace DeliverySystem.Infrastructure.Reports;

public sealed class JsonReportRenderer : IReportRenderer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public string Render(string title, IReadOnlyDictionary<string, string> data)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Report title cannot be empty.", nameof(title));

        if (data == null)
            throw new ArgumentNullException(nameof(data));

        var report = new
        {
            Title = title,
            GeneratedAt = DateTime.UtcNow.ToString("O"),
            Data = data
        };

        return JsonSerializer.Serialize(report, JsonOptions);
    }
}
