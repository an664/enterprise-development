using System.ComponentModel.DataAnnotations;

namespace VeterinaryClinic.Contracts.Validation;

/// <summary>
/// Запрещает нулевой символ, который нельзя сохранить в текстовом поле PostgreSQL.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class NoNullCharactersAttribute() : ValidationAttribute("Текст не может содержать нулевой символ.")
{
    /// <inheritdoc />
    public override bool IsValid(object? value)
    {
        return value is not string text || !text.Contains('\0');
    }
}
