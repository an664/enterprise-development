using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Npgsql;

namespace VeterinaryClinic.IntegrationTests;

/// <summary>
/// Проверяет API на базе PostgreSQL, созданной при запуске AppHost.
/// </summary>
/// <param name="clinic">API и база данных тестовой клиники.</param>
public class PostgresApiTests(ClinicAppFixture clinic) : IClassFixture<ClinicAppFixture>
{
    /// <summary>
    /// Проверяет фильтрацию и порядок результатов на исходных данных миграций.
    /// </summary>
    /// <param name="query">Аналитический запрос.</param>
    /// <param name="expectedIds">Идентификаторы в ожидаемом порядке.</param>
    [Theory]
    [InlineData("veterinarians?species=Cat", new[] { 1, 2 })]
    [InlineData("pets?veterinarianId=1", new[] { 1, 2, 13, 3 })]
    [InlineData("appointments?asOfDate=2026-09-15&roomNumber=%20101А%20", new[] { 15, 1, 2 })]
    public async Task Analytics_ReturnsSeededRecords(string query, int[] expectedIds)
    {
        var records = await clinic.Client.GetFromJsonAsync<JsonElement[]>(
            $"/api/analytics/{query}", TestContext.Current.CancellationToken);

        Assert.NotNull(records);
        Assert.Equal(expectedIds, records.Select(record => record.GetProperty("id").GetInt32()));
    }

    /// <summary>
    /// Проверяет количество питомцев и порядок владельцев по ФИО.
    /// </summary>
    [Fact]
    public async Task OwnersWithMultiplePets_ReturnsPetCountsInNameOrder()
    {
        int[] expectedOwnerIds = [2, 3, 1];
        const int expectedPetCount = 2;

        var owners = await clinic.Client.GetFromJsonAsync<JsonElement[]>(
            "/api/analytics/owners-with-multiple-pets", TestContext.Current.CancellationToken);

        Assert.NotNull(owners);
        Assert.Equal(expectedOwnerIds, owners.Select(owner => owner.GetProperty("id").GetInt32()));
        Assert.All(owners, owner => Assert.Equal(expectedPetCount, owner.GetProperty("petCount").GetInt32()));
    }

    /// <summary>
    /// Проверяет связь созданного питомца с владельцем и порядок их удаления.
    /// </summary>
    [Fact]
    public async Task CreatePet_PersistsOwnerRelationshipAndPreventsOwnerDeletion()
    {
        var token = TestContext.Current.CancellationToken;
        var owner = new
        {
            FullName = "Елена Смирнова",
            Address = "Самара, ул. Полевая, 10",
            Phone = "+79990000001"
        };
        using var ownerResponse = await clinic.Client.PostAsJsonAsync("/api/owners", owner, token);
        Assert.Equal(HttpStatusCode.Created, ownerResponse.StatusCode);
        var ownerId = (await ownerResponse.Content.ReadFromJsonAsync<JsonElement>(token)).GetProperty("id").GetInt32();

        var pet = new
        {
            Name = "Луна",
            BreedId = 1,
            OwnerId = ownerId,
            BirthDate = new DateOnly(2021, 5, 12),
            Weight = 4.5m
        };
        using var petResponse = await clinic.Client.PostAsJsonAsync("/api/pets", pet, token);
        Assert.Equal(HttpStatusCode.Created, petResponse.StatusCode);
        var petId = (await petResponse.Content.ReadFromJsonAsync<JsonElement>(token)).GetProperty("id").GetInt32();

        var relatedOwner = await clinic.Client.GetFromJsonAsync<JsonElement>($"/api/pets/{petId}/owner", token);
        Assert.Equal(ownerId, relatedOwner.GetProperty("id").GetInt32());

        using var conflict = await clinic.Client.DeleteAsync($"/api/owners/{ownerId}", token);
        Assert.Equal(HttpStatusCode.Conflict, conflict.StatusCode);

        await using var connection = new NpgsqlConnection(clinic.ConnectionString);
        await connection.OpenAsync(token);
        await using var query = new NpgsqlCommand("""
            SELECT o.full_name
            FROM pets AS p
            JOIN owners AS o ON o.id = p.owner_id
            WHERE p.id = @pet AND o.id = @owner
            """, connection);
        query.Parameters.AddWithValue("pet", petId);
        query.Parameters.AddWithValue("owner", ownerId);
        Assert.Equal(owner.FullName, await query.ExecuteScalarAsync(token));

        using var deletePet = await clinic.Client.DeleteAsync($"/api/pets/{petId}", token);
        Assert.Equal(HttpStatusCode.NoContent, deletePet.StatusCode);
        using var deleteOwner = await clinic.Client.DeleteAsync($"/api/owners/{ownerId}", token);
        Assert.Equal(HttpStatusCode.NoContent, deleteOwner.StatusCode);

        using var missingOwner = await clinic.Client.PostAsJsonAsync("/api/pets", pet, token);
        Assert.Equal(HttpStatusCode.NotFound, missingOwner.StatusCode);
    }

