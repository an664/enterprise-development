namespace VeterinaryClinic.Contracts.Analytics;

/// <summary>
/// Количество повторных приёмов питомцев одной породы.
/// </summary>
/// <param name="BreedId">Идентификатор породы.</param>
/// <param name="Count">Количество повторных приёмов.</param>
public record FollowUpCountDto(int BreedId, int Count);
