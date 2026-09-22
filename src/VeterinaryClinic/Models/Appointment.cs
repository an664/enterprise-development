namespace VeterinaryClinic.Models;

/// <summary>
/// Запись питомца на приём к ветеринару.
/// </summary>
public class Appointment
{
    /// <summary>
    /// Идентификатор приёма.
    /// </summary>
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
    public required DateTime StartsAt { get; set; }

    /// <summary>
    /// Номер кабинета.
    /// </summary>
    public required string RoomNumber { get; set; }

    /// <summary>
    /// Признак повторного приёма.
    /// </summary>
    public bool IsFollowUp { get; set; }
}
