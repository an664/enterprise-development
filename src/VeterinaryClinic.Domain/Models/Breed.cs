using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Domain.Models;

/// <summary>
/// Порода животного.
/// </summary>
[Table("breeds")]
public class Breed
{
    /// <summary>
    /// Идентификатор породы.
    /// </summary>
    [Column("id")]
    public required int Id { get; set; }

    /// <summary>
    /// Название породы.
    /// </summary>
    [Column("name")]
    [MaxLength(100)]
    public required string Name { get; set; }

    /// <summary>
    /// Биологический вид животного.
    /// </summary>
    [Column("species")]
    public required AnimalSpecies Species { get; set; }
}
