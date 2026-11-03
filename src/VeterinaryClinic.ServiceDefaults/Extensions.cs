using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace VeterinaryClinic.ServiceDefaults;

/// <summary>
/// Общие настройки телеметрии и проверки готовности API.
/// </summary>
public static class Extensions
{
    extension(WebApplicationBuilder builder)
    {
        /// <summary>
        /// Регистрирует журналы, метрики и трассировку HTTP-запросов для панели Aspire.
        /// </summary>
        public void AddServiceDefaults()
        {
            builder.Services.AddHealthChecks();
            builder.Logging.AddOpenTelemetry(options =>
            {
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
            });
            var telemetry = builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics => metrics.AddAspNetCoreInstrumentation().AddRuntimeInstrumentation())
                .WithTracing(tracing => tracing.AddAspNetCoreInstrumentation(options =>
                    options.Filter = context => !context.Request.Path.StartsWithSegments("/health")));

            if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
            {
                telemetry.UseOtlpExporter();
            }
        }
    }

    extension(WebApplication app)
    {
        /// <summary>
        /// Открывает проверку готовности для локального запуска через Aspire.
        /// </summary>
        public void MapDefaultEndpoints()
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapHealthChecks("/health");
            }
        }
    }
}
