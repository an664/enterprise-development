using VeterinaryClinic.Data;
using VeterinaryClinic.Models;
using VeterinaryClinic.Repositories;

namespace VeterinaryClinic.Infrastructure.Repositories;

/// <summary>
/// Репозиторий специализаций в памяти.
/// </summary>
/// <param name="store">Общее хранилище сущностей.</param>
/// <param name="synchronization">Общая блокировка операций.</param>
public class SpecializationRepository(ClinicStore store, ClinicLock synchronization) : ISpecializationRepository
{
    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = store.Specializations.Keys.Max() + 1;

    /// <inheritdoc />
    public IReadOnlyList<Specialization> GetAll()
    {
        lock (synchronization.SyncRoot)
        {
            return [.. store.Specializations.Values.OrderBy(entity => entity.Id)];
        }
    }

    /// <inheritdoc />
    public Specialization? GetById(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return store.Specializations.GetValueOrDefault(id);
        }
    }

    /// <inheritdoc />
    public Specialization Add(Specialization entity)
    {
        lock (synchronization.SyncRoot)
        {
            entity.Id = _nextId++;
            store.Specializations.Add(entity.Id, entity);
            return entity;
        }
    }

    /// <inheritdoc />
    public bool Update(Specialization entity)
    {
        lock (synchronization.SyncRoot)
        {
            if (!store.Specializations.TryGetValue(entity.Id, out var current))
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
            return store.Specializations.Remove(id);
        }
    }
}
