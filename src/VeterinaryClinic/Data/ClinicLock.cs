namespace VeterinaryClinic.Data;

/// <summary>
/// Синхронизирует чтение и изменение связанных коллекций в памяти.
/// </summary>
public class ClinicLock
{
    /// <summary>
    /// Общий объект блокировки служб и репозиториев.
    /// </summary>
    public object SyncRoot { get; } = new();
}
