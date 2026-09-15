namespace VeterinaryClinic.Models;

/// <summary>A pet's appointment with a veterinarian; the domain contract.</summary>
public sealed class Appointment
{
    public required int Id { get; init; }
    public required Pet Pet { get; init; }
    public required Veterinarian Veterinarian { get; init; }

    /// <summary>Date and time in the clinic's local time zone.</summary>
    public required DateTime StartsAt { get; init; }

    public required int RoomNumber { get; init; }
    public required bool IsFollowUp { get; init; }
}
