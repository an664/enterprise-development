using System.ComponentModel.DataAnnotations;
using VeterinaryClinic.Contracts.Validation;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Contracts.Breeds;

/// <summary>
/// Данные для создания или изменения породы.
/// </summary>
public class BreedRequest
{
    /// <summary>
    /// Название породы.
    /// </summary>
    [Required, StringLength(100), NoNullCharacters]
    public required string Name { get; init; }

    /// <summary>
    /// Вид животного.
    /// </summary>
    [EnumDataType(typeof(AnimalSpecies))]
    public required AnimalSpecies Species { get; init; }
}
