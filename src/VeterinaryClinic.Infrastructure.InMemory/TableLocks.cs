namespace VeterinaryClinic.Infrastructure.InMemory;

/// <summary>
/// Выполняет операцию под блокировками только участвующих коллекций.
/// </summary>
internal static class TableLocks
{
    /// <summary>
    /// Коллекции передаются в одном порядке: владельцы, породы, специализации, ветеринары, питомцы, приёмы.
    /// Это исключает взаимное ожидание при проверке связей между записями.
    /// </summary>
    internal static Task<T> RunAsync<T>(object[] tables, CancellationToken cancellationToken, Func<T> operation)
    {
        T WithLock(int index)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (index == tables.Length)
            {
                return operation();
            }

            lock (tables[index])
            {
                return WithLock(index + 1);
            }
        }

        return Task.FromResult(WithLock(0));
    }
}
