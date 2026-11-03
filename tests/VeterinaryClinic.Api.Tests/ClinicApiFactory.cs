using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace VeterinaryClinic.Api.Tests;

/// <summary>
/// Запускает API с отдельным хранилищем тестовых данных.
/// </summary>
public class ClinicApiFactory : WebApplicationFactory<Program>
{
    /// <inheritdoc />
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config => config.AddInMemoryCollection(
            new Dictionary<string, string?> { ["Storage:Provider"] = "InMemory" }));
        return base.CreateHost(builder);
    }

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureLogging(logging => logging.ClearProviders());
    }

    /// <summary>
    /// Создаёт тело запроса JSON.
    /// </summary>
    /// <param name="value">Текст JSON.</param>
    internal static StringContent Json(string value)
    {
        return new(value, Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// Читает успешный ответ JSON.
    /// </summary>
    /// <param name="client">HTTP-клиент тестового сервера.</param>
    /// <param name="path">Адрес запроса.</param>
    internal static async Task<JsonElement> GetJson(HttpClient client, string path)
    {
        using var response = await client.GetAsync(path, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await ReadJson(response);
    }

    /// <summary>
    /// Читает JSON, сохраняя его после закрытия ответа.
    /// </summary>
    /// <param name="response">Ответ сервера.</param>
    internal static async Task<JsonElement> ReadJson(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

    /// <summary>
    /// Извлекает идентификаторы списка в порядке ответа.
    /// </summary>
    /// <param name="list">Массив записей JSON.</param>
    internal static int[] Ids(JsonElement list)
    {
        return list.EnumerateArray().Select(item => item.GetProperty("id").GetInt32()).ToArray();
    }
}
