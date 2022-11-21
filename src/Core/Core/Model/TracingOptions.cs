namespace Core.Model;

public class TracingOptions
{
    public string ServiceName { get; set; }
    public string Version { get; set; }
    public string OtlpExporterUrl { get; set; }
    public bool AddDatabaseTracing { get; set; } = false;
    public bool AddHttpClientTracing { get; set; } = false;
    public bool AddRedisTracing { get; set; } = false;
    public bool AddEventBusTracing { get; set; } = false;
    public bool AddCustomSpanTracing { get; set; } = false;
    public bool AddPerformanceTracing { get; set; } = false;
}