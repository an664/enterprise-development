using VeterinaryClinic.Data;
using VeterinaryClinic.Models;
using VeterinaryClinic.Repositories;

namespace VeterinaryClinic.Infrastructure.Repositories;

/// <summary>
/// Репозиторий питомцев в памяти.
/// </summary>
/// <param name="store">Общее хранилище сущностей.</param>
/// <param name="synchronization">Общая блокировка операций.</param>
public class PetRepository(ClinicStore store, ClinicLock synchronization) : IPetRepository
{
    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = store.Pets.Keys.Max() + 1;

    /// <inheritdoc />
    public IReadOnlyList<Pet> GetAll()
    {
        lock (synchronization.SyncRoot)
        {
            return [.. store.Pets.Values.OrderBy(entity => entity.Id)];
        }
    }

    /// <inheritdoc />
    public Pet? GetById(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return store.Pets.GetValueOrDefault(id);
        }
    }

    /// <inheritdoc />
    public Pet Add(Pet entity)
    {
        lock (synchronization.SyncRoot)
        {
            entity.Id = _nextId++;
            store.Pets.Add(entity.Id, entity);
            return entity;
        }
    }

    /// <inheritdoc />
    public bool Update(Pet entity)
    {
        lock (synchronization.SyncRoot)
        {
            if (!store.Pets.TryGetValue(entity.Id, out var current))
            {
                return false;
            }

            current.Name = entity.Name;
            current.Breed = entity.Breed;
            current.Owner = entity.Owner;
            current.BirthDate = entity.BirthDate;
            current.Weight = entity.Weight;
            return true;
        }
    }

    /// <inheritdoc />
    public bool Delete(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return store.Pets.Remove(id);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Pet> GetByOwner(int ownerId)
    {
        lock (synchronization.SyncRoot)
        {
            return
            [
                .. store.Pets.Values
                    .Where(pet => pet.Owner.Id == ownerId)
                    .OrderBy(pet => pet.Id)
            ];
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Pet> GetByBreed(int breedId)
    {
        lock (synchronization.SyncRoot)
        {
            return
            [
                .. store.Pets.Values
                    .Where(pet => pet.Breed.Id == breedId)
                    .OrderBy(pet => pet.Id)
            ];
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Pet> GetByVeterinarian(int veterinarianId)
    {
        lock (synchronization.SyncRoot)
        {
            return
            [
                .. store.Appointments.Values
                    .Where(appointment => appointment.Veterinarian.Id == veterinarianId)
                    .Select(appointment => appointment.Pet)
                    .DistinctBy(pet => pet.Id)
                    .OrderBy(pet => pet.Name, StringComparer.Ordinal)
                    .ThenBy(pet => pet.Id)
            ];
        }
    }
}
