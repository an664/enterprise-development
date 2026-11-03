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
    public async Task<IReadOnlyList<OwnerWithPetCount>> GetWithMultiplePetsAsync(CancellationToken cancellationToken)
    {
        var petCounts = Database.Set<Pet>()
            .GroupBy(pet => pet.Owner.Id)
            .Where(group => group.Count() > 1)
            .Select(group => new { OwnerId = group.Key, PetCount = group.Count() });

        var owners = from owner in Entities.AsNoTracking()
                     join count in petCounts on owner.Id equals count.OwnerId
                     orderby EF.Functions.Collate(owner.FullName, "C"), owner.Id
                     select new OwnerWithPetCount(owner, count.PetCount);

        return await owners.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override void ApplyChanges(Owner current, Owner updated)
    {
        current.FullName = updated.FullName;
        current.Address = updated.Address;
        current.Phone = updated.Phone;
    }
}
