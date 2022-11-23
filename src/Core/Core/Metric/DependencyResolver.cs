using Core.Model.OptionModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Metric;

public static class DependencyResolver
{
    public static void AddMetricSupport(this WebApplicationBuilder builder)
    {
        MetricOptions options = builder.Configuration.GetSection("MetricOptions").Get<MetricOptions>();

        if (options is null)
            throw new ArgumentNullException();

        builder.Services.AddMetricSupport(options);
    }
}