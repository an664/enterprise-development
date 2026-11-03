using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Domain.Models;

/// <summary>
/// Специализация ветеринара.
/// </summary>
[Table("specializations")]
public class Specialization
{
    /// <summary>
    /// Идентификатор специализации.
    /// </summary>
    [Column("id")]
    public required int Id { get; set; }

    /// <summary>
    /// Название специализации.
    /// </summary>
    [Column("name")]
    [MaxLength(100)]
    public required string Name { get; set; }

    /// <summary>
    /// Вид животных, на котором специализируется ветеринар.
    /// </summary>
    [Column("species")]
    public required AnimalSpecies Species { get; set; }
}
