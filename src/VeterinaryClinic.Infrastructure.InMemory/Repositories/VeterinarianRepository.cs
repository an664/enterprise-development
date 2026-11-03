using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Infrastructure.InMemory.Repositories;

/// <summary>
/// Репозиторий ветеринаров в памяти.
/// </summary>
/// <param name="specializations">Записи специализаций.</param>
/// <param name="veterinarians">Записи ветеринаров.</param>
/// <param name="appointments">Записи приёмов.</param>
internal class VeterinarianRepository(
    Dictionary<int, Specialization> specializations,
    Dictionary<int, Veterinarian> veterinarians,
    Dictionary<int, Appointment> appointments) : IVeterinarianRepository
{
    /// <summary>
    /// Коллекции, из которых берутся данные для полной копии записи.
    /// </summary>
    private readonly object[] _snapshotTables = [specializations, veterinarians];

    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = veterinarians.Keys.Max() + 1;

    /// <inheritdoc />
    public Task<IReadOnlyList<Veterinarian>> GetAllAsync(CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Veterinarian>>(_snapshotTables, cancellationToken, () =>
            veterinarians.Values.OrderBy(record => record.Id).Select(EntitySnapshot.Copy).ToList());
    }

    /// <inheritdoc />
    public Task<Veterinarian?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<Veterinarian?>(_snapshotTables, cancellationToken, () =>
            veterinarians.TryGetValue(id, out var record) ? EntitySnapshot.Copy(record) : null);
    }

    /// <inheritdoc />
    public Task<Veterinarian> AddAsync(Veterinarian entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync(_snapshotTables, cancellationToken, () =>
        {
            var stored = EntitySnapshot.Copy(entity);
            BindReferences(stored);
            stored.Id = _nextId++;
            veterinarians.Add(stored.Id, stored);
            return EntitySnapshot.Copy(stored);
        });
    }

    /// <inheritdoc />
    public Task<bool> UpdateAsync(Veterinarian entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync(_snapshotTables, cancellationToken, () =>
        {
            if (!veterinarians.TryGetValue(entity.Id, out var current))
            {
                return false;
            }

            var updated = EntitySnapshot.Copy(entity);
            BindReferences(updated);
            current.PassportNumber = updated.PassportNumber;
            current.FullName = updated.FullName;
            current.BirthYear = updated.BirthYear;
            current.Specialization = updated.Specialization;
            current.ExperienceYears = updated.ExperienceYears;
            return true;
        });
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([veterinarians, appointments], cancellationToken, () =>
        {
            if (appointments.Values.Any(record => record.Veterinarian.Id == id))
            {
                throw new EntityInUseException("Запись ветеринара используется другими записями.");
            }

            return veterinarians.Remove(id);
        });
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Veterinarian>> GetBySpecializationAsync(
        int specializationId,
        CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Veterinarian>>(_snapshotTables, cancellationToken, () =>
            veterinarians.Values
                .Where(vet => vet.Specialization.Id == specializationId)
                .OrderBy(record => record.Id)
                .Select(EntitySnapshot.Copy)
                .ToList());
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Veterinarian>> GetBySpeciesAsync(
        AnimalSpecies species,
        CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Veterinarian>>(_snapshotTables, cancellationToken, () =>
            veterinarians.Values
                .Where(vet => vet.Specialization.Species == species)
                .OrderBy(record => record.Id)
                .Select(EntitySnapshot.Copy)
                .ToList());
    }

    /// <summary>
    /// Связывает запись с актуальными объектами, пока их нельзя удалить или изменить.
    /// </summary>
    private void BindReferences(Veterinarian entity)
    {
        entity.Specialization = specializations.GetValueOrDefault(entity.Specialization.Id)
            ?? throw new EntityNotFoundException("Связанная запись специализации не найдена.");
    }
}
