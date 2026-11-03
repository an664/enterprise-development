using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Infrastructure.PostgreSQL.Repositories;

/// <summary>
/// Репозиторий приёмов в PostgreSQL.
/// </summary>
/// <param name="db">Контекст текущего запроса.</param>
internal class AppointmentRepository(ClinicDbContext db)
    : PostgreSqlRepository<Appointment>(db, entity => entity.Id), IAppointmentRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<Appointment>> GetByPetAsync(int petId, CancellationToken cancellationToken)
    {
        return await Entities.AsNoTracking()
            .Where(visit => visit.Pet.Id == petId)
            .OrderBy(visit => visit.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> HasAppointmentBeforeAsync(int petId, DateOnly date, CancellationToken cancellationToken)
    {
        var startOfDay = date.ToDateTime(TimeOnly.MinValue);
        return Entities.AnyAsync(visit => visit.Pet.Id == petId && visit.StartsAt < startOfDay, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Appointment>> GetByVeterinarianAsync(
        int veterinarianId,
        CancellationToken cancellationToken)
    {
        return await Entities.AsNoTracking()
            .Where(visit => visit.Veterinarian.Id == veterinarianId)
            .OrderBy(visit => visit.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Appointment>> GetForPeriodAsync(
        DateTime from,
        DateTime to,
        string roomNumber,
        CancellationToken cancellationToken)
    {
        return await Entities.AsNoTracking()
            .Where(visit => visit.RoomNumber == roomNumber && visit.StartsAt >= from && visit.StartsAt < to)
            .OrderBy(visit => visit.StartsAt)
            .ThenBy(visit => visit.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountFollowUpsByBreedAsync(int breedId, CancellationToken cancellationToken)
    {
        return Entities.CountAsync(visit => visit.IsFollowUp && visit.Pet.Breed.Id == breedId, cancellationToken);
    }

    /// <inheritdoc />
    protected override void ApplyChanges(Appointment current, Appointment updated)
    {
        current.Pet = updated.Pet;
        current.Veterinarian = updated.Veterinarian;
        current.StartsAt = updated.StartsAt;
        current.RoomNumber = updated.RoomNumber;
        current.IsFollowUp = updated.IsFollowUp;
    }
}
