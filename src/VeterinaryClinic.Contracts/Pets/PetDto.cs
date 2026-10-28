using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Contracts.Pets;

/// <summary>
/// Сведения о записи питомца.
/// </summary>
/// <param name="Id">Идентификатор записи.</param>
/// <param name="Name">Кличка питомца.</param>
/// <param name="BreedId">Идентификатор породы.</param>
/// <param name="OwnerId">Идентификатор владельца.</param>
/// <param name="BirthDate">Дата рождения.</param>
/// <param name="Weight">Вес в килограммах.</param>
/// <param name="Species">Вид животного, определяемый породой.</param>
public record PetDto(
    int Id,
    string Name,
    int BreedId,
    int OwnerId,
    DateOnly BirthDate,
    decimal Weight,
    AnimalSpecies Species);
