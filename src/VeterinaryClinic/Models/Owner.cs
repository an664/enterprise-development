namespace VeterinaryClinic.Models;

public sealed class Owner
{
    public required int Id { get; init; }
    public required string FullName { get; init; }
    public required string Address { get; init; }
    public required string Phone { get; init; }
}
