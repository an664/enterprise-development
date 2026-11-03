using VeterinaryClinic.Contracts.Specializations;
using VeterinaryClinic.Contracts.Veterinarians;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Операции с данными специализации.
/// </summary>
public interface ISpecializationService
{
    /// <summary>
    /// Возвращает все записи.
    /// </summary>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<IReadOnlyList<SpecializationDto>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает запись по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<SpecializationDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Создаёт запись.
    /// </summary>
    /// <param name="request">Новые данные записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<SpecializationDto> CreateAsync(SpecializationRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Изменяет запись целиком.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="request">Новые данные записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    public Task UpdateAsync(int id, SpecializationRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет запись, если нет зависимых объектов.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    public Task DeleteAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Врачи выбранной специализации.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<IReadOnlyList<VeterinarianDto>> GetVeterinariansAsync(int id, CancellationToken cancellationToken);
}
