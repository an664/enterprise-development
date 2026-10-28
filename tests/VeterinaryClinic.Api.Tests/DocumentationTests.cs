using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using static VeterinaryClinic.Api.Tests.ClinicApiFactory;

namespace VeterinaryClinic.Api.Tests;

/// <summary>
/// Проверяет опубликованную документацию API.
/// </summary>
public class DocumentationTests
{
    /// <summary>
    /// Проверяет описания операций, схемы моделей и коды ответов OpenAPI.
    /// </summary>
    [Fact]
    public async Task OpenApi_ContainsSummariesSchemasAndResponseMetadata()
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();
        string[] resources = ["owners", "breeds", "pets", "specializations", "veterinarians", "appointments"];
        string[] models = ["Owner", "Breed", "Pet", "Specialization", "Veterinarian", "Appointment"];

        var document = await GetJson(client, "/openapi/v1.json");

        Assert.StartsWith("3.", document.GetProperty("openapi").GetString());
        Assert.False(string.IsNullOrWhiteSpace(document.GetProperty("info").GetProperty("title").GetString()));
        var paths = document.GetProperty("paths");
        var breedId = paths.GetProperty("/api/analytics/follow-up-count").GetProperty("get")
            .GetProperty("parameters").EnumerateArray()
            .Single(parameter => parameter.GetProperty("name").GetString() == "breedId"
                && parameter.GetProperty("in").GetString() == "query");
        var ownerId = paths.GetProperty("/api/owners/{id}").GetProperty("get")
            .GetProperty("parameters").EnumerateArray()
            .Single(parameter => parameter.GetProperty("name").GetString() == "id"
                && parameter.GetProperty("in").GetString() == "path");
        Assert.Equal("integer", breedId.GetProperty("schema").GetProperty("type").GetString());
        Assert.Equal("integer", ownerId.GetProperty("schema").GetProperty("type").GetString());
        foreach (var resource in resources)
        {
            var collection = paths.GetProperty($"/api/{resource}");
            var item = paths.GetProperty($"/api/{resource}/{{id}}");
            Assert.True(collection.GetProperty("get").GetProperty("responses").TryGetProperty("200", out _));
            var create = collection.GetProperty("post");
            Assert.True(create.GetProperty("responses").TryGetProperty("201", out _));
            Assert.True(create.GetProperty("responses").TryGetProperty("400", out _));
            var requestBody = create.GetProperty("requestBody");
            Assert.True(requestBody.GetProperty("required").GetBoolean());
            Assert.True(requestBody.GetProperty("content").TryGetProperty("application/json", out _));
            Assert.True(item.GetProperty("get").GetProperty("responses").TryGetProperty("404", out _));
            Assert.True(item.GetProperty("put").GetProperty("responses").TryGetProperty("204", out _));
            Assert.True(item.GetProperty("delete").GetProperty("responses").TryGetProperty("204", out _));
        }

        foreach (var path in paths.EnumerateObject())
        {
            var operations = path.Value.EnumerateObject()
                .Where(value => value.Name is "get" or "post" or "put" or "delete");
            foreach (var operation in operations)
            {
                Assert.True(
                    operation.Value.TryGetProperty("summary", out var summary),
                    $"Нет summary у {operation.Name} {path.Name}");
                Assert.False(string.IsNullOrWhiteSpace(summary.GetString()));
                Assert.True(operation.Value.GetProperty("responses").TryGetProperty("500", out _));
            }
        }

        var schemas = document.GetProperty("components").GetProperty("schemas");
        foreach (var model in models)
        {
            Assert.True(schemas.TryGetProperty($"{model}Dto", out _));
            Assert.True(schemas.TryGetProperty($"{model}Request", out _));
        }
        var petRequest = schemas.GetProperty("PetRequest");
        Assert.Contains(
            petRequest.GetProperty("required").EnumerateArray(),
            value => value.GetString() == "birthDate");
        var weight = petRequest.GetProperty("properties").GetProperty("weight");
        Assert.Contains("килограмм", weight.GetProperty("description").GetString());
        Assert.Equal(0.001m, weight.GetProperty("minimum").GetDecimal());
        var appointmentRequest = schemas.GetProperty("AppointmentRequest");
        Assert.Contains(
            appointmentRequest.GetProperty("required").EnumerateArray(),
            value => value.GetString() == "startsAt");
    }

    /// <summary>
    /// Проверяет доступность пользовательской страницы Swagger.
    /// </summary>
    [Fact]
    public async Task Swagger_ReturnsHtml()
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(
            "/swagger/index.html",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        var html = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("swagger-ui", html);
        Assert.Contains("src=\"index.js\"", html);
        using var script = await client.GetAsync("/swagger/index.js", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, script.StatusCode);
        var scriptText = await script.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("/openapi/v1.json", scriptText);
    }

    /// <summary>
    /// Проверяет соответствие схемы вида животного строковому ответу API.
    /// </summary>
    [Fact]
    public async Task OpenApi_DescribesSpeciesAsStrings()
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();
        string[] expectedSpecies =
        [
            "Cat", "Dog", "Rabbit", "Parrot", "Hamster",
            "GuineaPig", "Ferret", "Turtle", "Horse", "Snake"
        ];

        var breed = await GetJson(client, "/api/breeds/3");
        var document = await GetJson(client, "/openapi/v1.json");

        Assert.Equal("Dog", breed.GetProperty("species").GetString());
        var schema = document.GetProperty("components").GetProperty("schemas").GetProperty("AnimalSpecies");
        if (schema.TryGetProperty("type", out var type))
        {
            Assert.Equal("string", type.GetString());
        }
        var species = schema.GetProperty("enum").EnumerateArray();
        Assert.All(species, value => Assert.Equal(JsonValueKind.String, value.ValueKind));
        Assert.Equal(expectedSpecies, species.Select(value => value.GetString()));
    }

    /// <summary>
    /// Проверяет отсутствие документации вне среды разработки.
    /// </summary>
    /// <param name="path">Адрес документации.</param>
    [Theory]
    [InlineData("/openapi/v1.json")]
    [InlineData("/swagger/index.html")]
    public async Task Documentation_IsUnavailableInProduction(string path)
    {
        using var baseFactory = new ClinicApiFactory();
        using var factory = baseFactory.WithWebHostBuilder(builder => builder.UseEnvironment("Production"));
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(path, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
