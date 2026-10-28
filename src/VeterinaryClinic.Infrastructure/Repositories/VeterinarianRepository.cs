using VeterinaryClinic.Data;
using VeterinaryClinic.Models;
using VeterinaryClinic.Repositories;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Infrastructure.Repositories;

/// <summary>
/// Репозиторий ветеринаров в памяти.
/// </summary>
/// <param name="store">Общее хранилище сущностей.</param>
/// <param name="synchronization">Общая блокировка операций.</param>
public class VeterinarianRepository(ClinicStore store, ClinicLock synchronization) : IVeterinarianRepository
{
    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = store.Veterinarians.Keys.Max() + 1;

    /// <inheritdoc />
    public IReadOnlyList<Veterinarian> GetAll()
    {
        lock (synchronization.SyncRoot)
        {
            return [.. store.Veterinarians.Values.OrderBy(entity => entity.Id)];
        }
    }

    /// <inheritdoc />
    public Veterinarian? GetById(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return store.Veterinarians.GetValueOrDefault(id);
        }
    }

    /// <inheritdoc />
    public Veterinarian Add(Veterinarian entity)
    {
        lock (synchronization.SyncRoot)
        {
            entity.Id = _nextId++;
            store.Veterinarians.Add(entity.Id, entity);
            return entity;
        }
    }

    /// <inheritdoc />
    public bool Update(Veterinarian entity)
    {
        lock (synchronization.SyncRoot)
        {
            if (!store.Veterinarians.TryGetValue(entity.Id, out var current))
            {
                return false;
            }

            current.PassportNumber = entity.PassportNumber;
            current.FullName = entity.FullName;
            current.BirthYear = entity.BirthYear;
            current.Specialization = entity.Specialization;
            current.ExperienceYears = entity.ExperienceYears;
            return true;
        }
    }

    /// <inheritdoc />
    public bool Delete(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return store.Veterinarians.Remove(id);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Veterinarian> GetBySpecialization(int specializationId)
    {
        lock (synchronization.SyncRoot)
        {
            return
            [
                .. store.Veterinarians.Values
                    .Where(veterinarian => veterinarian.Specialization.Id == specializationId)
                    .OrderBy(veterinarian => veterinarian.Id)
            ];
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Veterinarian> GetBySpecies(AnimalSpecies species)
    {
        lock (synchronization.SyncRoot)
        {
            return
            [
                .. store.Veterinarians.Values
                    .Where(veterinarian => veterinarian.Specialization.Species == species)
                    .OrderBy(veterinarian => veterinarian.Id)
            ];
        }
    }
}
