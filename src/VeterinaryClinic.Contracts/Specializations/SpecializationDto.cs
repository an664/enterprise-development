using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Contracts.Specializations;

/// <summary>
/// Сведения о записи специализации.
/// </summary>
/// <param name="Id">Идентификатор записи.</param>
/// <param name="Name">Название специализации.</param>
/// <param name="Species">Вид животных, с которым работает врач.</param>
public record SpecializationDto(
    int Id,
    string Name,
    AnimalSpecies Species);
