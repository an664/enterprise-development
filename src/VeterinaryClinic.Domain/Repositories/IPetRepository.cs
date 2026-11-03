using VeterinaryClinic.Domain.Models;

namespace VeterinaryClinic.Domain.Repositories;

/// <summary>
/// Репозиторий питомца.
/// </summary>
public interface IPetRepository : IRepository<Pet>
{
    /// <summary>
    /// Питомцы владельца.
    /// </summary>
    /// <param name="ownerId">Идентификатор владельца.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат выборки.</returns>
    public Task<IReadOnlyList<Pet>> GetByOwnerAsync(int ownerId, CancellationToken cancellationToken);

    /// <summary>
    /// Питомцы выбранной породы.
    /// </summary>
    /// <param name="breedId">Идентификатор породы.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат выборки.</returns>
    public Task<IReadOnlyList<Pet>> GetByBreedAsync(int breedId, CancellationToken cancellationToken);

    /// <summary>
    /// Питомцы врача без повторов, отсортированные по кличке.
    /// </summary>
    /// <param name="veterinarianId">Идентификатор ветеринара.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат выборки.</returns>
    public Task<IReadOnlyList<Pet>> GetByVeterinarianAsync(int veterinarianId, CancellationToken cancellationToken);
}
