namespace Core.Model.OptionModel;

public class MetricOptions : Options
{
    public bool AddAspNetCoreMetrics { get; set; } = false;
    public bool AddHttpClientMetrics { get; set; } = false;
    public bool AddRuntimeMetrics { get; set; } = false;
    public bool AddCustomMetrics { get; set; } = false;
}