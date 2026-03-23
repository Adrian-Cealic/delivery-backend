namespace DeliverySystem.Interfaces.Reports;

public interface IReportRenderer
{
    string Render(string title, IReadOnlyDictionary<string, string> data);
}
