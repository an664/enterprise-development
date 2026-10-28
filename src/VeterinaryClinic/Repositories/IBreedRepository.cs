using VeterinaryClinic.Models;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Repositories;

/// <summary>
/// Репозиторий породы.
/// </summary>
public interface IBreedRepository : IRepository<Breed>
{
    /// <summary>
    /// Породы выбранного вида животных.
    /// </summary>
    /// <param name="species">Выбранный вид животных.</param>
    /// <returns>Результат выборки.</returns>
    public IReadOnlyList<Breed> GetBySpecies(AnimalSpecies species);
}
