using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Contracts.Breeds;

/// <summary>
/// Сведения о записи породы.
/// </summary>
/// <param name="Id">Идентификатор записи.</param>
/// <param name="Name">Название породы.</param>
/// <param name="Species">Вид животного.</param>
public record BreedDto(
    int Id,
    string Name,
    AnimalSpecies Species);
