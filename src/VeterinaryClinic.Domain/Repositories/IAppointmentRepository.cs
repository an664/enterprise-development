using VeterinaryClinic.Domain.Models;

namespace VeterinaryClinic.Domain.Repositories;

/// <summary>
/// Репозиторий приёма.
/// </summary>
public interface IAppointmentRepository : IRepository<Appointment>
{
    /// <summary>
    /// Приёмы питомца.
    /// </summary>
    /// <param name="petId">Идентификатор питомца.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат выборки.</returns>
    public Task<IReadOnlyList<Appointment>> GetByPetAsync(int petId, CancellationToken cancellationToken);

    /// <summary>
    /// Проверяет, был ли у питомца приём раньше указанной даты.
    /// </summary>
    /// <param name="petId">Идентификатор питомца.</param>
    /// <param name="date">Дата, не входящая в проверяемый интервал.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Есть ли хотя бы один более ранний приём.</returns>
    public Task<bool> HasAppointmentBeforeAsync(int petId, DateOnly date, CancellationToken cancellationToken);

    /// <summary>
    /// Приёмы врача.
    /// </summary>
    /// <param name="veterinarianId">Идентификатор ветеринара.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат выборки.</returns>
    public Task<IReadOnlyList<Appointment>> GetByVeterinarianAsync(
        int veterinarianId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Приёмы кабинета в интервале [from, to), отсортированные по времени.
    /// </summary>
    /// <param name="from">Начало периода включительно.</param>
    /// <param name="to">Конец периода исключительно.</param>
    /// <param name="roomNumber">Номер кабинета.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат выборки.</returns>
    public Task<IReadOnlyList<Appointment>> GetForPeriodAsync(
        DateTime from,
        DateTime to,
        string roomNumber,
        CancellationToken cancellationToken);

    /// <summary>
    /// Число повторных приёмов выбранной породы.
    /// </summary>
    /// <param name="breedId">Идентификатор породы.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Количество повторных приёмов.</returns>
    public Task<int> CountFollowUpsByBreedAsync(int breedId, CancellationToken cancellationToken);
}
