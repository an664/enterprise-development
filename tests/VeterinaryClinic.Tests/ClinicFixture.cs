using VeterinaryClinic.Data;

namespace VeterinaryClinic.Tests;

/// <summary>
/// Подготавливает общие данные ветеринарной клиники для тестов.
/// </summary>
public class ClinicFixture
{
    /// <summary>
    /// Общий набор данных с фиксированными датами приёмов.
    /// </summary>
    internal ClinicData Data { get; } = ClinicSeed.Create();
}
