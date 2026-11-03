using System.ComponentModel.DataAnnotations;
using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Infrastructure.InMemory.Repositories;

/// <summary>
/// Репозиторий питомцев в памяти.
/// </summary>
/// <param name="owners">Записи владельцев.</param>
/// <param name="breeds">Записи пород.</param>
/// <param name="pets">Записи питомцев.</param>
/// <param name="appointments">Записи приёмов.</param>
internal class PetRepository(
    Dictionary<int, Owner> owners,
    Dictionary<int, Breed> breeds,
    Dictionary<int, Pet> pets,
    Dictionary<int, Appointment> appointments) : IPetRepository
{
    /// <summary>
    /// Коллекции, из которых берутся данные для полной копии записи.
    /// </summary>
    private readonly object[] _snapshotTables = [owners, breeds, pets];

    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = pets.Keys.Max() + 1;

    /// <inheritdoc />
    public Task<IReadOnlyList<Pet>> GetAllAsync(CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Pet>>(_snapshotTables, cancellationToken, () =>
            pets.Values.OrderBy(record => record.Id).Select(EntitySnapshot.Copy).ToList());
    }

    /// <inheritdoc />
    public Task<Pet?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<Pet?>(_snapshotTables, cancellationToken, () =>
            pets.TryGetValue(id, out var record) ? EntitySnapshot.Copy(record) : null);
    }

    /// <inheritdoc />
    public Task<Pet> AddAsync(Pet entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync(_snapshotTables, cancellationToken, () =>
        {
            var stored = EntitySnapshot.Copy(entity);
            BindReferences(stored);
            stored.Id = _nextId++;
            pets.Add(stored.Id, stored);
            return EntitySnapshot.Copy(stored);
        });
    }

    /// <inheritdoc />
    public Task<bool> UpdateAsync(Pet entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([owners, breeds, pets, appointments], cancellationToken, () =>
        {
            if (!pets.TryGetValue(entity.Id, out var current))
            {
                return false;
            }

            var updated = EntitySnapshot.Copy(entity);
            BindReferences(updated);
            if (updated.BirthDate > current.BirthDate
                && appointments.Values.Any(visit => visit.Pet.Id == updated.Id
                    && DateOnly.FromDateTime(visit.StartsAt) < updated.BirthDate))
            {
                throw new ValidationException("Дата рождения не может быть позже существующего приёма питомца.");
            }

            current.Name = updated.Name;
            current.Breed = updated.Breed;
            current.Owner = updated.Owner;
            current.BirthDate = updated.BirthDate;
            current.Weight = updated.Weight;
            return true;
        });
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([pets, appointments], cancellationToken, () =>
        {
            if (appointments.Values.Any(record => record.Pet.Id == id))
            {
                throw new EntityInUseException("Запись питомца используется другими записями.");
            }

            return pets.Remove(id);
        });
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Pet>> GetByOwnerAsync(int ownerId, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Pet>>(_snapshotTables, cancellationToken, () =>
            pets.Values
                .Where(pet => pet.Owner.Id == ownerId)
                .OrderBy(record => record.Id)
                .Select(EntitySnapshot.Copy)
                .ToList());
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Pet>> GetByBreedAsync(int breedId, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Pet>>(_snapshotTables, cancellationToken, () =>
            pets.Values
                .Where(pet => pet.Breed.Id == breedId)
                .OrderBy(record => record.Id)
                .Select(EntitySnapshot.Copy)
                .ToList());
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Pet>> GetByVeterinarianAsync(int veterinarianId, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Pet>>([owners, breeds, pets, appointments], cancellationToken, () =>
            appointments.Values
                .Where(visit => visit.Veterinarian.Id == veterinarianId)
                .Select(visit => visit.Pet)
                .DistinctBy(pet => pet.Id)
                .OrderBy(pet => pet.Name, StringComparer.Ordinal)
                .ThenBy(pet => pet.Id)
                .Select(EntitySnapshot.Copy)
                .ToList());
    }

    /// <summary>
    /// Связывает запись с актуальными объектами, пока их нельзя удалить или изменить.
    /// </summary>
    private void BindReferences(Pet entity)
    {
        entity.Breed = breeds.GetValueOrDefault(entity.Breed.Id)
            ?? throw new EntityNotFoundException("Связанная запись породы не найдена.");
        entity.Owner = owners.GetValueOrDefault(entity.Owner.Id)
            ?? throw new EntityNotFoundException("Связанная запись владельца не найдена.");
    }
}
