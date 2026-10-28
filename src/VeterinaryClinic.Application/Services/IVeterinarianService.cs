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
    /// <returns>Результат операции.</returns>
    public IReadOnlyList<VeterinarianDto> GetAll();

    /// <summary>
    /// Возвращает запись по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    public VeterinarianDto GetById(int id);

    /// <summary>
    /// Создаёт запись.
    /// </summary>
    /// <param name="request">Новые данные записи.</param>
    /// <returns>Результат операции.</returns>
    public VeterinarianDto Create(VeterinarianRequest request);

    /// <summary>
    /// Изменяет запись целиком.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="request">Новые данные записи.</param>
    public void Update(int id, VeterinarianRequest request);

    /// <summary>
    /// Удаляет запись, если нет зависимых объектов.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    public void Delete(int id);

    /// <summary>
    /// Приёмы ветеринара.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    public IReadOnlyList<AppointmentDto> GetAppointments(int id);

    /// <summary>
    /// Специализация ветеринара.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    public SpecializationDto GetSpecialization(int id);
}
