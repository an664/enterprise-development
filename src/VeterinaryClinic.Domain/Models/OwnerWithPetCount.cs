namespace VeterinaryClinic.Domain.Models;

/// <summary>
/// Владелец и количество его питомцев.
/// </summary>
/// <param name="Owner">Владелец питомцев.</param>
/// <param name="PetCount">Количество питомцев.</param>
public record OwnerWithPetCount(Owner Owner, int PetCount);
