using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Domain.Models;

/// <summary>
/// Питомец ветеринарной клиники.
/// </summary>
[Table("pets")]
public class Pet
{
    /// <summary>
    /// Идентификатор питомца.
    /// </summary>
    [Column("id")]
    public required int Id { get; set; }

    /// <summary>
    /// Кличка питомца.
    /// </summary>
    [Column("name")]
    [MaxLength(100)]
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
    [Column("birth_date")]
    public required DateOnly BirthDate { get; set; }

    /// <summary>
    /// Вес питомца в килограммах.
    /// </summary>
    [Column("weight")]
    public required decimal Weight { get; set; }

    /// <summary>
    /// Вид питомца, определяемый его породой.
    /// </summary>
    [NotMapped]
    public AnimalSpecies Species => Breed.Species;
}
