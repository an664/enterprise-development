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
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<IReadOnlyList<OwnerDto>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает запись по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<OwnerDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Создаёт запись.
    /// </summary>
    /// <param name="request">Новые данные записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<OwnerDto> CreateAsync(OwnerRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Изменяет запись целиком.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="request">Новые данные записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    public Task UpdateAsync(int id, OwnerRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет запись, если нет зависимых объектов.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    public Task DeleteAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Питомцы владельца.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<IReadOnlyList<PetDto>> GetPetsAsync(int id, CancellationToken cancellationToken);
}
