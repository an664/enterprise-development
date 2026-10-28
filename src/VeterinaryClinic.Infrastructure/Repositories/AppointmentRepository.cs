using VeterinaryClinic.Data;
using VeterinaryClinic.Models;
using VeterinaryClinic.Repositories;

namespace VeterinaryClinic.Infrastructure.Repositories;

/// <summary>
/// Репозиторий приёмов в памяти.
/// </summary>
/// <param name="store">Общее хранилище сущностей.</param>
/// <param name="synchronization">Общая блокировка операций.</param>
public class AppointmentRepository(ClinicStore store, ClinicLock synchronization) : IAppointmentRepository
{
    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = store.Appointments.Keys.Max() + 1;

    /// <inheritdoc />
    public IReadOnlyList<Appointment> GetAll()
    {
        lock (synchronization.SyncRoot)
        {
            return [.. store.Appointments.Values.OrderBy(entity => entity.Id)];
        }
    }

    /// <inheritdoc />
    public Appointment? GetById(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return store.Appointments.GetValueOrDefault(id);
        }
    }

    /// <inheritdoc />
    public Appointment Add(Appointment entity)
    {
        lock (synchronization.SyncRoot)
        {
            entity.Id = _nextId++;
            store.Appointments.Add(entity.Id, entity);
            return entity;
        }
    }

    /// <inheritdoc />
    public bool Update(Appointment entity)
    {
        lock (synchronization.SyncRoot)
        {
            if (!store.Appointments.TryGetValue(entity.Id, out var current))
            {
                return false;
            }

            current.Pet = entity.Pet;
            current.Veterinarian = entity.Veterinarian;
            current.StartsAt = entity.StartsAt;
            current.RoomNumber = entity.RoomNumber;
            current.IsFollowUp = entity.IsFollowUp;
            return true;
        }
    }

    /// <inheritdoc />
    public bool Delete(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return store.Appointments.Remove(id);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Appointment> GetByPet(int petId)
    {
        lock (synchronization.SyncRoot)
        {
            return
            [
                .. store.Appointments.Values
                    .Where(appointment => appointment.Pet.Id == petId)
                    .OrderBy(appointment => appointment.Id)
            ];
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Appointment> GetByVeterinarian(int veterinarianId)
    {
        lock (synchronization.SyncRoot)
        {
            return
            [
                .. store.Appointments.Values
                    .Where(appointment => appointment.Veterinarian.Id == veterinarianId)
                    .OrderBy(appointment => appointment.Id)
            ];
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Appointment> GetForPeriod(DateTime from, DateTime to, string roomNumber)
    {
        lock (synchronization.SyncRoot)
        {
            return
            [
                .. store.Appointments.Values
                    .Where(appointment => appointment.StartsAt >= from
                        && appointment.StartsAt < to
                        && appointment.RoomNumber == roomNumber)
                    .OrderBy(appointment => appointment.StartsAt)
                    .ThenBy(appointment => appointment.Id)
            ];
        }
    }

    /// <inheritdoc />
    public int CountFollowUpsByBreed(int breedId)
    {
        lock (synchronization.SyncRoot)
        {
            return store.Appointments.Values.Count(appointment =>
                appointment.IsFollowUp && appointment.Pet.Breed.Id == breedId);
        }
    }
}
