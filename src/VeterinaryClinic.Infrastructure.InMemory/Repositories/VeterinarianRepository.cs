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
/// <param name="locks">Блокировки коллекций хранилища.</param>
internal class VeterinarianRepository(
    Dictionary<int, Specialization> specializations,
    Dictionary<int, Veterinarian> veterinarians,
    Dictionary<int, Appointment> appointments,
    TableLocks locks) : IVeterinarianRepository
{
    /// <summary>
    /// Блокировки коллекций, из которых копируются данные записи.
    /// </summary>
    private readonly Lock[] _snapshotLocks = [locks.Specializations, locks.Veterinarians];

    /// <summary>
    /// Следующий свободный идентификатор.
    /// </summary>
    private int _nextId = veterinarians.Keys.Max() + 1;

    /// <inheritdoc />
    public Task<IReadOnlyList<Veterinarian>> GetAllAsync(CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<IReadOnlyList<Veterinarian>>(_snapshotLocks, cancellationToken, () =>
            veterinarians.Values.OrderBy(record => record.Id).Select(EntitySnapshot.Copy).ToList());
    }

    /// <inheritdoc />
    public Task<Veterinarian?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync<Veterinarian?>(_snapshotLocks, cancellationToken, () =>
            veterinarians.TryGetValue(id, out var record) ? EntitySnapshot.Copy(record) : null);
    }

    /// <inheritdoc />
    public Task<Veterinarian> AddAsync(Veterinarian entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync(_snapshotLocks, cancellationToken, () =>
        {
            var specialization = GetSpecialization(entity.Specialization.Id);
            var stored = new Veterinarian
            {
                Id = _nextId++,
                PassportNumber = entity.PassportNumber,
                FullName = entity.FullName,
                BirthYear = entity.BirthYear,
                Specialization = specialization,
                ExperienceYears = entity.ExperienceYears
            };
            veterinarians.Add(stored.Id, stored);
            return EntitySnapshot.Copy(stored);
        });
    }

    /// <inheritdoc />
    public Task<bool> UpdateAsync(Veterinarian entity, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync(_snapshotLocks, cancellationToken, () =>
        {
            if (!veterinarians.TryGetValue(entity.Id, out var current))
            {
                return false;
            }

            var specialization = GetSpecialization(entity.Specialization.Id);
            current.PassportNumber = entity.PassportNumber;
            current.FullName = entity.FullName;
            current.BirthYear = entity.BirthYear;
            current.Specialization = specialization;
            current.ExperienceYears = entity.ExperienceYears;
            return true;
        });
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return TableLocks.RunAsync([locks.Veterinarians, locks.Appointments], cancellationToken, () =>
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
        return TableLocks.RunAsync<IReadOnlyList<Veterinarian>>(_snapshotLocks, cancellationToken, () =>
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
        return TableLocks.RunAsync<IReadOnlyList<Veterinarian>>(_snapshotLocks, cancellationToken, () =>
            veterinarians.Values
                .Where(vet => vet.Specialization.Species == species)
                .OrderBy(record => record.Id)
                .Select(EntitySnapshot.Copy)
                .ToList());
    }

    /// <summary>
    /// Находит специализацию ветеринара в хранилище.
    /// </summary>
    private Specialization GetSpecialization(int id)
    {
        return specializations.GetValueOrDefault(id)
            ?? throw new EntityNotFoundException("Связанная запись специализации не найдена.");
    }
}
