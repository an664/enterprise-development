using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinaryClinic.Domain.Models;

/// <summary>
/// Владелец питомцев.
/// </summary>
[Table("owners")]
public class Owner
{
    /// <summary>
    /// Идентификатор владельца.
    /// </summary>
    [Column("id")]
    public required int Id { get; set; }

    /// <summary>
    /// Фамилия, имя и отчество владельца.
    /// </summary>
    [Column("full_name")]
    [MaxLength(200)]
    public required string FullName { get; set; }

    /// <summary>
    /// Адрес владельца.
    /// </summary>
    [Column("address")]
    [MaxLength(300)]
    public required string Address { get; set; }

    /// <summary>
    /// Телефон владельца.
    /// </summary>
    [Column("phone")]
    [MaxLength(30)]
    public required string Phone { get; set; }
}
