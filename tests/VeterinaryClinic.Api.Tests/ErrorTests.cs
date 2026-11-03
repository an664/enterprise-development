using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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
    /// Отклоняет запись на занятое время, но разрешает менять данные самого приёма.
    /// </summary>
    [Fact]
    public async Task Appointment_RejectsOccupiedRoomWithoutChangingExistingVisit()
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();
        var token = TestContext.Current.CancellationToken;
        var original = await GetJson(client, "/api/appointments/2");
        var occupied = new
        {
            PetId = 2,
            VeterinarianId = 2,
            RoomNumber = " 101А ",
            StartsAt = new DateTime(2026, 9, 20, 9, 0, 0)
        };

        using var create = await client.PostAsJsonAsync("/api/appointments", occupied, token);
        Assert.Equal(HttpStatusCode.Conflict, create.StatusCode);

        using var move = await client.PutAsJsonAsync("/api/appointments/2", occupied, token);
        Assert.Equal(HttpStatusCode.Conflict, move.StatusCode);
        Assert.True(JsonElement.DeepEquals(original, await GetJson(client, "/api/appointments/2")));

        using var edit = await client.PutAsJsonAsync("/api/appointments/1", occupied, token);
        Assert.Equal(HttpStatusCode.NoContent, edit.StatusCode);
    }

    /// <summary>
    /// Создаёт и находит приём по номеру предельной длины с пробелами по краям.
    /// </summary>
    [Fact]
    public async Task RoomNumber_IgnoresOuterSpacesAtLengthLimit()
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();
        var token = TestContext.Current.CancellationToken;
        var roomNumber = new string('А', 20);
        var request = new
        {
            PetId = 1,
            VeterinarianId = 1,
            StartsAt = new DateTime(2027, 4, 18, 10, 0, 0),
            RoomNumber = $" {roomNumber} "
        };

        using var create = await client.PostAsJsonAsync("/api/appointments", request, token);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await ReadJson(create);
        Assert.Equal(roomNumber, created.GetProperty("roomNumber").GetString());

        var encodedRoom = Uri.EscapeDataString(request.RoomNumber);
        var appointments = await GetJson(client, $"/api/analytics/appointments?asOfDate=2027-04-01&roomNumber={encodedRoom}");
        var foundId = Assert.Single(Ids(appointments));
        Assert.Equal(created.GetProperty("id").GetInt32(), foundId);
    }

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
            "fullName": "Имя\u0000",
            "address": "Адрес",
            "phone": "+79990000001"
        }
        """,
        HttpStatusCode.BadRequest)]
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
            "roomNumber": "302",
            "startsAt": "2027-04-18T10:00:00.1234567"
        }
        """,
        HttpStatusCode.BadRequest)]
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
            "roomNumber": "123456789012345678901"
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
    [InlineData("breeds/by-species/Dog%2C%20Rabbit")]
    [InlineData("analytics/veterinarians?species=Dog%2C%20Rabbit")]
    [InlineData("analytics/veterinarians")]
    [InlineData("analytics/veterinarians?species=999")]
    [InlineData("analytics/pets")]
    [InlineData("analytics/pets?veterinarianId=-1")]
    [InlineData("analytics/follow-up-count")]
    [InlineData("analytics/follow-up-count?breedId=0")]
    [InlineData("analytics/appointments?roomNumber=101А")]
    [InlineData("analytics/appointments?asOfDate=2026-09-15")]
    [InlineData("analytics/appointments?asOfDate=2026-09-15&roomNumber=%20%20")]
    [InlineData("analytics/appointments?asOfDate=2026-09-15&roomNumber=123456789012345678901")]
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
    /// Не позволяет записать сочетание видов как третье значение перечисления.
    /// </summary>
    /// <param name="resource">Ресурс со свойством вида животного.</param>
    [Theory]
    [InlineData("breeds")]
    [InlineData("specializations")]
    public async Task MultipleSpecies_ReturnsBadRequest(string resource)
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            $"/api/{resource}",
            new { Name = "Проверка вида", Species = "Dog, Rabbit" },
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
        public Task<IReadOnlyList<OwnerDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("sensitive-marker");
        }

        /// <inheritdoc />
        public Task<OwnerDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<OwnerDto> CreateAsync(OwnerRequest request, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task UpdateAsync(int id, OwnerRequest request, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<PetDto>> GetPetsAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
