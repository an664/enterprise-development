using VeterinaryClinic.Contracts.Breeds;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Операции с данными породы.
/// </summary>
public interface IBreedService
{
    /// <summary>
    /// Возвращает все записи.
    /// </summary>
    /// <returns>Результат операции.</returns>
    public IReadOnlyList<BreedDto> GetAll();

    /// <summary>
    /// Возвращает запись по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    public BreedDto GetById(int id);

    /// <summary>
    /// Создаёт запись.
    /// </summary>
    /// <param name="request">Новые данные записи.</param>
    /// <returns>Результат операции.</returns>
    public BreedDto Create(BreedRequest request);

    /// <summary>
    /// Изменяет запись целиком.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="request">Новые данные записи.</param>
    public void Update(int id, BreedRequest request);

    /// <summary>
    /// Удаляет запись, если нет зависимых объектов.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    public void Delete(int id);

    /// <summary>
    /// Питомцы выбранной породы.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    public IReadOnlyList<PetDto> GetPets(int id);

    /// <summary>
    /// Породы выбранного вида.
    /// </summary>
    /// <param name="species">Выбранный вид животных.</param>
    /// <returns>Результат операции.</returns>
    public IReadOnlyList<BreedDto> GetBySpecies(AnimalSpecies species);
}
