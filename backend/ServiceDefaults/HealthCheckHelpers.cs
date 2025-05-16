using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ServiceDefaults;

[ExcludeFromCodeCoverage]
public static class HealthCheckHelpers
{
    public const string SelfName = "Self";

    public static readonly Func<HealthCheckRegistration, bool> Self = r => r.Tags.Contains(SelfName);
    public static readonly Func<HealthCheckRegistration, bool> None = _ => false;
    public static readonly Func<HealthCheckRegistration, bool> All = _ => true;

    public static HealthCheckOptions CreateHealthCheck(Func<HealthCheckRegistration, bool>? filter = null)
    {
        filter ??= _ => true;

        return new HealthCheckOptions
        {
            AllowCachingResponses = false,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            },
            ResponseWriter = WriteResponse,
            Predicate = filter
        };
    }

    public static Task WriteResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = false };

        using var memoryStream = new MemoryStream();

        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            jsonWriter.WriteString("duration", healthReport.TotalDuration.ToString());

            if (healthReport.Entries.Any())
            {
                jsonWriter.WriteStartObject("results");

                foreach (var healthReportEntry in healthReport.Entries)
                {
                    jsonWriter.WriteStartObject(healthReportEntry.Key);

                    jsonWriter.WriteString("status", healthReportEntry.Value.Status.ToString());

                    jsonWriter.WriteString("description", healthReportEntry.Value.Description);

                    if (healthReportEntry.Value.Data is { Count: > 0 })
                    {
                        jsonWriter.WriteStartObject("data");

                        foreach (var item in healthReportEntry.Value.Data)
                        {
                            jsonWriter.WritePropertyName(item.Key);

                            JsonSerializer.Serialize(jsonWriter, item.Value, item.Value?.GetType() ?? typeof(object));
                        }

                        jsonWriter.WriteEndObject();
                    }

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
        }

        return context.Response.WriteAsync(Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}