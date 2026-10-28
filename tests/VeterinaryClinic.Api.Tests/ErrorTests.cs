using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VeterinaryClinic.Application.Services;
using VeterinaryClinic.Contracts.Owners;
using VeterinaryClinic.Contracts.Pets;
using static VeterinaryClinic.Api.Tests.ClinicApiFactory;

namespace VeterinaryClinic.Api.Tests;

/// <summary>
/// Проверяет ошибки запросов и сохранность данных при отказе.
/// </summary>
public class ErrorTests
{
    /// <summary>
    /// Проверяет отклонение неверных значений и отсутствующих связанных записей.
    /// </summary>
    /// <param name="resource">Имя ресурса API.</param>
    /// <param name="body">Некорректные данные новой записи.</param>
    /// <param name="status">Ожидаемый статус ответа.</param>
    [Theory]
    [InlineData(
        "owners",
        """
        {
            "fullName": " ",
            "address": "Адрес",
            "phone": "+79990000001"
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "owners",
        """
        {
            "fullName": "Имя",
            "address": "Адрес",
            "phone": "invalid"
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "breeds",
        """
        {
            "name": "Порода",
            "species": 999
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "specializations",
        """
        {
            "name": "Специализация",
            "species": "Unknown"
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "pets",
        """
        {
            "name": "Питомец",
            "breedId": 1,
            "ownerId": 1,
            "weight": 4.5
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "pets",
        """
        {
            "name": "Питомец",
            "breedId": 1,
            "ownerId": 1,
            "birthDate": "0001-01-01",
            "weight": 4.5
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "pets",
        """
        {
            "name": "Питомец",
            "breedId": 1,
            "ownerId": 1,
            "birthDate": "2020-01-01",
            "weight": 0
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "pets",
        """
        {
            "name": "Питомец",
            "breedId": 1,
            "ownerId": 1,
            "birthDate": "2020-01-01",
            "weight": "4.5"
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "pets",
        """
        {
            "name": "Питомец",
            "breedId": 0,
            "ownerId": 1,
            "birthDate": "2020-01-01",
            "weight": 4.5
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "pets",
        """
        {
            "name": "Питомец",
            "breedId": 999,
            "ownerId": 1,
            "birthDate": "2020-01-01",
            "weight": 4.5
        }
        """,
        HttpStatusCode.NotFound)]
    [InlineData(
        "pets",
        """
        {
            "name": "Питомец",
            "breedId": 1,
            "ownerId": 999,
            "birthDate": "2020-01-01",
            "weight": 4.5
        }
        """,
        HttpStatusCode.NotFound)]
    [InlineData(
        "veterinarians",
        """
        {
            "passportNumber": "ТЕСТ",
            "fullName": "Врач",
            "birthYear": 9999,
            "specializationId": 1
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "veterinarians",
        """
        {
            "passportNumber": "ТЕСТ",
            "fullName": "Врач",
            "birthYear": 1990,
            "specializationId": 1,
            "experienceYears": 150
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "veterinarians",
        """
        {
            "passportNumber": "ТЕСТ",
            "fullName": "Врач",
            "birthYear": 1990,
            "specializationId": 999
        }
        """,
        HttpStatusCode.NotFound)]
    [InlineData(
        "appointments",
        """
        {
            "petId": 1,
            "veterinarianId": 1,
            "roomNumber": "101А"
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "appointments",
        """
        {
            "petId": 1,
            "veterinarianId": 1,
            "startsAt": "0001-01-01T00:00:00",
            "roomNumber": "101А"
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "appointments",
        """
        {
            "petId": 1,
            "veterinarianId": 1,
            "startsAt": "2026-09-15T10:00:00Z",
            "roomNumber": "101А"
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "appointments",
        """
        {
            "petId": 1,
            "veterinarianId": 1,
            "startsAt": "2019-12-31T23:59:59",
            "roomNumber": "101А"
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "appointments",
        """
        {
            "petId": 1,
            "veterinarianId": 1,
            "startsAt": "2026-09-15T10:00:00",
            "roomNumber": " "
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "appointments",
        """
        {
            "petId": 999,
            "veterinarianId": 1,
            "startsAt": "2026-09-15T10:00:00",
            "roomNumber": "101А"
        }
        """,
        HttpStatusCode.NotFound)]
    [InlineData(
        "appointments",
        """
        {
            "petId": 1,
            "veterinarianId": 999,
            "startsAt": "2026-09-15T10:00:00",
            "roomNumber": "101А"
        }
        """,
        HttpStatusCode.NotFound)]
    [InlineData("owners", "{}", HttpStatusCode.BadRequest)]
    [InlineData("owners", "{", HttpStatusCode.BadRequest)]
    public async Task InvalidCreate_ReturnsProblem_WithoutChangingList(
        string resource,
        string body,
        HttpStatusCode status)
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();
        var path = $"/api/{resource}";
        var before = await GetJson(client, path);

        using var response = await client.PostAsync(path, Json(body), TestContext.Current.CancellationToken);

        Assert.Equal(status, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await ReadJson(response);
        Assert.Equal((int)status, problem.GetProperty("status").GetInt32());
        if (status == HttpStatusCode.BadRequest)
        {
            Assert.NotEmpty(problem.GetProperty("errors").EnumerateObject());
        }
        Assert.Equal(before.GetRawText(), (await GetJson(client, path)).GetRawText());
    }

    /// <summary>
    /// Проверяет ошибки параметров маршрута и аналитических запросов.
    /// </summary>
    /// <param name="path">Маршрут с некорректными параметрами.</param>
    [Theory]
    [InlineData("owners/0")]
    [InlineData("breeds/by-species/999")]
    [InlineData("analytics/veterinarians")]
    [InlineData("analytics/veterinarians?species=999")]
    [InlineData("analytics/pets")]
    [InlineData("analytics/pets?veterinarianId=-1")]
    [InlineData("analytics/follow-up-count")]
    [InlineData("analytics/follow-up-count?breedId=0")]
    [InlineData("analytics/appointments?roomNumber=101А")]
    [InlineData("analytics/appointments?asOfDate=2026-09-15")]
    [InlineData("analytics/appointments?asOfDate=invalid&roomNumber=101А")]
    [InlineData("analytics/appointments?asOfDate=0001-01-01&roomNumber=101А")]
    [InlineData("analytics/appointments?asOfDate=9999-12-01&roomNumber=101А")]
    public async Task InvalidQuery_ReturnsBadRequest(string path)
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync($"/api/{path}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.NotEmpty((await ReadJson(response)).GetProperty("errors").EnumerateObject());
    }

    /// <summary>
    /// Проверяет ответы для отсутствующей основной или связанной сущности.
    /// </summary>
    /// <param name="path">Маршрут отсутствующей записи.</param>
    [Theory]
    [InlineData("owners/999")]
    [InlineData("breeds/999")]
    [InlineData("pets/999")]
    [InlineData("specializations/999")]
    [InlineData("veterinarians/999")]
    [InlineData("appointments/999")]
    [InlineData("owners/999/pets")]
    [InlineData("breeds/999/pets")]
    [InlineData("pets/999/appointments")]
    [InlineData("pets/999/owner")]
    [InlineData("pets/999/breed")]
    [InlineData("specializations/999/veterinarians")]
    [InlineData("veterinarians/999/appointments")]
    [InlineData("veterinarians/999/specialization")]
    [InlineData("appointments/999/pet")]
    [InlineData("appointments/999/veterinarian")]
    [InlineData("analytics/pets?veterinarianId=999")]
    [InlineData("analytics/follow-up-count?breedId=999")]
    public async Task MissingEntity_ReturnsNotFound(string path)
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync($"/api/{path}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    /// <summary>
    /// Проверяет допустимое числовое значение перечисления и его строковую выдачу.
    /// </summary>
    [Fact]
    public async Task DefinedNumericSpecies_IsAcceptedAndReturnedAsString()
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.PostAsync(
            "/api/breeds",
            Json("""
                {
                    "name": "Новая порода",
                    "species": 1
                }
                """),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Dog", (await ReadJson(response)).GetProperty("species").GetString());
    }

    /// <summary>
    /// Проверяет границу десятичного веса при разных региональных настройках.
    /// </summary>
    /// <param name="cultureName">Проверяемые региональные настройки.</param>
    [Theory]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task WeightValidation_IsIndependentOfCulture(string cultureName)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
            using var factory = new ClinicApiFactory();
            factory.Server.PreserveExecutionContext = true;
            using var client = factory.CreateClient();

            using var valid = await client.PostAsync(
                "/api/pets",
                Json("""
                    {
                        "name": "Питомец",
                        "breedId": 1,
                        "ownerId": 1,
                        "birthDate": "2020-01-01",
                        "weight": 0.001
                    }
                    """),
                TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Created, valid.StatusCode);
            Assert.Equal(0.001m, (await ReadJson(valid)).GetProperty("weight").GetDecimal());
            using var invalid = await client.PostAsync(
                "/api/pets",
                Json("""
                    {
                        "name": "Питомец",
                        "breedId": 1,
                        "ownerId": 1,
                        "birthDate": "2020-01-01",
                        "weight": 0.0009
                    }
                    """),
                TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    /// <summary>
    /// Проверяет, что внутренняя ошибка не раскрывает исключение или стек вызовов.
    /// </summary>
    [Fact]
    public async Task UnexpectedException_ReturnsSanitizedProblem()
    {
        using var baseFactory = new ClinicApiFactory();
        using var factory = baseFactory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IOwnerService>();
            services.AddScoped<IOwnerService>(_ => new ThrowingOwnerService());
        }));
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/owners", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var text = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.DoesNotContain("sensitive-marker", text);
        Assert.DoesNotContain("InvalidOperationException", text);
        Assert.DoesNotContain("stack", text, StringComparison.OrdinalIgnoreCase);
        var problem = await ReadJson(response);
        Assert.Equal(500, problem.GetProperty("status").GetInt32());
        Assert.False(problem.TryGetProperty("detail", out _));
    }

    /// <summary>
    /// Воспроизводит неожиданный сбой службы владельцев.
    /// </summary>
    private class ThrowingOwnerService : IOwnerService
    {
        /// <inheritdoc />
        public IReadOnlyList<OwnerDto> GetAll()
        {
            throw new InvalidOperationException("sensitive-marker");
        }

        /// <inheritdoc />
        public OwnerDto GetById(int id)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public OwnerDto Create(OwnerRequest request)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public void Update(int id, OwnerRequest request)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public void Delete(int id)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IReadOnlyList<PetDto> GetPets(int id)
        {
            throw new NotSupportedException();
        }
    }
}
