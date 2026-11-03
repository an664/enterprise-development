using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Infrastructure.PostgreSQL.Repositories;

/// <summary>
/// Репозиторий владельцев в PostgreSQL.
/// </summary>
/// <param name="db">Контекст текущего запроса.</param>
internal class OwnerRepository(ClinicDbContext db)
    : PostgreSqlRepository<Owner>(db, entity => entity.Id), IOwnerRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<Owner>> GetWithMultiplePetsAsync(CancellationToken cancellationToken)
    {
        var pets = Database.Set<Pet>();
        return await Entities.AsNoTracking()
            .Where(owner => pets.Count(pet => pet.Owner.Id == owner.Id) > 1)
            .OrderBy(owner => EF.Functions.Collate(owner.FullName, "C"))
            .ThenBy(owner => owner.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override void ApplyChanges(Owner current, Owner updated)
    {
        current.FullName = updated.FullName;
        current.Address = updated.Address;
        current.Phone = updated.Phone;
    }
}
