using VeterinaryClinic.Data;
using VeterinaryClinic.Models;
using VeterinaryClinic.Repositories;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Infrastructure.Repositories;

/// <summary>
/// Репозиторий пород в памяти.
/// </summary>
/// <param name="store">Общее хранилище сущностей.</param>
/// <param name="synchronization">Общая блокировка операций.</param>
public class BreedRepository(ClinicStore store, ClinicLock synchronization) : IBreedRepository
{
    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = store.Breeds.Keys.Max() + 1;

    /// <inheritdoc />
    public IReadOnlyList<Breed> GetAll()
    {
        lock (synchronization.SyncRoot)
        {
            return [.. store.Breeds.Values.OrderBy(entity => entity.Id)];
        }
    }

    /// <inheritdoc />
    public Breed? GetById(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return store.Breeds.GetValueOrDefault(id);
        }
    }

    /// <inheritdoc />
    public Breed Add(Breed entity)
    {
        lock (synchronization.SyncRoot)
        {
            entity.Id = _nextId++;
            store.Breeds.Add(entity.Id, entity);
            return entity;
        }
    }

    /// <inheritdoc />
    public bool Update(Breed entity)
    {
        lock (synchronization.SyncRoot)
        {
            if (!store.Breeds.TryGetValue(entity.Id, out var current))
            {
                return false;
            }

            current.Name = entity.Name;
            current.Species = entity.Species;
            return true;
        }
    }

    /// <inheritdoc />
    public bool Delete(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return store.Breeds.Remove(id);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Breed> GetBySpecies(AnimalSpecies species)
    {
        lock (synchronization.SyncRoot)
        {
            return
            [
                .. store.Breeds.Values
                    .Where(breed => breed.Species == species)
                    .OrderBy(breed => breed.Id)
            ];
        }
    }
}
