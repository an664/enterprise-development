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
    /// <returns>Подходящие врачи.</returns>
    public IReadOnlyList<VeterinarianDto> GetVeterinarians(AnimalSpecies species);

    /// <summary>
    /// Возвращает питомцев выбранного врача без повторов, по кличке.
    /// </summary>
    /// <param name="veterinarianId">Идентификатор врача.</param>
    /// <returns>Питомцы врача.</returns>
    public IReadOnlyList<PetDto> GetPets(int veterinarianId);

    /// <summary>
    /// Считает повторные приёмы питомцев выбранной породы.
    /// </summary>
    /// <param name="breedId">Идентификатор породы.</param>
    /// <returns>Количество повторных приёмов.</returns>
    public FollowUpCountDto GetFollowUpCount(int breedId);

    /// <summary>
    /// Возвращает владельцев нескольких питомцев, по ФИО.
    /// </summary>
    /// <returns>Владельцы более одного питомца.</returns>
    public IReadOnlyList<OwnerDto> GetOwnersWithMultiplePets();

    /// <summary>
    /// Возвращает приёмы кабинета за месяц от выбранной даты, по времени.
    /// </summary>
    /// <param name="asOfDate">Начальная дата, включаемая в выборку.</param>
    /// <param name="roomNumber">Номер кабинета.</param>
    /// <returns>Приёмы до той же даты следующего месяца, не включая её.</returns>
    public IReadOnlyList<AppointmentDto> GetAppointments(DateOnly asOfDate, string roomNumber);
}
