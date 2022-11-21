using Microsoft.Extensions.DependencyInjection;

namespace Core.Tracing;

public static class DependencyResolver
{
    public static IServiceCollection AddCustomTracerSupport(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSingleton<ICustomTracer, OpenTelemetryCustomTracer>();
    }
}