namespace VeterinaryClinic.Models;

/// <summary>A breed belongs to exactly one animal species.</summary>
public sealed class Breed
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required AnimalSpecies Species { get; init; }
}
