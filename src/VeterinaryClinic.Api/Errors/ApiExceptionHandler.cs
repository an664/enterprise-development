using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using VeterinaryClinic.Application;

namespace VeterinaryClinic.Api.Errors;

/// <summary>
/// Преобразует ошибки приложения в ответы HTTP.
/// </summary>
/// <param name="logger">Журнал ошибок.</param>
public class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    /// <summary>
    /// Возвращает описание известной ошибки или безопасный ответ при внутреннем сбое.
    /// </summary>
    /// <param name="httpContext">Контекст HTTP-запроса.</param>
    /// <param name="exception">Возникшая ошибка.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Признак обработки ошибки.</returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var status = exception switch
        {
            EntityNotFoundException => StatusCodes.Status404NotFound,
            EntityInUseException => StatusCodes.Status409Conflict,
            ValidationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
        var trace = new Dictionary<string, object?>
        {
            ["traceId"] = httpContext.TraceIdentifier
        };
        IResult result;
        if (exception is ValidationException)
        {
            result = Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["request"] = [exception.Message]
                },
                title: "Некорректные данные запроса.",
                extensions: trace);
        }
        else
        {
            var title = status switch
            {
                StatusCodes.Status404NotFound => "Запись не найдена.",
                StatusCodes.Status409Conflict => "Запись используется другими данными.",
                _ => "Внутренняя ошибка сервера."
            };
            result = Results.Problem(
                statusCode: status,
                title: title,
                detail: status == StatusCodes.Status500InternalServerError ? null : exception.Message,
                extensions: trace);
        }

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Внутренняя ошибка запроса {TraceId}", httpContext.TraceIdentifier);
        }

        await result.ExecuteAsync(httpContext);
        return true;
    }
}
