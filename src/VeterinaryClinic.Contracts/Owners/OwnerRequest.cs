using System.ComponentModel.DataAnnotations;
using VeterinaryClinic.Contracts.Validation;

namespace VeterinaryClinic.Contracts.Owners;

/// <summary>
/// Данные для создания или изменения владельца.
/// </summary>
public class OwnerRequest
{
    /// <summary>
    /// Фамилия, имя и отчество.
    /// </summary>
    [Required, StringLength(200), NoNullCharacters]
    public required string FullName { get; init; }

    /// <summary>
    /// Адрес владельца.
    /// </summary>
    [Required, StringLength(300), NoNullCharacters]
    public required string Address { get; init; }

    /// <summary>
    /// Контактный телефон.
    /// </summary>
    [Required, Phone, StringLength(30), NoNullCharacters]
    public required string Phone { get; init; }
}
