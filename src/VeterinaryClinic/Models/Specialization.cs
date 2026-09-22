namespace VeterinaryClinic.Models;

/// <summary>
/// Специализация ветеринара.
/// </summary>
public class Specialization
{
    /// <summary>
    /// Идентификатор специализации.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Название специализации.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Вид животных, на котором специализируется ветеринар.
    /// </summary>
    public required AnimalSpecies Species { get; set; }
}
