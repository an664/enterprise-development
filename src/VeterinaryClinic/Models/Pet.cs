namespace VeterinaryClinic.Models;

public sealed class Pet
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required Breed Breed { get; init; }
    public required Owner Owner { get; init; }
    public required DateOnly BirthDate { get; init; }
    public required decimal WeightKg { get; init; }

    // Deriving the species prevents a pet and its breed from disagreeing.
    public AnimalSpecies Species => Breed.Species;
}
