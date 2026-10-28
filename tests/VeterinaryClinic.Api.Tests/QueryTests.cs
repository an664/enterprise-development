using System.Net;
using System.Text.Json;
using static VeterinaryClinic.Api.Tests.ClinicApiFactory;

namespace VeterinaryClinic.Api.Tests;

/// <summary>
/// Проверяет связанные данные и аналитические запросы API.
/// </summary>
public class QueryTests
{
    /// <summary>
    /// Проверяет все маршруты чтения связанных сущностей.
    /// </summary>
    /// <param name="path">Маршрут связанных записей.</param>
    /// <param name="expectedIds">Ожидаемые идентификаторы в порядке ответа.</param>
    [Theory]
    [InlineData("owners/1/pets", new[] { 1, 3 })]
    [InlineData("breeds/1/pets", new[] { 1, 2 })]
    [InlineData("breeds/by-species/Dog", new[] { 3, 4 })]
    [InlineData("pets/1/appointments", new[] { 1, 3, 15, 16 })]
    [InlineData("pets/1/owner", new[] { 1 })]
    [InlineData("pets/1/breed", new[] { 1 })]
    [InlineData("specializations/1/veterinarians", new[] { 1 })]
    [InlineData("veterinarians/1/appointments", new[] { 1, 2, 3, 4, 14, 15, 16 })]
    [InlineData("veterinarians/1/specialization", new[] { 1 })]
    [InlineData("appointments/1/pet", new[] { 1 })]
    [InlineData("appointments/1/veterinarian", new[] { 1 })]
    public async Task RelatedEntities_ReturnExpectedRecords(string path, int[] expectedIds)
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();

        var result = await GetJson(client, $"/api/{path}");

