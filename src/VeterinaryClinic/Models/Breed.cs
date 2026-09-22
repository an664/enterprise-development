namespace VeterinaryClinic.Models;

/// <summary>
/// Порода животного.
/// </summary>
public class Breed
{
    /// <summary>
    /// Идентификатор породы.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Название породы.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Биологический вид животного.
    /// </summary>
    public required AnimalSpecies Species { get; set; }
}
