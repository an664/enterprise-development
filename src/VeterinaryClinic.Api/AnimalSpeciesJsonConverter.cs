using System.Text.Json;
using System.Text.Json.Serialization;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Api;

/// <summary>
/// Читает один вид животного по имени или числовому коду, а в ответе записывает имя.
/// </summary>
internal class AnimalSpeciesJsonConverter : JsonConverter<AnimalSpecies>
{
    /// <inheritdoc />
    public override AnimalSpecies Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String
            && AnimalSpeciesConverter.TryParse(reader.GetString()!, out var species))
        {
            return species;
        }

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var code)
            && Enum.IsDefined((AnimalSpecies)code))
        {
            return (AnimalSpecies)code;
        }

        throw new JsonException("Укажите один вид животного из списка.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, AnimalSpecies value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
