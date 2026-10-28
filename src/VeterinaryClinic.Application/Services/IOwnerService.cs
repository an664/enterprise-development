using VeterinaryClinic.Contracts.Owners;
using VeterinaryClinic.Contracts.Pets;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Операции с данными владельца.
/// </summary>
public interface IOwnerService
{
    /// <summary>
    /// Возвращает все записи.
    /// </summary>
    /// <returns>Результат операции.</returns>
    public IReadOnlyList<OwnerDto> GetAll();

    /// <summary>
    /// Возвращает запись по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    public OwnerDto GetById(int id);

    /// <summary>
    /// Создаёт запись.
    /// </summary>
    /// <param name="request">Новые данные записи.</param>
    /// <returns>Результат операции.</returns>
    public OwnerDto Create(OwnerRequest request);

    /// <summary>
    /// Изменяет запись целиком.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="request">Новые данные записи.</param>
    public void Update(int id, OwnerRequest request);

    /// <summary>
    /// Удаляет запись, если нет зависимых объектов.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    public void Delete(int id);

    /// <summary>
    /// Питомцы владельца.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    public IReadOnlyList<PetDto> GetPets(int id);
}
