using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Infrastructure.InMemory.Repositories;

/// <summary>
/// Репозиторий владельцев в памяти.
/// </summary>
/// <param name="owners">Записи владельцев.</param>
/// <param name="pets">Записи питомцев.</param>
internal class OwnerRepository(
    Dictionary<int, Owner> owners,
    Dictionary<int, Pet> pets) : IOwnerRepository
{
    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = owners.Keys.Max() + 1;

    /// <inheritdoc />
    public Task<IReadOnlyList<Owner>> GetAllAsync(CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Owner>>([owners], cancellationToken, () =>
            owners.Values.OrderBy(record => record.Id).Select(EntitySnapshot.Copy).ToList());
    }

    /// <inheritdoc />
    public Task<Owner?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<Owner?>([owners], cancellationToken, () =>
            owners.TryGetValue(id, out var record) ? EntitySnapshot.Copy(record) : null);
    }

    /// <inheritdoc />
    public Task<Owner> AddAsync(Owner entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([owners], cancellationToken, () =>
        {
            var stored = EntitySnapshot.Copy(entity);
            stored.Id = _nextId++;
            owners.Add(stored.Id, stored);
            return EntitySnapshot.Copy(stored);
        });
    }

    /// <inheritdoc />
    public Task<bool> UpdateAsync(Owner entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([owners], cancellationToken, () =>
        {
            if (!owners.TryGetValue(entity.Id, out var current))
            {
                return false;
            }

            current.FullName = entity.FullName;
            current.Address = entity.Address;
            current.Phone = entity.Phone;
            return true;
        });
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([owners, pets], cancellationToken, () =>
        {
            if (pets.Values.Any(record => record.Owner.Id == id))
            {
                throw new EntityInUseException("Запись владельца используется другими записями.");
            }

            return owners.Remove(id);
        });
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Owner>> GetWithMultiplePetsAsync(CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Owner>>([owners, pets], cancellationToken, () =>
            pets.Values
                .GroupBy(pet => pet.Owner.Id)
                .Where(group => group.Count() > 1)
                .Select(group => owners[group.Key])
                .OrderBy(owner => owner.FullName, StringComparer.Ordinal)
                .ThenBy(owner => owner.Id)
                .Select(EntitySnapshot.Copy)
                .ToList());
    }
}
