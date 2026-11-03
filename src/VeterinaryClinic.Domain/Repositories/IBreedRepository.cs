using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Domain.Repositories;

/// <summary>
/// Репозиторий породы.
/// </summary>
public interface IBreedRepository : IRepository<Breed>
{
    /// <summary>
    /// Породы выбранного вида животных.
    /// </summary>
    /// <param name="species">Выбранный вид животных.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат выборки.</returns>
    public Task<IReadOnlyList<Breed>> GetBySpeciesAsync(AnimalSpecies species, CancellationToken cancellationToken);
}
