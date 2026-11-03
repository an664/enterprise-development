using System.ComponentModel;
using System.Globalization;

namespace VeterinaryClinic.Shared;

/// <summary>
/// Преобразует имя или код одного вида животного без объединения значений как флагов.
/// </summary>
public class AnimalSpeciesConverter() : EnumConverter(typeof(AnimalSpecies))
{
    /// <summary>
    /// Распознаёт имя без учёта регистра или числовой код из перечисления.
    /// </summary>
    /// <param name="text">Имя или код вида.</param>
    /// <param name="species">Распознанный вид.</param>
    /// <returns>Указан ровно один существующий вид.</returns>
    public static bool TryParse(string text, out AnimalSpecies species)
    {
        species = default;
        return !text.Contains(',')
            && Enum.TryParse(text, ignoreCase: true, out species)
            && Enum.IsDefined(species);
    }

    /// <inheritdoc />
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string text)
        {
            return TryParse(text, out var species)
                ? species
                : throw new FormatException("Укажите один вид животного из списка.");
        }

        return base.ConvertFrom(context, culture, value);
    }
}
