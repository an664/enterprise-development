using System.Net;
using System.Text.Json;
using static VeterinaryClinic.Api.Tests.ClinicApiFactory;

namespace VeterinaryClinic.Api.Tests;

/// <summary>
/// Проверяет операции изменения и чтения сущностей через HTTP.
/// </summary>
public class CrudTests
{
    /// <summary>
    /// Проверяет создание, чтение, изменение, удаление и выдачу новых идентификаторов.
    /// </summary>
    /// <param name="resource">Имя ресурса API.</param>
    /// <param name="createJson">Данные новой записи.</param>
    /// <param name="updateJson">Данные для изменения записи.</param>
    [Theory]
    [InlineData(
        "owners",
        """
        {
            "fullName": "Новый владелец",
            "address": "Адрес 1",
            "phone": "+79990000001"
        }
        """,
        """
        {
            "fullName": "Другой владелец",
            "address": "Адрес 2",
            "phone": "+79990000002"
        }
        """)]
    [InlineData(
        "breeds",
        """
        {
            "name": "Новая порода",
            "species": "Cat"
        }
        """,
        """
        {
            "name": "Другая порода",
            "species": "Dog"
        }
        """)]
    [InlineData(
        "pets",
        """
        {
            "name": "Новый питомец",
            "breedId": 1,
            "ownerId": 1,
            "birthDate": "2020-01-01",
            "weight": 4.5
        }
        """,
        """
        {
            "name": "Другой питомец",
            "breedId": 3,
            "ownerId": 2,
            "birthDate": "2021-02-03",
            "weight": 5.5
        }
        """)]
    [InlineData(
        "specializations",
        """
        {
            "name": "Новая специализация",
            "species": "Cat"
        }
        """,
        """
        {
            "name": "Другая специализация",
            "species": "Dog"
        }
        """)]
    [InlineData(
        "veterinarians",
        """
        {
            "passportNumber": "НОВЫЙ-1",
            "fullName": "Новый врач",
            "birthYear": 1990,
            "specializationId": 1,
            "experienceYears": 5
        }
        """,
        """
        {
            "passportNumber": "НОВЫЙ-2",
            "fullName": "Другой врач",
            "birthYear": 1991,
            "specializationId": 3,
            "experienceYears": 6
        }
        """)]
    [InlineData(
        "appointments",
        """
        {
            "petId": 1,
            "veterinarianId": 1,
            "startsAt": "2026-09-16T10:00:00",
            "roomNumber": "101А",
            "isFollowUp": false
        }
        """,
        """
        {
            "petId": 2,
            "veterinarianId": 2,
            "startsAt": "2026-09-17T11:30:00",
            "roomNumber": "102",
            "isFollowUp": true
        }
        """)]
    public async Task Entity_CompletesCrud_WithoutReusingId(string resource, string createJson, string updateJson)
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();
        var path = $"/api/{resource}";
        var initialIds = Ids(await GetJson(client, path));

        using var createdResponse = await client.PostAsync(
            path,
            Json(createJson),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        var created = await ReadJson(createdResponse);
        var id = created.GetProperty("id").GetInt32();
        Assert.True(id > initialIds.Max());
        AssertFields(created, createJson);
        var location = Assert.IsType<Uri>(createdResponse.Headers.Location);
        Assert.EndsWith($"{path}/{id}", location.ToString());
        AssertFields(await GetJson(client, location.ToString()), createJson);
        Assert.Equal(initialIds.Append(id), Ids(await GetJson(client, path)));

        using var updatedResponse = await client.PutAsync(
            $"{path}/{id}",
            Json(updateJson),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, updatedResponse.StatusCode);
        var updated = await GetJson(client, $"{path}/{id}");
        Assert.Equal(id, updated.GetProperty("id").GetInt32());
        AssertFields(updated, updateJson);

        using var deletedResponse = await client.DeleteAsync(
            $"{path}/{id}",
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, deletedResponse.StatusCode);
        using var missingResponse = await client.GetAsync(
            $"{path}/{id}",
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);
        using var missingUpdate = await client.PutAsync(
            $"{path}/{id}",
            Json(updateJson),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, missingUpdate.StatusCode);
        using var repeatedDelete = await client.DeleteAsync(
            $"{path}/{id}",
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, repeatedDelete.StatusCode);
        Assert.Equal(initialIds, Ids(await GetJson(client, path)));

        using var recreatedResponse = await client.PostAsync(
            path,
            Json(createJson),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, recreatedResponse.StatusCode);
        Assert.True((await ReadJson(recreatedResponse)).GetProperty("id").GetInt32() > id);
    }

