namespace Core.Model.OptionModel;

public class LoggingOptions : Options
{
    public bool IncludeFormattedMessage { get; set; } = false;
    public bool IncludeScopes { get; set; } = false;
    public bool ParseStateValues { get; set; } = false;
    public string OtlpExportUrl { get; set; }
}