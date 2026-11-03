using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Domain.Repositories;

/// <summary>
/// Репозиторий ветеринара.
/// </summary>
public interface IVeterinarianRepository : IRepository<Veterinarian>
{
    /// <summary>
    /// Врачи выбранной специализации.
    /// </summary>
    /// <param name="specializationId">Идентификатор специализации.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат выборки.</returns>
    public Task<IReadOnlyList<Veterinarian>> GetBySpecializationAsync(
        int specializationId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Врачи, специализирующиеся на выбранном виде.
    /// </summary>
    /// <param name="species">Выбранный вид животных.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат выборки.</returns>
    public Task<IReadOnlyList<Veterinarian>> GetBySpeciesAsync(
        AnimalSpecies species,
        CancellationToken cancellationToken);
}
