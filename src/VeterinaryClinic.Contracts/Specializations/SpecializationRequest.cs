using System.ComponentModel.DataAnnotations;
using VeterinaryClinic.Contracts.Validation;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Contracts.Specializations;

/// <summary>
/// Данные для создания или изменения специализации.
/// </summary>
public class SpecializationRequest
{
    /// <summary>
    /// Название специализации.
    /// </summary>
    [Required, StringLength(100), NoNullCharacters]
    public required string Name { get; init; }

    /// <summary>
    /// Вид животных, с которым работает врач.
    /// </summary>
    [EnumDataType(typeof(AnimalSpecies))]
    public required AnimalSpecies Species { get; init; }
}
