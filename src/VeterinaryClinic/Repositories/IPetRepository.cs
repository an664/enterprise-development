using VeterinaryClinic.Models;

namespace VeterinaryClinic.Repositories;

/// <summary>
/// Репозиторий питомца.
/// </summary>
public interface IPetRepository : IRepository<Pet>
{
    /// <summary>
    /// Питомцы владельца.
    /// </summary>
    /// <param name="ownerId">Идентификатор владельца.</param>
    /// <returns>Результат выборки.</returns>
    public IReadOnlyList<Pet> GetByOwner(int ownerId);

    /// <summary>
    /// Питомцы выбранной породы.
    /// </summary>
    /// <param name="breedId">Идентификатор породы.</param>
    /// <returns>Результат выборки.</returns>
    public IReadOnlyList<Pet> GetByBreed(int breedId);

    /// <summary>
    /// Питомцы врача без повторов, отсортированные по кличке.
    /// </summary>
    /// <param name="veterinarianId">Идентификатор ветеринара.</param>
    /// <returns>Результат выборки.</returns>
    public IReadOnlyList<Pet> GetByVeterinarian(int veterinarianId);
}
