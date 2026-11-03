using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinaryClinic.Domain.Models;

/// <summary>
/// Ветеринар клиники.
/// </summary>
[Table("veterinarians")]
public class Veterinarian
{
    /// <summary>
    /// Идентификатор ветеринара.
    /// </summary>
    [Column("id")]
    public required int Id { get; set; }

    /// <summary>
    /// Номер паспорта ветеринара.
    /// </summary>
    [Column("passport_number")]
    [MaxLength(30)]
    public required string PassportNumber { get; set; }

    /// <summary>
    /// Фамилия, имя и отчество ветеринара.
    /// </summary>
    [Column("full_name")]
    [MaxLength(200)]
    public required string FullName { get; set; }

    /// <summary>
    /// Год рождения ветеринара.
    /// </summary>
    [Column("birth_year")]
    public required int BirthYear { get; set; }

    /// <summary>
    /// Специализация ветеринара.
    /// </summary>
    public required Specialization Specialization { get; set; }

    /// <summary>
    /// Стаж работы ветеринара в годах.
    /// </summary>
    [Column("experience_years")]
    public int ExperienceYears { get; set; }
}
