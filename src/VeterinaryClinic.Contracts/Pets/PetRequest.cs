using System.ComponentModel.DataAnnotations;

namespace VeterinaryClinic.Contracts.Pets;

/// <summary>
/// Данные для создания или изменения питомца.
/// </summary>
public class PetRequest
{
    /// <summary>
    /// Кличка питомца.
    /// </summary>
    [Required, StringLength(100)]
    public required string Name { get; init; }

    /// <summary>
    /// Идентификатор породы.
    /// </summary>
    [Range(1, int.MaxValue)]
    public required int BreedId { get; init; }

    /// <summary>
    /// Идентификатор владельца.
    /// </summary>
    [Range(1, int.MaxValue)]
    public required int OwnerId { get; init; }

    /// <summary>
    /// Дата рождения.
    /// </summary>
    public required DateOnly BirthDate { get; init; }

    /// <summary>
    /// Вес в килограммах.
    /// </summary>
    [Range(typeof(decimal), "0.001", "1000000",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true)]
    public required decimal Weight { get; init; }
}
