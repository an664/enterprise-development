using VeterinaryClinic.Models;

namespace VeterinaryClinic.Tests;

/// <summary>
/// Проверяет запросы к данным ветеринарной клиники.
/// </summary>
public class ClinicQueryTests(ClinicFixture fixture) : IClassFixture<ClinicFixture>
{
    /// <summary>
    /// Проверяет наличие минимум десяти экземпляров каждой сущности.
    /// </summary>
    [Fact]
    public void SeedContainsAtLeastTenInstancesOfEveryDomainClass()
    {
        var data = fixture.Data;

        Assert.True(data.Breeds.Count >= 10);
        Assert.True(data.Owners.Count >= 10);
        Assert.True(data.Pets.Count >= 10);
        Assert.True(data.Specializations.Count >= 10);
        Assert.True(data.Veterinarians.Count >= 10);
        Assert.True(data.Appointments.Count >= 10);
    }

    /// <summary>
    /// Проверяет отбор врачей по виду животных.
    /// </summary>
    [Theory]
    [InlineData(AnimalSpecies.Cat, new[] { 1, 2 })]
    [InlineData(AnimalSpecies.Dog, new[] { 3, 4 })]
    [InlineData(AnimalSpecies.Rabbit, new[] { 5 })]
    [InlineData(AnimalSpecies.Snake, new int[] { })]
    public void VeterinariansAreFilteredByTheSelectedSpecies(
        AnimalSpecies species, int[] expectedIds)
    {
        var data = fixture.Data;

        var veterinarians = data.Veterinarians
            .Where(veterinarian => veterinarian.Specialization.Species == species)
            .ToArray();

        Assert.Equal(expectedIds, veterinarians.Select(veterinarian => veterinarian.Id));
    }

    /// <summary>
    /// Проверяет отбор питомцев врача без повторов и сортировку по кличке.
    /// </summary>
    [Fact]
    public void PetsOfAVeterinarianAreDistinctAndSortedByName()
    {
        var data = fixture.Data;
        const int veterinarianId = 1;
        var expectedIds = new[] { 1, 2, 13, 3 };
        var expectedNames = new[] { "Арчи", "Ася", "Ася", "Барсик" };

        var pets = data.Appointments
            .Where(appointment => appointment.Veterinarian.Id == veterinarianId)
            .Select(appointment => appointment.Pet)
            .DistinctBy(pet => pet.Id)
            .OrderBy(pet => pet.Name, StringComparer.Ordinal)
            .ThenBy(pet => pet.Id)
            .ToArray();

        Assert.Equal(expectedIds, pets.Select(pet => pet.Id));
        Assert.Equal(expectedNames, pets.Select(pet => pet.Name));
    }

    /// <summary>
    /// Проверяет число повторных приёмов питомцев выбранной породы.
    /// </summary>
    [Theory]
    [InlineData(1, 4)]
    [InlineData(2, 1)]
    [InlineData(10, 1)]
    [InlineData(5, 0)]
    [InlineData(999, 0)]
    public void FollowUpCountIncludesEachVisitOfOnlyTheSelectedBreed(
        int breedId, int expectedCount)
    {
        var data = fixture.Data;

        var count = data.Appointments.Count(appointment =>
            appointment.IsFollowUp && appointment.Pet.Breed.Id == breedId);

        Assert.Equal(expectedCount, count);
    }

    /// <summary>
    /// Проверяет отбор владельцев нескольких питомцев и сортировку по ФИО.
    /// </summary>
    [Fact]
    public void OwnersWithMultiplePetsAreSortedByFullName()
    {
        var data = fixture.Data;
        var expectedIds = new[] { 2, 3, 1 };
        var expectedNames = new[]
        {
            "Алексеева Вера Павловна", "Иванов Борис Сергеевич", "Яковлев Артём Ильич"
        };

        var owners = data.Owners
            .Where(owner => data.Pets.Count(pet => pet.Owner.Id == owner.Id) > 1)
            .OrderBy(owner => owner.FullName, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expectedIds, owners.Select(owner => owner.Id));
        Assert.Equal(expectedNames, owners.Select(owner => owner.FullName));
    }

    /// <summary>
    /// Проверяет приёмы выбранного кабинета за месяц от указанной даты.
    /// </summary>
    [Theory]
    [InlineData(2026, 9, 15, "101А", new[] { 15, 1, 2 })]
    [InlineData(2026, 9, 15, "102", new[] { 3, 5 })]
    [InlineData(2026, 10, 1, "101А", new[] { 2, 16, 4 })]
    [InlineData(2026, 12, 31, "102", new[] { 10, 8 })]
    [InlineData(2026, 1, 31, "101А", new[] { 9, 6 })]
    [InlineData(2024, 1, 31, "101А", new[] { 17, 11 })]
    [InlineData(2026, 11, 1, "101А", new int[] { })]
    public void AppointmentsUseTheNextMonthFromTheSelectedDateAndRoom(
        int year, int month, int day, string roomNumber, int[] expectedIds)
    {
        var asOfDate = new DateOnly(year, month, day);
        // Даты приёмов фиксированы; меняется только период отбора.
        var data = fixture.Data;
        var periodStart = asOfDate.ToDateTime(TimeOnly.MinValue);
        var periodEnd = periodStart.AddMonths(1);

        var appointments = data.Appointments
            .Where(appointment => appointment.StartsAt >= periodStart
                && appointment.StartsAt < periodEnd
                && appointment.RoomNumber == roomNumber)
            .OrderBy(appointment => appointment.StartsAt)
            .ToArray();

        Assert.Equal(expectedIds, appointments.Select(appointment => appointment.Id));
    }
}
