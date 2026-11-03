using System.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Infrastructure.PostgreSQL;

namespace VeterinaryClinic.Api.Filters;

/// <summary>
/// Объединяет проверку связанных записей и их изменение в одну транзакцию PostgreSQL.
/// </summary>
/// <param name="database">Контекст текущего запроса.</param>
public class WriteTransactionFilter(ClinicDbContext database) : IAsyncActionFilter
{
    /// <inheritdoc />
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (HttpMethods.IsGet(context.HttpContext.Request.Method))
        {
            await next();
            return;
        }

        var cancellationToken = context.HttpContext.RequestAborted;
        await using var transaction = await database.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);
        var result = await next();
        if (result.Exception is null && !result.Canceled)
        {
            await transaction.CommitAsync(cancellationToken);
        }
    }
}
