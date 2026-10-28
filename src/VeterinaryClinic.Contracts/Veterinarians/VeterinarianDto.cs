namespace VeterinaryClinic.Contracts.Veterinarians;

/// <summary>
/// Сведения о записи ветеринара.
/// </summary>
/// <param name="Id">Идентификатор записи.</param>
/// <param name="PassportNumber">Номер паспорта.</param>
/// <param name="FullName">Фамилия, имя и отчество.</param>
/// <param name="BirthYear">Год рождения.</param>
/// <param name="SpecializationId">Идентификатор специализации.</param>
/// <param name="ExperienceYears">Стаж работы в годах.</param>
public record VeterinarianDto(
    int Id,
    string PassportNumber,
    string FullName,
    int BirthYear,
    int SpecializationId,
    int ExperienceYears);
