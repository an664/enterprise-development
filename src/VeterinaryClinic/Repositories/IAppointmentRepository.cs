using VeterinaryClinic.Models;

namespace VeterinaryClinic.Repositories;

/// <summary>
/// Репозиторий приёма.
/// </summary>
public interface IAppointmentRepository : IRepository<Appointment>
{
    /// <summary>
    /// Приёмы питомца.
    /// </summary>
    /// <param name="petId">Идентификатор питомца.</param>
    /// <returns>Результат выборки.</returns>
    public IReadOnlyList<Appointment> GetByPet(int petId);

    /// <summary>
    /// Приёмы врача.
    /// </summary>
    /// <param name="veterinarianId">Идентификатор ветеринара.</param>
    /// <returns>Результат выборки.</returns>
    public IReadOnlyList<Appointment> GetByVeterinarian(int veterinarianId);

    /// <summary>
    /// Приёмы кабинета в интервале [from, to), отсортированные по времени.
    /// </summary>
    /// <param name="from">Начало периода включительно.</param>
    /// <param name="to">Конец периода исключительно.</param>
    /// <param name="roomNumber">Номер кабинета.</param>
    /// <returns>Результат выборки.</returns>
    public IReadOnlyList<Appointment> GetForPeriod(DateTime from, DateTime to, string roomNumber);

    /// <summary>
    /// Число повторных приёмов выбранной породы.
    /// </summary>
    /// <param name="breedId">Идентификатор породы.</param>
    /// <returns>Количество повторных приёмов.</returns>
    public int CountFollowUpsByBreed(int breedId);
}
