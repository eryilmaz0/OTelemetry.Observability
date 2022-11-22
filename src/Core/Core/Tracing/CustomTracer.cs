using OpenTelemetry.Trace;

namespace Core.Tracing;

public interface ICustomTracer
{
    public void Trace(OperationType operationType, string actionName, IDictionary<string, string> metrics);
}


public class OpenTelemetryCustomTracer : ICustomTracer
{
    public void Trace(OperationType operationType, string actionName, IDictionary<string, string> metrics)
    {
        var tracer = OpenTelemetryTracerBuilder.CreateTracer(operationType);
        var activSpan = tracer.StartActiveSpan(actionName);

        foreach (var metricPair in metrics)
        {
            activSpan.SetAttribute(metricPair.Key, metricPair.Value);
        }
        
        activSpan.SetStatus(Status.Ok);
        SetDefaultAttributes(activSpan);
        activSpan.End();
    }
    
    
    private static void SetDefaultAttributes(TelemetrySpan span)
    {
        span.SetAttribute("TraceId", Tracer.CurrentSpan.Context.TraceId.ToString());
        span.SetAttribute("SpanId", Tracer.CurrentSpan.Context.SpanId.ToString());
        span.SetAttribute("ParentSpanId", Tracer.CurrentSpan.ParentSpanId.ToString());
    }
}



public static class OpenTelemetryTracerBuilder
{
    private readonly static IDictionary<OperationType, string> _metricTracerNames = new Dictionary<OperationType, string>()
    {
        { OperationType.EventPublish , "MassTransit"},
        { OperationType.HandledEvent , "MassTransit"},
        { OperationType.ActionExecution , "Performance Metric"},
        { OperationType.Other , "Custom"},
    };
    
    public static Tracer CreateTracer(OperationType operationType)
    {
        string tracerSource = _metricTracerNames[operationType];
        return TracerProvider.Default.GetTracer(tracerSource);
    }
}