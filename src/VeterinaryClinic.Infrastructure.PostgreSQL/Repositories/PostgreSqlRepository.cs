using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Infrastructure.PostgreSQL.Repositories;

/// <summary>
/// Общие операции с таблицей сущностей, ключ которой - Id.
/// </summary>
/// <typeparam name="T">Доменная сущность.</typeparam>
/// <param name="db">Контекст текущего запроса.</param>
/// <param name="getId">Чтение ключа из сущности.</param>
internal abstract class PostgreSqlRepository<T>(ClinicDbContext db, Func<T, int> getId) : IRepository<T>
    where T : class
{
    /// <summary>
    /// Контекст текущего запроса.
    /// </summary>
    protected ClinicDbContext Database { get; } = db;

    /// <summary>
    /// Таблица сущностей текущего типа.
    /// </summary>
    protected DbSet<T> Entities => Database.Set<T>();

    /// <inheritdoc />
    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await Entities.AsNoTracking().OrderBy(entity => EF.Property<int>(entity, "Id"))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return Entities.SingleOrDefaultAsync(entity => EF.Property<int>(entity, "Id") == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken)
    {
        Entities.Add(entity);
        await Database.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        var current = await Entities.FindAsync([getId(entity)], cancellationToken);
        if (current is null)
        {
            return false;
        }

        ApplyChanges(current, entity);
        await Database.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var deleted = await Entities.Where(entity => EF.Property<int>(entity, "Id") == id)
            .ExecuteDeleteAsync(cancellationToken);
        return deleted != 0;
    }

    /// <summary>
    /// Переносит новые поля в найденную запись, не изменяя её ключ.
    /// </summary>
    /// <param name="current">Запись, отслеживаемая контекстом.</param>
    /// <param name="updated">Новые данные.</param>
    protected abstract void ApplyChanges(T current, T updated);
}