    /// <summary>
    /// Проверяет защиту используемых записей от удаления.
    /// </summary>
    /// <param name="resource">Имя ресурса API.</param>
    [Theory]
    [InlineData("owners")]
    [InlineData("breeds")]
    [InlineData("pets")]
    [InlineData("specializations")]
    [InlineData("veterinarians")]
    public async Task ReferencedEntity_DeleteReturnsConflict_AndPreservesEntity(string resource)
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();
        var path = $"/api/{resource}/1";
        var before = await GetJson(client, path);

        using var response = await client.DeleteAsync(path, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(before.GetRawText(), (await GetJson(client, path)).GetRawText());
    }

    /// <summary>
    /// Проверяет, что неудачное изменение не затрагивает существующую запись.
    /// </summary>
    /// <param name="resource">Имя ресурса API.</param>
    /// <param name="body">Некорректные данные изменения.</param>
    /// <param name="status">Ожидаемый статус ответа.</param>
    [Theory]
    [InlineData(
        "owners",
        """
        {
            "fullName": "Изменено",
            "address": "Адрес",
            "phone": "invalid"
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "breeds",
        """
        {
            "name": "Изменено",
            "species": 999
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "pets",
        """
        {
            "name": "Изменено",
            "breedId": 999,
            "ownerId": 2,
            "birthDate": "2020-01-01",
            "weight": 5
        }
        """,
        HttpStatusCode.NotFound)]
    [InlineData(
        "pets",
        """
        {
            "name": "Изменено",
            "breedId": 2,
            "ownerId": 999,
            "birthDate": "2020-01-01",
            "weight": 5
        }
        """,
        HttpStatusCode.NotFound)]
    [InlineData(
        "specializations",
        """
        {
            "name": "Изменено",
            "species": 999
        }
        """,
        HttpStatusCode.BadRequest)]
    [InlineData(
        "veterinarians",
        """
        {
            "passportNumber": "Изменено",
            "fullName": "Изменено",
            "birthYear": 1990,
            "specializationId": 999,
            "experienceYears": 5
        }
        """,
        HttpStatusCode.NotFound)]
    [InlineData(
        "appointments",
        """
        {
            "petId": 999,
            "veterinarianId": 2,
            "startsAt": "2026-09-16T10:00:00",
            "roomNumber": "Изменено"
        }
        """,
        HttpStatusCode.NotFound)]
    [InlineData(
        "appointments",
        """
        {
            "petId": 2,
            "veterinarianId": 999,
            "startsAt": "2026-09-16T10:00:00",
            "roomNumber": "Изменено"
        }
        """,
        HttpStatusCode.NotFound)]
    [InlineData(
        "pets",
        """
        {
            "name": "Изменено",
            "breedId": 2,
            "ownerId": 2,
            "birthDate": "2026-09-16",
            "weight": 5
        }
        """,
        HttpStatusCode.BadRequest)]
    public async Task InvalidUpdate_DoesNotChangeEntity(string resource, string body, HttpStatusCode status)
    {
        using var factory = new ClinicApiFactory();
        using var client = factory.CreateClient();
        var path = $"/api/{resource}/1";
        var before = await GetJson(client, path);

        using var response = await client.PutAsync(path, Json(body), TestContext.Current.CancellationToken);

        Assert.Equal(status, response.StatusCode);
        Assert.Equal(before.GetRawText(), (await GetJson(client, path)).GetRawText());
    }

    /// <summary>
    /// Сравнивает поля запроса с сохранённой записью.
    /// </summary>
    /// <param name="actual">Сохранённая запись.</param>
    /// <param name="request">JSON исходного запроса.</param>
    private static void AssertFields(JsonElement actual, string request)
    {
        using var document = JsonDocument.Parse(request);
        foreach (var property in document.RootElement.EnumerateObject())
        {
            var actualValue = actual.GetProperty(property.Name);
            Assert.Equal(property.Value.ValueKind, actualValue.ValueKind);
            Assert.Equal(property.Value.ToString(), actualValue.ToString());
        }
    }
}
