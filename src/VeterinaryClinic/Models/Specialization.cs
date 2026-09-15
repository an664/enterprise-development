namespace VeterinaryClinic.Models;

/// <summary>A directory entry describing expertise in a particular species.</summary>
public sealed class Specialization
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required AnimalSpecies Species { get; init; }
}
