using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinaryClinic.Domain.Models;

/// <summary>
/// Запись питомца на приём к ветеринару.
/// </summary>
[Table("appointments")]
public class Appointment
{
    /// <summary>
    /// Идентификатор приёма.
    /// </summary>
    [Column("id")]
    public required int Id { get; set; }

    /// <summary>
    /// Питомец, записанный на приём.
    /// </summary>
    public required Pet Pet { get; set; }

    /// <summary>
    /// Ветеринар, проводящий приём.
    /// </summary>
    public required Veterinarian Veterinarian { get; set; }

    /// <summary>
    /// Дата и время приёма в часовом поясе клиники.
    /// </summary>
    [Column("starts_at", TypeName = "timestamp without time zone")]
    public required DateTime StartsAt { get; set; }

    /// <summary>
    /// Номер кабинета.
    /// </summary>
    [Column("room_number")]
    [MaxLength(20)]
    public required string RoomNumber { get; set; }

    /// <summary>
    /// Признак повторного приёма.
    /// </summary>
    [Column("is_follow_up")]
    public bool IsFollowUp { get; set; }
}
