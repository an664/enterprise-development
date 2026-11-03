using System.ComponentModel.DataAnnotations;
using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Infrastructure.InMemory.Repositories;

/// <summary>
/// Репозиторий приёмов в памяти.
/// </summary>
/// <param name="owners">Записи владельцев.</param>
/// <param name="breeds">Записи пород.</param>
/// <param name="specializations">Записи специализаций.</param>
/// <param name="veterinarians">Записи ветеринаров.</param>
/// <param name="pets">Записи питомцев.</param>
/// <param name="appointments">Записи приёмов.</param>
internal class AppointmentRepository(
    Dictionary<int, Owner> owners,
    Dictionary<int, Breed> breeds,
    Dictionary<int, Specialization> specializations,
    Dictionary<int, Veterinarian> veterinarians,
    Dictionary<int, Pet> pets,
    Dictionary<int, Appointment> appointments) : IAppointmentRepository
{
    /// <summary>
    /// Коллекции, из которых берутся данные для полной копии записи.
    /// </summary>
    private readonly object[] _snapshotTables = [owners, breeds, specializations, veterinarians, pets, appointments];

    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = appointments.Keys.Max() + 1;

    /// <inheritdoc />
    public Task<IReadOnlyList<Appointment>> GetAllAsync(CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Appointment>>(_snapshotTables, cancellationToken, () =>
            appointments.Values.OrderBy(record => record.Id).Select(EntitySnapshot.Copy).ToList());
    }

    /// <inheritdoc />
    public Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<Appointment?>(_snapshotTables, cancellationToken, () =>
            appointments.TryGetValue(id, out var record) ? EntitySnapshot.Copy(record) : null);
    }

    /// <inheritdoc />
    public Task<Appointment> AddAsync(Appointment entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync(_snapshotTables, cancellationToken, () =>
        {
            var stored = EntitySnapshot.Copy(entity);
            BindReferences(stored);
            EnsureRoomIsAvailable(stored);
            stored.Id = _nextId++;
            appointments.Add(stored.Id, stored);
            return EntitySnapshot.Copy(stored);
        });
    }

    /// <inheritdoc />
    public Task<bool> UpdateAsync(Appointment entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([veterinarians, pets, appointments], cancellationToken, () =>
        {
            if (!appointments.TryGetValue(entity.Id, out var current))
            {
                return false;
            }

            var updated = EntitySnapshot.Copy(entity);
            BindReferences(updated);
            EnsureRoomIsAvailable(updated, current.Id);
            current.Pet = updated.Pet;
            current.Veterinarian = updated.Veterinarian;
            current.StartsAt = updated.StartsAt;
            current.RoomNumber = updated.RoomNumber;
            current.IsFollowUp = updated.IsFollowUp;
            return true;
        });
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([appointments], cancellationToken, () =>
            appointments.Remove(id));
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Appointment>> GetByPetAsync(int petId, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Appointment>>(_snapshotTables, cancellationToken, () =>
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
        return TableLocks.RunAsync([appointments], cancellationToken, () =>
            appointments.Values.Any(visit => visit.Pet.Id == petId && visit.StartsAt < startOfDay));
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Appointment>> GetByVeterinarianAsync(
        int veterinarianId,
        CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Appointment>>(_snapshotTables, cancellationToken, () =>
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
        return TableLocks.RunAsync<IReadOnlyList<Appointment>>(_snapshotTables, cancellationToken, () =>
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
        return TableLocks.RunAsync([pets, appointments], cancellationToken, () =>
            appointments.Values.Count(visit => visit.IsFollowUp && visit.Pet.Breed.Id == breedId));
    }

    /// <summary>
    /// При переносе приёма его собственное время не считается занятым.
    /// </summary>
    private void EnsureRoomIsAvailable(Appointment appointment, int? currentId = null)
    {
        if (appointments.Values.Any(other => other.Id != currentId
            && other.RoomNumber == appointment.RoomNumber
            && other.StartsAt == appointment.StartsAt))
        {
            throw new AppointmentConflictException();
        }
    }

    /// <summary>
    /// Связывает запись с актуальными объектами, пока их нельзя удалить или изменить.
    /// </summary>
    private void BindReferences(Appointment entity)
    {
        entity.Pet = pets.GetValueOrDefault(entity.Pet.Id)
            ?? throw new EntityNotFoundException("Связанная запись питомца не найдена.");
        entity.Veterinarian = veterinarians.GetValueOrDefault(entity.Veterinarian.Id)
            ?? throw new EntityNotFoundException("Связанная запись ветеринара не найдена.");
        if (DateOnly.FromDateTime(entity.StartsAt) < entity.Pet.BirthDate)
        {
            throw new ValidationException("Приём не может быть раньше рождения питомца.");
        }
    }
}
