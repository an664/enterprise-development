using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Infrastructure.InMemory.Repositories;

/// <summary>
/// Репозиторий пород в памяти.
/// </summary>
/// <param name="breeds">Записи пород.</param>
/// <param name="pets">Записи питомцев.</param>
internal class BreedRepository(
    Dictionary<int, Breed> breeds,
    Dictionary<int, Pet> pets) : IBreedRepository
{
    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = breeds.Keys.Max() + 1;

    /// <inheritdoc />
    public Task<IReadOnlyList<Breed>> GetAllAsync(CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Breed>>([breeds], cancellationToken, () =>
            breeds.Values.OrderBy(record => record.Id).Select(EntitySnapshot.Copy).ToList());
    }

    /// <inheritdoc />
    public Task<Breed?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<Breed?>([breeds], cancellationToken, () =>
            breeds.TryGetValue(id, out var record) ? EntitySnapshot.Copy(record) : null);
    }

    /// <inheritdoc />
    public Task<Breed> AddAsync(Breed entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([breeds], cancellationToken, () =>
        {
            var stored = EntitySnapshot.Copy(entity);
            stored.Id = _nextId++;
            breeds.Add(stored.Id, stored);
            return EntitySnapshot.Copy(stored);
        });
    }

    /// <inheritdoc />
    public Task<bool> UpdateAsync(Breed entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([breeds], cancellationToken, () =>
        {
            if (!breeds.TryGetValue(entity.Id, out var current))
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
        return TableLocks.RunAsync([breeds, pets], cancellationToken, () =>
        {
            if (pets.Values.Any(record => record.Breed.Id == id))
            {
                throw new EntityInUseException("Запись породы используется другими записями.");
            }

            return breeds.Remove(id);
        });
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Breed>> GetBySpeciesAsync(AnimalSpecies species, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Breed>>([breeds], cancellationToken, () =>
            breeds.Values
                .Where(breed => breed.Species == species)
                .OrderBy(record => record.Id)
                .Select(EntitySnapshot.Copy)
                .ToList());
    }
}