        var actualIds = result.ValueKind == JsonValueKind.Array
            ? Ids(result)
            : [result.GetProperty("id").GetInt32()];
        Assert.Equal(expectedIds, actualIds);
    }

    /// <summary>
    /// Проверяет фильтрацию, порядок и отсутствие повторов в аналитике.
    /// </summary>
    /// <param name="query">Маршрут и параметры аналитического запроса.</param>
    /// <param name="expectedIds">Ожидаемые идентификаторы в порядке ответа.</param>
    [Theory]
    [InlineData("veterinarians?species=Cat", new[] { 1, 2 })]
    [InlineData("veterinarians?species=Dog", new[] { 3, 4 })]
    [InlineData("veterinarians?species=Rabbit", new[] { 5 })]
    [InlineData("veterinarians?species=Snake", new int[] { })]
    [InlineData("pets?veterinarianId=1", new[] { 1, 2, 13, 3 })]
    [InlineData("owners-with-multiple-pets", new[] { 2, 3, 1 })]
    [InlineData("appointments?asOfDate=2026-09-15&roomNumber=101А", new[] { 15, 1, 2 })]
    [InlineData("appointments?asOfDate=2026-09-15&roomNumber=102", new[] { 3, 5 })]
    [InlineData("appointments?asOfDate=2026-10-01&roomNumber=101А", new[] { 2, 16, 4 })]
    [InlineData("appointments?asOfDate=2026-12-31&roomNumber=102", new[] { 10, 8 })]
    [InlineData("appointments?asOfDate=2026-01-31&roomNumber=101А", new[] { 9, 6 })]
    [InlineData("appointments?asOfDate=2024-01-31&roomNumber=101А", new[] { 17, 11 })]
    [InlineData("appointments?asOfDate=2026-11-01&roomNumber=101А", new int[] { })]
    public async Task Analytics_ReturnExpectedIds(string query, int[] expectedIds)
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();

        var result = await GetJson(client, $"/api/analytics/{query}");

        Assert.Equal(expectedIds, Ids(result));
    }

    /// <summary>
    /// Проверяет количество повторных приёмов по породе.
    /// </summary>
    /// <param name="breedId">Идентификатор породы.</param>
    /// <param name="expectedCount">Ожидаемое количество повторных приёмов.</param>
    [Theory]
    [InlineData(1, 4)]
    [InlineData(2, 1)]
    [InlineData(10, 1)]
    [InlineData(5, 0)]
    public async Task FollowUpCount_ReturnsExpectedCount(int breedId, int expectedCount)
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();

        var result = await GetJson(client, $"/api/analytics/follow-up-count?breedId={breedId}");

        Assert.Equal(breedId, result.GetProperty("breedId").GetInt32());
        Assert.Equal(expectedCount, result.GetProperty("count").GetInt32());
    }

    /// <summary>
    /// Проверяет обновление владельца, породы и ссылок питомца во всех связанных данных.
    /// </summary>
    [Fact]
    public async Task OwnerBreedAndPetUpdates_AreVisibleThroughExistingRelationships()
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();
        int[] expectedPreviousOwnerPets = [3];
        int[] expectedOwnerPets = [1, 2, 13];
        int[] expectedBreedPets = [1, 4];
        int[] expectedOwnersWithMultiplePets = [2, 3];
        int[] expectedVeterinarianPets = [2, 13, 3, 1];

        using var ownerUpdate = await client.PutAsync(
            "/api/owners/1",
            Json("""
                {
                    "fullName": "Изменённый владелец",
                    "address": "Новый адрес",
                    "phone": "+79990000001"
                }
                """),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, ownerUpdate.StatusCode);
        var petOwner = await GetJson(client, "/api/pets/1/owner");
        Assert.Equal("Изменённый владелец", petOwner.GetProperty("fullName").GetString());

        using var breedUpdate = await client.PutAsync(
            "/api/breeds/1",
            Json("""
                {
                    "name": "Изменённая порода",
                    "species": "Dog"
                }
                """),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, breedUpdate.StatusCode);
        var petBreed = await GetJson(client, "/api/pets/1/breed");
        var pet = await GetJson(client, "/api/pets/1");
        Assert.Equal("Изменённая порода", petBreed.GetProperty("name").GetString());
        Assert.Equal("Dog", pet.GetProperty("species").GetString());

        using var petUpdate = await client.PutAsync(
            "/api/pets/1",
            Json("""
                {
                    "name": "Обновлённый питомец",
                    "breedId": 3,
                    "ownerId": 2,
                    "birthDate": "2020-01-01",
                    "weight": 4.5
                }
                """),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, petUpdate.StatusCode);
        Assert.Equal(expectedPreviousOwnerPets, Ids(await GetJson(client, "/api/owners/1/pets")));
        Assert.Equal(expectedOwnerPets, Ids(await GetJson(client, "/api/owners/2/pets")));
        Assert.Equal(expectedBreedPets, Ids(await GetJson(client, "/api/breeds/3/pets")));
        var appointmentPet = await GetJson(client, "/api/appointments/1/pet");
        Assert.Equal("Обновлённый питомец", appointmentPet.GetProperty("name").GetString());
        Assert.Equal(2, appointmentPet.GetProperty("ownerId").GetInt32());
        Assert.Equal(3, appointmentPet.GetProperty("breedId").GetInt32());
        var owners = await GetJson(client, "/api/analytics/owners-with-multiple-pets");
        var veterinarianPets = await GetJson(client, "/api/analytics/pets?veterinarianId=1");
        var followUpCount = await GetJson(client, "/api/analytics/follow-up-count?breedId=1");
        Assert.Equal(expectedOwnersWithMultiplePets, Ids(owners));
        Assert.Equal(expectedVeterinarianPets, Ids(veterinarianPets));
        Assert.Equal(1, followUpCount.GetProperty("count").GetInt32());
    }

    /// <summary>
    /// Проверяет обновление специализации, врача и ссылок приёма.
    /// </summary>
    [Fact]
    public async Task SpecializationVeterinarianAndAppointmentUpdates_PreserveRelationships()
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();
        int[] expectedDogVeterinarians = [1, 3, 4];
        int[] expectedSpecializationVeterinarians = [1, 3];
        int[] expectedPetAppointments = [3, 15, 16];
        int[] expectedUpdatedPetAppointments = [1, 2];
        int[] expectedVeterinarianAppointments = [1, 13];

        using var specializationUpdate = await client.PutAsync(
            "/api/specializations/1",
            Json("""
                {
                    "name": "Обновлённая специализация",
                    "species": "Dog"
                }
                """),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, specializationUpdate.StatusCode);
        var specialization = await GetJson(client, "/api/veterinarians/1/specialization");
        var dogVeterinarians = await GetJson(client, "/api/analytics/veterinarians?species=Dog");
        Assert.Equal("Обновлённая специализация", specialization.GetProperty("name").GetString());
        Assert.Equal(expectedDogVeterinarians, Ids(dogVeterinarians));

        using var veterinarianUpdate = await client.PutAsync(
            "/api/veterinarians/1",
            Json("""
                {
                    "passportNumber": "ТЕСТ-ИЗМЕНЁН",
                    "fullName": "Обновлённый врач",
                    "birthYear": 1990,
                    "specializationId": 3,
                    "experienceYears": 5
                }
                """),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, veterinarianUpdate.StatusCode);
        Assert.Empty(Ids(await GetJson(client, "/api/specializations/1/veterinarians")));
        var specializationVeterinarians = await GetJson(client, "/api/specializations/3/veterinarians");
        Assert.Equal(expectedSpecializationVeterinarians, Ids(specializationVeterinarians));
        var veterinarian = await GetJson(client, "/api/appointments/1/veterinarian");
        Assert.Equal("Обновлённый врач", veterinarian.GetProperty("fullName").GetString());

        using var appointmentUpdate = await client.PutAsync(
            "/api/appointments/1",
            Json("""
                {
                    "petId": 2,
                    "veterinarianId": 2,
                    "startsAt": "2026-09-16T10:00:00",
                    "roomNumber": "102",
                    "isFollowUp": true
                }
                """),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, appointmentUpdate.StatusCode);
        Assert.Equal(expectedPetAppointments, Ids(await GetJson(client, "/api/pets/1/appointments")));
        Assert.Equal(expectedUpdatedPetAppointments, Ids(await GetJson(client, "/api/pets/2/appointments")));
        var veterinarianAppointments = await GetJson(client, "/api/veterinarians/2/appointments");
        var appointmentPet = await GetJson(client, "/api/appointments/1/pet");
        var appointmentVeterinarian = await GetJson(client, "/api/appointments/1/veterinarian");
        Assert.Equal(expectedVeterinarianAppointments, Ids(veterinarianAppointments));
        Assert.Equal(2, appointmentPet.GetProperty("id").GetInt32());
        Assert.Equal(2, appointmentVeterinarian.GetProperty("id").GetInt32());
    }

    /// <summary>
    /// Проверяет отражение создания, изменения и удаления в аналитике.
    /// </summary>
    [Fact]
    public async Task AppointmentChanges_AreVisibleInAnalytics()
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();
        int[] expectedOriginalIds = [15, 1, 2];
        const string countPath = "/api/analytics/follow-up-count?breedId=1";
        const string appointmentsPath = "/api/analytics/appointments?asOfDate=2026-09-15&roomNumber=101А";

        using var create = await client.PostAsync(
            "/api/appointments",
            Json("""
                {
                    "petId": 1,
                    "veterinarianId": 1,
                    "startsAt": "2026-09-16T10:00:00",
                    "roomNumber": "101А",
                    "isFollowUp": true
                }
                """),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var id = (await ReadJson(create)).GetProperty("id").GetInt32();
        int[] expectedCreatedIds = [15, id, 1, 2];
        Assert.Equal(5, (await GetJson(client, countPath)).GetProperty("count").GetInt32());
        Assert.Equal(expectedCreatedIds, Ids(await GetJson(client, appointmentsPath)));

        using var update = await client.PutAsync(
            $"/api/appointments/{id}",
            Json("""
                {
                    "petId": 1,
                    "veterinarianId": 1,
                    "startsAt": "2026-10-15T00:00:00",
                    "roomNumber": "101А",
                    "isFollowUp": false
                }
                """),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, update.StatusCode);
        Assert.Equal(4, (await GetJson(client, countPath)).GetProperty("count").GetInt32());
        Assert.Equal(expectedOriginalIds, Ids(await GetJson(client, appointmentsPath)));

        using var delete = await client.DeleteAsync(
            $"/api/appointments/{id}",
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Equal(4, (await GetJson(client, countPath)).GetProperty("count").GetInt32());
        Assert.Equal(expectedOriginalIds, Ids(await GetJson(client, appointmentsPath)));
    }
}
