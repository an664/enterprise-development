namespace VeterinaryClinic.Contracts.Appointments;

/// <summary>
/// Сведения о записи приёма.
/// </summary>
/// <param name="Id">Идентификатор записи.</param>
/// <param name="PetId">Идентификатор питомца.</param>
/// <param name="VeterinarianId">Идентификатор ветеринара.</param>
/// <param name="StartsAt">Местные дата и время приёма, без смещения UTC.</param>
/// <param name="RoomNumber">Номер кабинета.</param>
/// <param name="IsFollowUp">Признак повторного приёма.</param>
public record AppointmentDto(
    int Id,
    int PetId,
    int VeterinarianId,
    DateTime StartsAt,
    string RoomNumber,
    bool IsFollowUp);
