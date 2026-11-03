using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Infrastructure.PostgreSQL.Repositories;

/// <summary>
/// Репозиторий пород в PostgreSQL.
/// </summary>
/// <param name="db">Контекст текущего запроса.</param>
internal class BreedRepository(ClinicDbContext db)
    : PostgreSqlRepository<Breed>(db, entity => entity.Id), IBreedRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<Breed>> GetBySpeciesAsync(
        AnimalSpecies species, CancellationToken cancellationToken)
    {
        return await Entities.AsNoTracking()
            .Where(breed => breed.Species == species)
            .OrderBy(breed => breed.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override void ApplyChanges(Breed current, Breed updated)
    {
        current.Name = updated.Name;
        current.Species = updated.Species;
    }
}
