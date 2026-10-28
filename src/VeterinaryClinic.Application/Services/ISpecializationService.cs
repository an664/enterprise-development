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
    /// <returns>Результат операции.</returns>
    public IReadOnlyList<SpecializationDto> GetAll();

    /// <summary>
    /// Возвращает запись по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    public SpecializationDto GetById(int id);

    /// <summary>
    /// Создаёт запись.
    /// </summary>
    /// <param name="request">Новые данные записи.</param>
    /// <returns>Результат операции.</returns>
    public SpecializationDto Create(SpecializationRequest request);

    /// <summary>
    /// Изменяет запись целиком.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="request">Новые данные записи.</param>
    public void Update(int id, SpecializationRequest request);

    /// <summary>
    /// Удаляет запись, если нет зависимых объектов.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    public void Delete(int id);

    /// <summary>
    /// Врачи выбранной специализации.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    public IReadOnlyList<VeterinarianDto> GetVeterinarians(int id);
}
