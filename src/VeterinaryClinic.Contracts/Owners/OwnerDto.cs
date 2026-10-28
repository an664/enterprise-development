namespace VeterinaryClinic.Contracts.Owners;

/// <summary>
/// Сведения о записи владельца.
/// </summary>
/// <param name="Id">Идентификатор записи.</param>
/// <param name="FullName">Фамилия, имя и отчество.</param>
/// <param name="Address">Адрес владельца.</param>
/// <param name="Phone">Контактный телефон.</param>
public record OwnerDto(
    int Id,
    string FullName,
    string Address,
    string Phone);
