using System.ComponentModel.DataAnnotations;
using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Infrastructure.InMemory.Repositories;

/// <summary>
/// Репозиторий приёмов в памяти.
/// </summary>
/// <param name="veterinarians">Записи ветеринаров.</param>
/// <param name="pets">Записи питомцев.</param>
/// <param name="appointments">Записи приёмов.</param>
/// <param name="locks">Блокировки коллекций хранилища.</param>
internal class AppointmentRepository(
    Dictionary<int, Veterinarian> veterinarians,
    Dictionary<int, Pet> pets,
    Dictionary<int, Appointment> appointments,
    TableLocks locks) : IAppointmentRepository
{
    /// <summary>
    /// Блокировки коллекций, из которых копируются данные записи.
    /// </summary>
    private readonly Lock[] _snapshotLocks =
    [
        locks.Owners, locks.Breeds, locks.Specializations,
        locks.Veterinarians, locks.Pets, locks.Appointments
    ];

    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = appointments.Keys.Max() + 1;

    /// <inheritdoc />
    public Task<IReadOnlyList<Appointment>> GetAllAsync(CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Appointment>>(_snapshotLocks, cancellationToken, () =>
            appointments.Values.OrderBy(record => record.Id).Select(EntitySnapshot.Copy).ToList());
    }

    /// <inheritdoc />
    public Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<Appointment?>(_snapshotLocks, cancellationToken, () =>
            appointments.TryGetValue(id, out var record) ? EntitySnapshot.Copy(record) : null);
    }

    /// <inheritdoc />
    public Task<Appointment> AddAsync(Appointment entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync(_snapshotLocks, cancellationToken, () =>
        {
            var (pet, veterinarian) = ValidateAppointment(entity);
            var stored = new Appointment
            {
                Id = _nextId++,
                Pet = pet,
                Veterinarian = veterinarian,
                StartsAt = entity.StartsAt,
                RoomNumber = entity.RoomNumber,
                IsFollowUp = entity.IsFollowUp
            };
            appointments.Add(stored.Id, stored);
            return EntitySnapshot.Copy(stored);
        });
    }

    /// <inheritdoc />
    public Task<bool> UpdateAsync(Appointment entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([locks.Veterinarians, locks.Pets, locks.Appointments], cancellationToken, () =>
        {
            if (!appointments.TryGetValue(entity.Id, out var current))
            {
                return false;
            }

            var (pet, veterinarian) = ValidateAppointment(entity, current.Id);
            current.Pet = pet;
            current.Veterinarian = veterinarian;
            current.StartsAt = entity.StartsAt;
            current.RoomNumber = entity.RoomNumber;
            current.IsFollowUp = entity.IsFollowUp;
            return true;
        });
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([locks.Appointments], cancellationToken, () =>
            appointments.Remove(id));
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Appointment>> GetByPetAsync(int petId, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Appointment>>(_snapshotLocks, cancellationToken, () =>
            appointments.Values
                .Where(visit => visit.Pet.Id == petId)
                .OrderBy(record => record.Id)
                .Select(EntitySnapshot.Copy)
                .ToList());
    }

    /// <inheritdoc />
    public Task<bool> HasAppointmentBeforeAsync(int petId, DateOnly date, CancellationToken cancellationToken)
    {
        var startOfDay = date.ToDateTime(TimeOnly.MinValue);
        return TableLocks.RunAsync([locks.Appointments], cancellationToken, () =>
            appointments.Values.Any(visit => visit.Pet.Id == petId && visit.StartsAt < startOfDay));
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Appointment>> GetByVeterinarianAsync(
        int veterinarianId,
        CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Appointment>>(_snapshotLocks, cancellationToken, () =>
            appointments.Values
                .Where(visit => visit.Veterinarian.Id == veterinarianId)
                .OrderBy(record => record.Id)
                .Select(EntitySnapshot.Copy)
                .ToList());
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Appointment>> GetForPeriodAsync(
        DateTime from,
        DateTime to,
        string roomNumber,
        CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Appointment>>(_snapshotLocks, cancellationToken, () =>
            appointments.Values
                .Where(visit => visit.StartsAt >= from && visit.StartsAt < to && visit.RoomNumber == roomNumber)
                .OrderBy(visit => visit.StartsAt)
                .ThenBy(visit => visit.Id)
                .Select(EntitySnapshot.Copy)
                .ToList());
    }

    /// <inheritdoc />
    public Task<int> CountFollowUpsByBreedAsync(int breedId, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([locks.Pets, locks.Appointments], cancellationToken, () =>
            appointments.Values.Count(visit => visit.IsFollowUp && visit.Pet.Breed.Id == breedId));
    }

    /// <summary>
    /// Проверяет дату и доступность кабинета, возвращает питомца и ветеринара из хранилища.
    /// </summary>
    private (Pet Pet, Veterinarian Veterinarian) ValidateAppointment(Appointment entity, int? currentId = null)
    {
        var pet = pets.GetValueOrDefault(entity.Pet.Id)
            ?? throw new EntityNotFoundException("Связанная запись питомца не найдена.");
        var veterinarian = veterinarians.GetValueOrDefault(entity.Veterinarian.Id)
            ?? throw new EntityNotFoundException("Связанная запись ветеринара не найдена.");
        if (DateOnly.FromDateTime(entity.StartsAt) < pet.BirthDate)
        {
            throw new ValidationException("Приём не может быть раньше рождения питомца.");
        }

        if (appointments.Values.Any(other => other.Id != currentId
            && other.RoomNumber == entity.RoomNumber
            && other.StartsAt == entity.StartsAt))
        {
            throw new AppointmentConflictException();
        }

        return (pet, veterinarian);
    }
}
