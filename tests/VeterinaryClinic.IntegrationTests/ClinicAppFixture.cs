using Aspire.Hosting;
using Aspire.Hosting.Testing;

namespace VeterinaryClinic.IntegrationTests;

/// <summary>
/// Запускает API с отдельной PostgreSQL, не подключая том с данными разработчика.
/// </summary>
public class ClinicAppFixture : IAsyncLifetime
{
    private DistributedApplication? _app;

    /// <summary>
    /// Клиент работающего API.
    /// </summary>
    public HttpClient Client { get; private set; } = null!;

    /// <summary>
    /// Строка подключения к тестовой базе.
    /// </summary>
    public string ConnectionString { get; private set; } = null!;

    /// <inheritdoc />
    public async ValueTask InitializeAsync()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(3));
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.VeterinaryClinic_AppHost>(
            ["UseVolumes=false"], timeout.Token);
        // Без общего пути Aspire отличает тестовый AppHost по PID и не останавливает его при обычном запуске.
        builder.Configuration["AppHost:FilePath"] = string.Empty;
        _app = await builder.BuildAsync(timeout.Token);
        await _app.StartAsync(timeout.Token);
        await _app.ResourceNotifications.WaitForResourceHealthyAsync("api", timeout.Token);
        Client = _app.CreateHttpClient("api", "http");
        ConnectionString = await _app.GetConnectionStringAsync("clinic", timeout.Token)
            ?? throw new InvalidOperationException("Aspire не передал строку подключения к тестовой базе.");
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        Client?.Dispose();
        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }
}
