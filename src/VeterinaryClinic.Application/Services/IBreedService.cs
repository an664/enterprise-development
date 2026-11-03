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
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<IReadOnlyList<BreedDto>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает запись по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<BreedDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Создаёт запись.
    /// </summary>
    /// <param name="request">Новые данные записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<BreedDto> CreateAsync(BreedRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Изменяет запись целиком.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="request">Новые данные записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    public Task UpdateAsync(int id, BreedRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет запись, если нет зависимых объектов.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    public Task DeleteAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Питомцы выбранной породы.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<IReadOnlyList<PetDto>> GetPetsAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Породы выбранного вида.
    /// </summary>
    /// <param name="species">Выбранный вид животных.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    public Task<IReadOnlyList<BreedDto>> GetBySpeciesAsync(
        AnimalSpecies species,
        CancellationToken cancellationToken);
}
