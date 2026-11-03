using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Infrastructure.PostgreSQL;

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
        var cause = exception.GetBaseException();
        if (cause is PostgresException
            { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: ClinicDbContext.AppointmentSlotIndex })
        {
            cause = new AppointmentConflictException();
        }

        var (status, title) = cause switch
        {
            ValidationException => (400, "Некорректные данные запроса."),
            EntityNotFoundException => (404, "Запись не найдена."),
            AppointmentConflictException => (409, cause.Message),
            EntityInUseException => (409, "Запись используется другими данными."),
            PostgresException { SqlState: PostgresErrorCodes.RestrictViolation or PostgresErrorCodes.ForeignKeyViolation }
                when HttpMethods.IsDelete(httpContext.Request.Method) => (409, "Запись используется другими данными."),
            PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation } => (404, "Связанная запись не найдена."),
            DbUpdateConcurrencyException or PostgresException
            { SqlState: PostgresErrorCodes.SerializationFailure or PostgresErrorCodes.DeadlockDetected } =>
                (409, "Данные изменились. Повторите запрос."),
            _ => (500, "Внутренняя ошибка сервера.")
        };
        var trace = new Dictionary<string, object?>
        {
            ["traceId"] = httpContext.TraceIdentifier
        };
        IResult result;
        if (cause is ValidationException)
        {
            result = Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["request"] = [cause.Message]
                },
                title: title,
                extensions: trace);
        }
        else
        {
            result = Results.Problem(
                statusCode: status,
                title: title,
                detail: cause is EntityNotFoundException or EntityInUseException ? cause.Message : null,
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
