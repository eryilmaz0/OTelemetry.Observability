using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Core.Tracing;

public static class TraceEnricher
{
    public static void HttpActivityEnrichment(Activity activity, string eventName, object rawObject)
    {
        if (eventName.Equals("OnStartActivity") && rawObject is HttpRequestMessage httpRequest)
        {
            var request = "EMPTY!";
            if (httpRequest.Content != null)
            {
                request = httpRequest.Content.ReadAsStringAsync().Result;

            }
            activity.SetTag("http.request_content", request);

        }

        if (eventName.Equals("OnStopActivity") && rawObject is HttpResponseMessage httpResponse)
        {
            var response = "EMPTY!";
            if (httpResponse.Content != null)
            {
                response = httpResponse.Content.ReadAsStringAsync().Result;
            }

            activity.SetTag("http.response_content", response);

        }

        if (eventName.Equals("OnException") && rawObject is Exception exception)
        {
            activity.SetTag("http.exception", exception.Message);
        }

        SetTraceId(activity);
    }
    
    
    public static void AspNetCoreActivityEnrichment(Activity activity, string eventName, object rawObject)
    {
        if ((activity.GetTagItem("http.target") ?? "/") == "/")
            activity.DisplayName = "Home";
        else
            activity.DisplayName = activity.GetTagItem("http.target")?.ToString();


        if (eventName.Equals("OnStartActivity"))
        {
            if (rawObject is HttpRequest httpRequest)
            {
                if (httpRequest.Query.ContainsKey("fordays"))
                {
                    activity.SetTag("data.ForDays", httpRequest.Query["fordays"].ToString());
                }
            }
        }
        else if (eventName.Equals("OnStopActivity"))
        {
            if (rawObject is HttpResponse httpResponse)
            {
                activity.SetTag("data.ResponseType", httpResponse.ContentType);
            }
        }

        SetTraceId(activity);
    }
    
    
    private static void SetTraceId(Activity activity)
    {
        activity.AddTag("trc.TraceId", activity.TraceId.ToString());
        activity.AddTag("trc.TraceSpanId", activity.SpanId.ToString());
        activity.AddTag("trc.TraceParentSpanId", activity.Parent?.SpanId.ToString());
    }
}