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
/// <param name="locks">Блокировки коллекций хранилища.</param>
internal class PetRepository(
    Dictionary<int, Owner> owners,
    Dictionary<int, Breed> breeds,
    Dictionary<int, Pet> pets,
    Dictionary<int, Appointment> appointments,
    TableLocks locks) : IPetRepository
{
    /// <summary>
    /// Блокировки коллекций, из которых копируются данные записи.
    /// </summary>
    private readonly Lock[] _snapshotLocks = [locks.Owners, locks.Breeds, locks.Pets];

    /// <summary>
    /// Блокировки для операций, которым нужны и данные питомцев, и их приёмы.
    /// </summary>
    private readonly Lock[] _petAndAppointmentLocks = [locks.Owners, locks.Breeds, locks.Pets, locks.Appointments];

    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = pets.Keys.Max() + 1;

    /// <inheritdoc />
    public Task<IReadOnlyList<Pet>> GetAllAsync(CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Pet>>(_snapshotLocks, cancellationToken, () =>
            pets.Values.OrderBy(record => record.Id).Select(EntitySnapshot.Copy).ToList());
    }

    /// <inheritdoc />
    public Task<Pet?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<Pet?>(_snapshotLocks, cancellationToken, () =>
            pets.TryGetValue(id, out var record) ? EntitySnapshot.Copy(record) : null);
    }

    /// <inheritdoc />
    public Task<Pet> AddAsync(Pet entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync(_snapshotLocks, cancellationToken, () =>
        {
            var (breed, owner) = GetBreedAndOwner(entity);
            var stored = new Pet
            {
                Id = _nextId++,
                Name = entity.Name,
                Breed = breed,
                Owner = owner,
                BirthDate = entity.BirthDate,
                Weight = entity.Weight
            };
            pets.Add(stored.Id, stored);
            return EntitySnapshot.Copy(stored);
        });
    }

    /// <inheritdoc />
    public Task<bool> UpdateAsync(Pet entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync(_petAndAppointmentLocks, cancellationToken, () =>
        {
            if (!pets.TryGetValue(entity.Id, out var current))
            {
                return false;
            }

            var (breed, owner) = GetBreedAndOwner(entity);
            if (entity.BirthDate > current.BirthDate
                && appointments.Values.Any(visit => visit.Pet.Id == entity.Id
                    && DateOnly.FromDateTime(visit.StartsAt) < entity.BirthDate))
            {
                throw new ValidationException("Дата рождения не может быть позже существующего приёма питомца.");
            }

            current.Name = entity.Name;
            current.Breed = breed;
            current.Owner = owner;
            current.BirthDate = entity.BirthDate;
            current.Weight = entity.Weight;
            return true;
        });
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([locks.Pets, locks.Appointments], cancellationToken, () =>
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
        return TableLocks.RunAsync<IReadOnlyList<Pet>>(_snapshotLocks, cancellationToken, () =>
            pets.Values
                .Where(pet => pet.Owner.Id == ownerId)
                .OrderBy(record => record.Id)
                .Select(EntitySnapshot.Copy)
                .ToList());
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Pet>> GetByBreedAsync(int breedId, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Pet>>(_snapshotLocks, cancellationToken, () =>
            pets.Values
                .Where(pet => pet.Breed.Id == breedId)
                .OrderBy(record => record.Id)
                .Select(EntitySnapshot.Copy)
                .ToList());
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Pet>> GetByVeterinarianAsync(int veterinarianId, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Pet>>(_petAndAppointmentLocks, cancellationToken, () =>
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
    /// Находит породу и владельца питомца в хранилище.
    /// </summary>
    private (Breed Breed, Owner Owner) GetBreedAndOwner(Pet entity)
    {
        var breed = breeds.GetValueOrDefault(entity.Breed.Id)
            ?? throw new EntityNotFoundException("Связанная запись породы не найдена.");
        var owner = owners.GetValueOrDefault(entity.Owner.Id)
            ?? throw new EntityNotFoundException("Связанная запись владельца не найдена.");
        return (breed, owner);
    }
}
