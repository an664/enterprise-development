using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Breeds;
using VeterinaryClinic.Contracts.Owners;
using VeterinaryClinic.Contracts.Pets;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Операции с данными питомца.
/// </summary>
public interface IPetService
{
    /// <summary>
    /// Возвращает все записи.
    /// </summary>
    /// <returns>Результат операции.</returns>
    public IReadOnlyList<PetDto> GetAll();

    /// <summary>
    /// Возвращает запись по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    public PetDto GetById(int id);

    /// <summary>
    /// Создаёт запись.
    /// </summary>
    /// <param name="request">Новые данные записи.</param>
    /// <returns>Результат операции.</returns>
    public PetDto Create(PetRequest request);

    /// <summary>
    /// Изменяет запись целиком.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="request">Новые данные записи.</param>
    public void Update(int id, PetRequest request);

    /// <summary>
    /// Удаляет запись, если нет зависимых объектов.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    public void Delete(int id);

    /// <summary>
    /// Приёмы питомца.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    public IReadOnlyList<AppointmentDto> GetAppointments(int id);

    /// <summary>
    /// Владелец питомца.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    public OwnerDto GetOwner(int id);

    /// <summary>
    /// Порода питомца.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    public BreedDto GetBreed(int id);
}