    /// <summary>
    /// Проверяет сохранение микросекунд и отклонение дополнительного тика DateTime.
    /// </summary>
    [Fact]
    public async Task AppointmentTime_RoundTripsToMicroseconds()
    {
        var token = TestContext.Current.CancellationToken;
        var appointment = new
        {
            PetId = 1,
            VeterinarianId = 1,
            RoomNumber = "302",
            StartsAt = new DateTime(2027, 4, 18, 10, 0, 0, millisecond: 123, microsecond: 456)
        };

        var tooPrecise = appointment with { StartsAt = appointment.StartsAt.AddTicks(1) };
        using var rejected = await clinic.Client.PostAsJsonAsync("/api/appointments", tooPrecise, token);
        Assert.Equal(HttpStatusCode.BadRequest, rejected.StatusCode);

        using var created = await clinic.Client.PostAsJsonAsync("/api/appointments", appointment, token);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var saved = await clinic.Client.GetFromJsonAsync<JsonElement>(created.Headers.Location, token);
        Assert.Equal(appointment.StartsAt, saved.GetProperty("startsAt").GetDateTime());

        using var delete = await clinic.Client.DeleteAsync(created.Headers.Location, token);
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
    }

    /// <summary>
    /// Проверяет, что два врача не могут одновременно занять один кабинет.
    /// </summary>
    [Fact]
    public async Task CompetingAppointments_OnlyOneCanBookTheRoom()
    {
        var token = TestContext.Current.CancellationToken;
        var appointment = new
        {
            PetId = 1,
            VeterinarianId = 1,
            RoomNumber = "301",
            StartsAt = new DateTime(2027, 4, 18, 10, 0, 0)
        };
        var otherAppointment = appointment with { PetId = 2, VeterinarianId = 2, RoomNumber = " 301 " };

        var firstRequest = clinic.Client.PostAsJsonAsync("/api/appointments", appointment, token);
        var secondRequest = clinic.Client.PostAsJsonAsync("/api/appointments", otherAppointment, token);
        using var first = await firstRequest;
        using var second = await secondRequest;

        HttpStatusCode[] statuses = [first.StatusCode, second.StatusCode];
        Assert.Contains(HttpStatusCode.Created, statuses);
        Assert.Contains(HttpStatusCode.Conflict, statuses);

        using var move = await clinic.Client.PutAsJsonAsync("/api/appointments/3", appointment, token);
        Assert.Equal(HttpStatusCode.Conflict, move.StatusCode);

        var created = first.StatusCode == HttpStatusCode.Created ? first : second;
        using var delete = await clinic.Client.DeleteAsync(created.Headers.Location, token);
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
    }

    /// <summary>
    /// Проверяет число повторных приёмов по породе в исходных данных.
    /// </summary>
    [Fact]
    public async Task FollowUpCount_CountsAppointmentsForBreed()
    {
        var result = await clinic.Client.GetFromJsonAsync<JsonElement>(
            "/api/analytics/follow-up-count?breedId=1", TestContext.Current.CancellationToken);

        Assert.Equal(4, result.GetProperty("count").GetInt32());
    }
}
