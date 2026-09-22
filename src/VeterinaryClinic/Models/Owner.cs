namespace VeterinaryClinic.Models;

/// <summary>
/// Владелец питомцев.
/// </summary>
public class Owner
{
    /// <summary>
    /// Идентификатор владельца.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Фамилия, имя и отчество владельца.
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// Адрес владельца.
    /// </summary>
    public required string Address { get; set; }

    /// <summary>
    /// Телефон владельца.
    /// </summary>
    public required string Phone { get; set; }
}
