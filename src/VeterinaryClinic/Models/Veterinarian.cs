namespace VeterinaryClinic.Models;

/// <summary>
/// Ветеринар клиники.
/// </summary>
public class Veterinarian
{
    /// <summary>
    /// Идентификатор ветеринара.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Номер паспорта ветеринара.
    /// </summary>
    public required string PassportNumber { get; set; }

    /// <summary>
    /// Фамилия, имя и отчество ветеринара.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// Год рождения ветеринара.
    /// </summary>
    public required int BirthYear { get; set; }

    /// <summary>
    /// Специализация ветеринара.
    /// </summary>
    public required Specialization Specialization { get; set; }

    /// <summary>
    /// Стаж работы ветеринара в годах.
    /// </summary>
    public int ExperienceYears { get; set; }
}
