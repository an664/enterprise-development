using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Infrastructure.InMemory.Repositories;

/// <summary>
/// Репозиторий владельцев в памяти.
/// </summary>
/// <param name="owners">Записи владельцев.</param>
/// <param name="pets">Записи питомцев.</param>
/// <param name="locks">Блокировки коллекций хранилища.</param>
internal class OwnerRepository(
    Dictionary<int, Owner> owners,
    Dictionary<int, Pet> pets,
    TableLocks locks) : IOwnerRepository
{
    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = owners.Keys.Max() + 1;

    /// <inheritdoc />
    public Task<IReadOnlyList<Owner>> GetAllAsync(CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Owner>>([locks.Owners], cancellationToken, () =>
            owners.Values.OrderBy(record => record.Id).Select(EntitySnapshot.Copy).ToList());
    }

    /// <inheritdoc />
    public Task<Owner?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<Owner?>([locks.Owners], cancellationToken, () =>
            owners.TryGetValue(id, out var record) ? EntitySnapshot.Copy(record) : null);
    }

    /// <inheritdoc />
    public Task<Owner> AddAsync(Owner entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([locks.Owners], cancellationToken, () =>
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
        return TableLocks.RunAsync([locks.Owners], cancellationToken, () =>
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
        return TableLocks.RunAsync([locks.Owners, locks.Pets], cancellationToken, () =>
        {
            if (pets.Values.Any(record => record.Owner.Id == id))
            {
                throw new EntityInUseException("Запись владельца используется другими записями.");
            }

            return owners.Remove(id);
        });
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<OwnerWithPetCount>> GetWithMultiplePetsAsync(CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<OwnerWithPetCount>>(
            [locks.Owners, locks.Pets],
            cancellationToken, () =>
            pets.Values
                .GroupBy(pet => pet.Owner.Id)
                .Where(group => group.Count() > 1)
                .Select(group => new OwnerWithPetCount(EntitySnapshot.Copy(owners[group.Key]), group.Count()))
                .OrderBy(result => result.Owner.FullName, StringComparer.Ordinal)
                .ThenBy(result => result.Owner.Id)
                .ToList());
    }
}
