using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Infrastructure.PostgreSQL.Repositories;

/// <summary>
/// Репозиторий питомцев в PostgreSQL.
/// </summary>
/// <param name="db">Контекст текущего запроса.</param>
internal class PetRepository(ClinicDbContext db)
    : PostgreSqlRepository<Pet>(db, entity => entity.Id), IPetRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<Pet>> GetByOwnerAsync(int ownerId, CancellationToken cancellationToken)
    {
        return await Entities.AsNoTracking()
            .Where(pet => pet.Owner.Id == ownerId)
            .OrderBy(pet => pet.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Pet>> GetByBreedAsync(int breedId, CancellationToken cancellationToken)
    {
        return await Entities.AsNoTracking()
            .Where(pet => pet.Breed.Id == breedId)
            .OrderBy(pet => pet.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Pet>> GetByVeterinarianAsync(
        int veterinarianId, CancellationToken cancellationToken)
    {
        var appointments = Database.Set<Appointment>();
        return await Entities.AsNoTracking()
            .Where(pet => appointments.Any(visit => visit.Pet.Id == pet.Id && visit.Veterinarian.Id == veterinarianId))
            .OrderBy(pet => EF.Functions.Collate(pet.Name, "C"))
            .ThenBy(pet => pet.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override void ApplyChanges(Pet current, Pet updated)
    {
        current.Name = updated.Name;
        current.Breed = updated.Breed;
        current.Owner = updated.Owner;
        current.BirthDate = updated.BirthDate;
        current.Weight = updated.Weight;
    }
}
