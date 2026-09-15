namespace VeterinaryClinic.Models;

public sealed class Veterinarian
{
    public required int Id { get; init; }
    public required string PassportNumber { get; init; }
    public required string FullName { get; init; }
    public required int BirthYear { get; init; }
    public required Specialization Specialization { get; init; }
    public required int ExperienceYears { get; init; }
}
