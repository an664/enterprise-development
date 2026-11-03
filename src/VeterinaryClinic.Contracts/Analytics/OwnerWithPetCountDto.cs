namespace VeterinaryClinic.Contracts.Analytics;

/// <summary>
/// Сведения о владельце с количеством его питомцев.
/// </summary>
/// <param name="Id">Идентификатор владельца.</param>
/// <param name="FullName">Фамилия, имя и отчество.</param>
/// <param name="Address">Адрес владельца.</param>
/// <param name="Phone">Контактный телефон.</param>
/// <param name="PetCount">Количество питомцев.</param>
public record OwnerWithPetCountDto(
    int Id,
    string FullName,
    string Address,
    string Phone,
    int PetCount);
