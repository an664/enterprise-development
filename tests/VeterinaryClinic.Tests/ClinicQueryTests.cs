using VeterinaryClinic.Models;

namespace VeterinaryClinic.Tests;

public sealed class ClinicQueryTests
{
    private static readonly DateOnly _asOfDate = new(2026, 9, 15);

    [Fact]
    public void SeedContainsAtLeastTenInstancesOfEveryDomainClass()
    {
        var data = ClinicSeed.Create(_asOfDate);

        Assert.True(data.Breeds.Count >= 10);
        Assert.True(data.Owners.Count >= 10);
        Assert.True(data.Pets.Count >= 10);
        Assert.True(data.Specializations.Count >= 10);
        Assert.True(data.Veterinarians.Count >= 10);
        Assert.True(data.Appointments.Count >= 10);
    }

    [Theory]
    [InlineData(AnimalSpecies.Cat, new[] { 1, 2 })]
    [InlineData(AnimalSpecies.Dog, new[] { 3, 4 })]
    [InlineData(AnimalSpecies.Rabbit, new[] { 5 })]
    [InlineData(AnimalSpecies.Snake, new int[0])]
    public void VeterinariansAreFilteredByTheSelectedSpecies(
        AnimalSpecies species, int[] expectedIds)
    {
        var data = ClinicSeed.Create(_asOfDate);

        var veterinarians = data.Veterinarians
            .Where(veterinarian => veterinarian.Specialization.Species == species)
            .ToArray();

        Assert.Equal(expectedIds, veterinarians.Select(veterinarian => veterinarian.Id));
    }

    [Fact]
    public void PetsOfAVeterinarianAreDistinctAndSortedByName()
    {
        var data = ClinicSeed.Create(_asOfDate);
        const int veterinarianId = 1;

        var pets = data.Appointments
            .Where(appointment => appointment.Veterinarian.Id == veterinarianId)
            .Select(appointment => appointment.Pet)
            .DistinctBy(pet => pet.Id)
            .OrderBy(pet => pet.Name, StringComparer.Ordinal)
            .ThenBy(pet => pet.Id)
            .ToArray();

        Assert.Equal(new[] { 1, 2, 13, 3 }, pets.Select(pet => pet.Id));
        Assert.Equal(new[] { "Арчи", "Ася", "Ася", "Барсик" }, pets.Select(pet => pet.Name));
    }

    [Theory]
    [InlineData(1, 4)]
    [InlineData(2, 1)]
    [InlineData(10, 1)]
    [InlineData(5, 0)]
    [InlineData(999, 0)]
    public void FollowUpCountIncludesEachVisitOfOnlyTheSelectedBreed(
        int breedId, int expectedCount)
    {
        var data = ClinicSeed.Create(_asOfDate);

        var count = data.Appointments.Count(appointment =>
            appointment.IsFollowUp && appointment.Pet.Breed.Id == breedId);

        Assert.Equal(expectedCount, count);
    }

    [Fact]
    public void OwnersWithMultiplePetsAreSortedByFullName()
    {
        var data = ClinicSeed.Create(_asOfDate);

        var owners = data.Owners
            .Where(owner => data.Pets.Count(pet => pet.Owner.Id == owner.Id) > 1)
            .OrderBy(owner => owner.FullName, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(new[] { 2, 3, 1 }, owners.Select(owner => owner.Id));
        Assert.Equal(new[]
        {
            "Алексеева Вера Павловна", "Иванов Борис Сергеевич", "Яковлев Артём Ильич"
        }, owners.Select(owner => owner.FullName));
    }

    [Theory]
    [InlineData(2026, 9, 15, 101, new[] { 15, 1, 2, 4, 6, 7, 9, 11 })]
    [InlineData(2026, 9, 15, 102, new[] { 3, 5, 8, 10 })]
    [InlineData(2026, 12, 31, 101, new[] { 15, 1, 2, 4, 6, 7, 9, 11 })]
    [InlineData(2024, 2, 29, 101, new[] { 15, 1, 2, 4, 6, 7, 9, 11 })]
    [InlineData(2025, 2, 28, 101, new[] { 15, 1, 2, 4, 6, 7, 9, 11 })]
    public void AppointmentsUseTheCurrentMonthAndSelectedRoom(
        int year, int month, int day, int roomNumber, int[] expectedIds)
    {
        var asOfDate = new DateOnly(year, month, day);
        // The seed shifts every appointment relative to the supplied month.
        // Expected IDs stay the same; the calendar boundaries change for each case.
        var data = ClinicSeed.Create(asOfDate);
        var monthStart = new DateTime(asOfDate.Year, asOfDate.Month, 1);
        var nextMonthStart = monthStart.AddMonths(1);

        var appointments = data.Appointments
            .Where(appointment => appointment.StartsAt >= monthStart
                && appointment.StartsAt < nextMonthStart
                && appointment.RoomNumber == roomNumber)
            .OrderBy(appointment => appointment.StartsAt)
            .ToArray();

        Assert.Equal(expectedIds, appointments.Select(appointment => appointment.Id));
    }
}
