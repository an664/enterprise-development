namespace VeterinaryClinic.Infrastructure.InMemory;

/// <summary>
/// Отдельные блокировки для коллекций одного хранилища в памяти.
/// </summary>
internal class TableLocks
{
    /// <summary>
    /// Блокировка коллекции владельцев.
    /// </summary>
    internal Lock Owners { get; } = new();

    /// <summary>
    /// Блокировка коллекции пород.
    /// </summary>
    internal Lock Breeds { get; } = new();

    /// <summary>
    /// Блокировка коллекции специализаций.
    /// </summary>
    internal Lock Specializations { get; } = new();

    /// <summary>
    /// Блокировка коллекции ветеринаров.
    /// </summary>
    internal Lock Veterinarians { get; } = new();

    /// <summary>
    /// Блокировка коллекции питомцев.
    /// </summary>
    internal Lock Pets { get; } = new();

    /// <summary>
    /// Блокировка коллекции приёмов.
    /// </summary>
    internal Lock Appointments { get; } = new();

    /// <summary>
    /// Блокировки передаются в одном порядке: владельцы, породы, специализации, ветеринары, питомцы, приёмы.
    /// Это исключает взаимное ожидание при проверке связей между записями.
    /// </summary>
    internal static Task<T> RunAsync<T>(Lock[] locks, CancellationToken cancellationToken, Func<T> operation)
    {
        T WithLock(int index)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (index == locks.Length)
            {
                return operation();
            }

            lock (locks[index])
            {
                return WithLock(index + 1);
            }
        }

        return Task.FromResult(WithLock(0));
    }
}
