using VeterinaryClinic.Contracts.Analytics;
using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Owners;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Contracts.Veterinarians;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Пять аналитических запросов ветеринарной клиники.
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Возвращает врачей, специализирующихся на выбранном виде животных.
    /// </summary>
    /// <param name="species">Вид животных.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Подходящие врачи.</returns>
    public Task<IReadOnlyList<VeterinarianDto>> GetVeterinariansAsync(
        AnimalSpecies species,
        CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает питомцев выбранного врача без повторов, по кличке.
    /// </summary>
    /// <param name="veterinarianId">Идентификатор врача.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Питомцы врача.</returns>
    public Task<IReadOnlyList<PetDto>> GetPetsAsync(int veterinarianId, CancellationToken cancellationToken);

    /// <summary>
    /// Считает повторные приёмы питомцев выбранной породы.
    /// </summary>
    /// <param name="breedId">Идентификатор породы.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Количество повторных приёмов.</returns>
    public Task<FollowUpCountDto> GetFollowUpCountAsync(int breedId, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает владельцев нескольких питомцев, по ФИО.
    /// </summary>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Владельцы более одного питомца.</returns>
    public Task<IReadOnlyList<OwnerDto>> GetOwnersWithMultiplePetsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает приёмы кабинета за месяц от выбранной даты, по времени.
    /// </summary>
    /// <param name="asOfDate">Начальная дата, включаемая в выборку.</param>
    /// <param name="roomNumber">Номер кабинета.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Приёмы до той же даты следующего месяца, не включая её.</returns>
    public Task<IReadOnlyList<AppointmentDto>> GetAppointmentsAsync(
        DateOnly asOfDate,
        string roomNumber,
        CancellationToken cancellationToken);
}
