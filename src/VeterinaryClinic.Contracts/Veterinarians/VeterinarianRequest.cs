using System.ComponentModel.DataAnnotations;
using VeterinaryClinic.Contracts.Validation;

namespace VeterinaryClinic.Contracts.Veterinarians;

/// <summary>
/// Данные для создания или изменения ветеринара.
/// </summary>
public class VeterinarianRequest
{
    /// <summary>
    /// Номер паспорта.
    /// </summary>
    [Required, StringLength(30), NoNullCharacters]
    public required string PassportNumber { get; init; }

    /// <summary>
    /// Фамилия, имя и отчество.
    /// </summary>
    [Required, StringLength(200), NoNullCharacters]
    public required string FullName { get; init; }

    /// <summary>
    /// Год рождения.
    /// </summary>
    [Range(1900, 9999)]
    public required int BirthYear { get; init; }

    /// <summary>
    /// Идентификатор специализации.
    /// </summary>
    [Range(1, int.MaxValue)]
    public required int SpecializationId { get; init; }

    /// <summary>
    /// Стаж работы в годах.
    /// </summary>
    [Range(0, 150)]
    public int ExperienceYears { get; init; }
}
