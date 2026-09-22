namespace VeterinaryClinic.Models;

/// <summary>
/// Питомец ветеринарной клиники.
/// </summary>
public class Pet
{
    /// <summary>
    /// Идентификатор питомца.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Кличка питомца.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Порода питомца.
    /// </summary>
    public required Breed Breed { get; set; }

    /// <summary>
    /// Владелец питомца.
    /// </summary>
    public required Owner Owner { get; set; }

    /// <summary>
    /// Дата рождения питомца.
    /// </summary>
    public required DateOnly BirthDate { get; set; }

    /// <summary>
    /// Вес питомца в килограммах.
    /// </summary>
    public required decimal Weight { get; set; }

    /// <summary>
    /// Вид питомца, определяемый его породой.
    /// </summary>
    public AnimalSpecies Species => Breed.Species;
}
