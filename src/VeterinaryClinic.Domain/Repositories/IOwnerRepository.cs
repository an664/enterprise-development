using VeterinaryClinic.Domain.Models;

namespace VeterinaryClinic.Domain.Repositories;

/// <summary>
/// Репозиторий владельца.
/// </summary>
public interface IOwnerRepository : IRepository<Owner>
{
    /// <summary>
    /// Владельцы нескольких питомцев, отсортированные по ФИО.
    /// </summary>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат выборки.</returns>
    public Task<IReadOnlyList<Owner>> GetWithMultiplePetsAsync(CancellationToken cancellationToken);
}
