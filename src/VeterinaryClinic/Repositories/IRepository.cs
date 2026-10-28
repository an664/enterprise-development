namespace VeterinaryClinic.Repositories;

/// <summary>
/// Хранение сущностей одного типа.
/// </summary>
/// <typeparam name="T">Тип доменной сущности.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Возвращает все записи.
    /// </summary>
    /// <returns>Список записей.</returns>
    public IReadOnlyList<T> GetAll();

    /// <summary>
    /// Находит запись по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Запись или null.</returns>
    public T? GetById(int id);

    /// <summary>
    /// Добавляет запись и назначает новый идентификатор.
    /// </summary>
    /// <param name="entity">Добавляемая запись.</param>
    /// <returns>Запись с назначенным идентификатором.</returns>
    public T Add(T entity);

    /// <summary>
    /// Обновляет свойства существующего объекта, сохраняя ссылки на него.
    /// </summary>
    /// <param name="entity">Новые данные с идентификатором изменяемой записи.</param>
    /// <returns>Удалось ли найти и обновить запись.</returns>
    public bool Update(T entity);

    /// <summary>
    /// Удаляет запись по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Существовала ли удалённая запись.</returns>
    public bool Delete(int id);
}
