using VeterinaryClinic.Models;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Repositories;

/// <summary>
/// Репозиторий ветеринара.
/// </summary>
public interface IVeterinarianRepository : IRepository<Veterinarian>
{
    /// <summary>
    /// Врачи выбранной специализации.
    /// </summary>
    /// <param name="specializationId">Идентификатор специализации.</param>
    /// <returns>Результат выборки.</returns>
    public IReadOnlyList<Veterinarian> GetBySpecialization(int specializationId);

    /// <summary>
    /// Врачи, специализирующиеся на выбранном виде.
    /// </summary>
    /// <param name="species">Выбранный вид животных.</param>
    /// <returns>Результат выборки.</returns>
    public IReadOnlyList<Veterinarian> GetBySpecies(AnimalSpecies species);
}
