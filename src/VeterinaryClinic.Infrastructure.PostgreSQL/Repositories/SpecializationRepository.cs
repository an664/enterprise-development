using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Infrastructure.PostgreSQL.Repositories;

/// <summary>
/// Репозиторий специализаций в PostgreSQL.
/// </summary>
/// <param name="db">Контекст текущего запроса.</param>
internal class SpecializationRepository(ClinicDbContext db)
    : PostgreSqlRepository<Specialization>(db, entity => entity.Id), ISpecializationRepository
{
    /// <inheritdoc />
    protected override void ApplyChanges(Specialization current, Specialization updated)
    {
        current.Name = updated.Name;
        current.Species = updated.Species;
    }
}
