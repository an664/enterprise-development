using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Contracts.Veterinarians;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Операции с данными приёма.
/// </summary>
public interface IAppointmentService
{
    /// <summary>
    /// Возвращает все записи.
    /// </summary>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<IReadOnlyList<AppointmentDto>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает запись по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<AppointmentDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Создаёт запись.
    /// </summary>
    /// <param name="request">Новые данные записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<AppointmentDto> CreateAsync(AppointmentRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Изменяет запись целиком.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="request">Новые данные записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    public Task UpdateAsync(int id, AppointmentRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет запись, если нет зависимых объектов.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    public Task DeleteAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Питомец, записанный на приём.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<PetDto> GetPetAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Ветеринар, проводящий приём.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<VeterinarianDto> GetVeterinarianAsync(int id, CancellationToken cancellationToken);
}
