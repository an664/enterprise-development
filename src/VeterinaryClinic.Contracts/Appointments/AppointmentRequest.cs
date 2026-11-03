using System.ComponentModel.DataAnnotations;
using VeterinaryClinic.Contracts.Validation;

namespace VeterinaryClinic.Contracts.Appointments;

/// <summary>
/// Данные для создания или изменения приёма.
/// </summary>
public class AppointmentRequest
{
    /// <summary>
    /// Идентификатор питомца.
    /// </summary>
    [Range(1, int.MaxValue)]
    public required int PetId { get; init; }

    /// <summary>
    /// Идентификатор ветеринара.
    /// </summary>
    [Range(1, int.MaxValue)]
    public required int VeterinarianId { get; init; }

    /// <summary>
    /// Местные дата и время приёма без смещения UTC. Точность хранения - одна микросекунда.
    /// </summary>
    public required DateTime StartsAt { get; init; }

    /// <summary>
    /// Номер кабинета: от 1 до 20 символов после удаления пробелов по краям.
    /// </summary>
    [Required, RoomNumberLength, NoNullCharacters]
    public required string RoomNumber { get; init; }

    /// <summary>
    /// Признак повторного приёма.
    /// </summary>
    public bool IsFollowUp { get; init; }
}
