namespace VeterinaryClinic.Domain.Repositories;

/// <summary>
/// Хранение сущностей одного типа.
/// </summary>
/// <typeparam name="T">Тип доменной сущности.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Возвращает все записи.
    /// </summary>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Список записей.</returns>
    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Находит запись по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Запись или null.</returns>
    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет запись и назначает новый идентификатор.
    /// </summary>
    /// <param name="entity">Добавляемая запись.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Запись с назначенным идентификатором.</returns>
    public Task<T> AddAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет свойства существующего объекта, сохраняя ссылки на него.
    /// </summary>
    /// <param name="entity">Новые данные с идентификатором изменяемой записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Удалось ли найти и обновить запись.</returns>
    public Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет запись по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Существовала ли удалённая запись.</returns>
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
