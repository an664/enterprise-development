using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Infrastructure.InMemory.Repositories;

/// <summary>
/// Репозиторий специализаций в памяти.
/// </summary>
/// <param name="specializations">Записи специализаций.</param>
/// <param name="veterinarians">Записи ветеринаров.</param>
internal class SpecializationRepository(
    Dictionary<int, Specialization> specializations,
    Dictionary<int, Veterinarian> veterinarians) : ISpecializationRepository
{
    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = specializations.Keys.Max() + 1;

    /// <inheritdoc />
    public Task<IReadOnlyList<Specialization>> GetAllAsync(CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Specialization>>([specializations], cancellationToken, () =>
            specializations.Values.OrderBy(record => record.Id).Select(EntitySnapshot.Copy).ToList());
    }

    /// <inheritdoc />
    public Task<Specialization?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<Specialization?>([specializations], cancellationToken, () =>
            specializations.TryGetValue(id, out var record) ? EntitySnapshot.Copy(record) : null);
    }

    /// <inheritdoc />
    public Task<Specialization> AddAsync(Specialization entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([specializations], cancellationToken, () =>
        {
            var stored = EntitySnapshot.Copy(entity);
            stored.Id = _nextId++;
            specializations.Add(stored.Id, stored);
            return EntitySnapshot.Copy(stored);
        });
    }

    /// <inheritdoc />
    public Task<bool> UpdateAsync(Specialization entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([specializations], cancellationToken, () =>
        {
            if (!specializations.TryGetValue(entity.Id, out var current))
            {
                return false;
            }

            current.Name = entity.Name;
            current.Species = entity.Species;
            return true;
        });
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([specializations, veterinarians], cancellationToken, () =>
        {
            if (veterinarians.Values.Any(record => record.Specialization.Id == id))
            {
                throw new EntityInUseException("Запись специализации используется другими записями.");
            }

            return specializations.Remove(id);
        });
    }
}
