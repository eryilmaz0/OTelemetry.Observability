namespace Core.Model.OptionModel;

public class TracingOptions : Options
{
    public bool AddDatabaseTracing { get; set; } = false;
    public bool AddHttpClientTracing { get; set; } = false;
    public bool AddRedisTracing { get; set; } = false;
    public bool AddEventBusTracing { get; set; } = false;
    public bool AddCustomSpanTracing { get; set; } = false;
    public bool AddPerformanceTracing { get; set; } = false;
}