using VeterinaryClinic.Models;

namespace VeterinaryClinic.Repositories;

/// <summary>
/// Репозиторий владельца.
/// </summary>
public interface IOwnerRepository : IRepository<Owner>
{
    /// <summary>
    /// Владельцы нескольких питомцев, отсортированные по ФИО.
    /// </summary>
    /// <returns>Результат выборки.</returns>
    public IReadOnlyList<Owner> GetWithMultiplePets();
}
