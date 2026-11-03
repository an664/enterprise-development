using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Specializations;
using VeterinaryClinic.Contracts.Veterinarians;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Операции с данными ветеринара.
/// </summary>
public interface IVeterinarianService
{
    /// <summary>
    /// Возвращает все записи.
    /// </summary>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<IReadOnlyList<VeterinarianDto>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает запись по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<VeterinarianDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Создаёт запись.
    /// </summary>
    /// <param name="request">Новые данные записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<VeterinarianDto> CreateAsync(VeterinarianRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Изменяет запись целиком.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="request">Новые данные записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    public Task UpdateAsync(int id, VeterinarianRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет запись, если нет зависимых объектов.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    public Task DeleteAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Приёмы ветеринара.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<IReadOnlyList<AppointmentDto>> GetAppointmentsAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Специализация ветеринара.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<SpecializationDto> GetSpecializationAsync(int id, CancellationToken cancellationToken);
}
