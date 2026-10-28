using Microsoft.AspNetCore.Mvc.Filters;

namespace VeterinaryClinic.Api.Filters;

/// <summary>
/// Записывает результат выполнения действия контроллера без данных запроса.
/// </summary>
/// <param name="logger">Журнал выполнения запросов.</param>
public class RequestLoggingFilter(ILogger<RequestLoggingFilter> logger) : IAsyncActionFilter
{
    /// <summary>
    /// Регистрирует запись метода, действия и итогового статуса ответа.
    /// </summary>
    /// <param name="context">Контекст выполняемого действия.</param>
    /// <param name="next">Следующий фильтр или действие контроллера.</param>
    /// <returns>Задача выполнения действия.</returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var method = context.HttpContext.Request.Method;
        var action = context.ActionDescriptor.DisplayName;
        context.HttpContext.Response.OnCompleted(() =>
        {
            logger.LogInformation("HTTP {Method}, действие {Action}, статус {StatusCode}",
                method, action, context.HttpContext.Response.StatusCode);
            return Task.CompletedTask;
        });
        await next();
    }
}
