using VeterinaryClinic.Data;
using VeterinaryClinic.Models;
using VeterinaryClinic.Repositories;

namespace VeterinaryClinic.Infrastructure.Repositories;

/// <summary>
/// Репозиторий владельцев в памяти.
/// </summary>
/// <param name="store">Общее хранилище сущностей.</param>
/// <param name="synchronization">Общая блокировка операций.</param>
public class OwnerRepository(ClinicStore store, ClinicLock synchronization) : IOwnerRepository
{
    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = store.Owners.Keys.Max() + 1;

    /// <inheritdoc />
    public IReadOnlyList<Owner> GetAll()
    {
        lock (synchronization.SyncRoot)
        {
            return [.. store.Owners.Values.OrderBy(entity => entity.Id)];
        }
    }

    /// <inheritdoc />
    public Owner? GetById(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return store.Owners.GetValueOrDefault(id);
        }
    }

    /// <inheritdoc />
    public Owner Add(Owner entity)
    {
        lock (synchronization.SyncRoot)
        {
            entity.Id = _nextId++;
            store.Owners.Add(entity.Id, entity);
            return entity;
        }
    }

    /// <inheritdoc />
    public bool Update(Owner entity)
    {
        lock (synchronization.SyncRoot)
        {
            if (!store.Owners.TryGetValue(entity.Id, out var current))
            {
                return false;
            }

            current.FullName = entity.FullName;
            current.Address = entity.Address;
            current.Phone = entity.Phone;
            return true;
        }
    }

    /// <inheritdoc />
    public bool Delete(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return store.Owners.Remove(id);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Owner> GetWithMultiplePets()
    {
        lock (synchronization.SyncRoot)
        {
            return
            [
                .. store.Pets.Values
                    .GroupBy(pet => pet.Owner.Id)
                    .Where(group => group.Count() > 1)
                    .Select(group => store.Owners[group.Key])
                    .OrderBy(owner => owner.FullName, StringComparer.Ordinal)
                    .ThenBy(owner => owner.Id)
            ];
        }
    }
}
