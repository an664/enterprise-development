using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Infrastructure.PostgreSQL.Repositories;

/// <summary>
/// Репозиторий ветеринаров в PostgreSQL.
/// </summary>
/// <param name="db">Контекст текущего запроса.</param>
internal class VeterinarianRepository(ClinicDbContext db)
    : PostgreSqlRepository<Veterinarian>(db, entity => entity.Id), IVeterinarianRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<Veterinarian>> GetBySpecializationAsync(
        int specializationId,
        CancellationToken cancellationToken)
    {
        return await Entities.AsNoTracking()
            .Where(vet => vet.Specialization.Id == specializationId)
            .OrderBy(vet => vet.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Veterinarian>> GetBySpeciesAsync(
        AnimalSpecies species,
        CancellationToken cancellationToken)
    {
        return await Entities.AsNoTracking()
            .Where(vet => vet.Specialization.Species == species)
            .OrderBy(vet => vet.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override void ApplyChanges(Veterinarian current, Veterinarian updated)
    {
        current.FullName = updated.FullName;
        current.PassportNumber = updated.PassportNumber;
        current.BirthYear = updated.BirthYear;
        current.Specialization = updated.Specialization;
        current.ExperienceYears = updated.ExperienceYears;
    }
}
