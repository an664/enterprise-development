using System.ComponentModel.DataAnnotations;

namespace VeterinaryClinic.Contracts.Validation;

/// <summary>
/// Ограничивает длину номера кабинета без учёта пробелов по краям.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class RoomNumberLengthAttribute()
    : ValidationAttribute("Номер кабинета не может быть длиннее 20 символов без учёта пробелов по краям.")
{
    /// <inheritdoc />
    public override bool IsValid(object? value)
    {
        return value is null || value is string roomNumber && roomNumber.Trim().Length <= 20;
    }
}
