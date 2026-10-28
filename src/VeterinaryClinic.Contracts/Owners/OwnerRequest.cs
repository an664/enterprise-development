using System.ComponentModel.DataAnnotations;

namespace VeterinaryClinic.Contracts.Owners;

/// <summary>
/// Данные для создания или изменения владельца.
/// </summary>
public class OwnerRequest
{
    /// <summary>
    /// Фамилия, имя и отчество.
    /// </summary>
    [Required, StringLength(200)]
    public required string FullName { get; init; }

    /// <summary>
    /// Адрес владельца.
    /// </summary>
    [Required, StringLength(300)]
    public required string Address { get; init; }

    /// <summary>
    /// Контактный телефон.
    /// </summary>
    [Required, Phone, StringLength(30)]
    public required string Phone { get; init; }
}
