using VeterinaryClinic.Models;

namespace VeterinaryClinic.Tests;

internal sealed record ClinicData(
    IReadOnlyList<Breed> Breeds,
    IReadOnlyList<Owner> Owners,
    IReadOnlyList<Pet> Pets,
    IReadOnlyList<Specialization> Specializations,
    IReadOnlyList<Veterinarian> Veterinarians,
    IReadOnlyList<Appointment> Appointments);

internal static class ClinicSeed
{
    internal static ClinicData Create(DateOnly asOfDate)
    {
        (string Name, AnimalSpecies Species)[] breedRows =
        [
            ("Британская короткошёрстная", AnimalSpecies.Cat),
            ("Сиамская", AnimalSpecies.Cat),
            ("Лабрадор", AnimalSpecies.Dog),
            ("Такса", AnimalSpecies.Dog),
            ("Карликовый баран", AnimalSpecies.Rabbit),
            ("Волнистый", AnimalSpecies.Parrot),
            ("Сирийский", AnimalSpecies.Hamster),
            ("Абиссинская", AnimalSpecies.GuineaPig),
            ("Домашний", AnimalSpecies.Ferret),
            ("Красноухая", AnimalSpecies.Turtle)
        ];
        var breeds = breedRows.Select((row, index) => new Breed
        {
            Id = index + 1,
            Name = row.Name,
            Species = row.Species
        }).ToArray();

        string[] ownerNames =
        [
            "Яковлев Артём Ильич", "Алексеева Вера Павловна", "Иванов Борис Сергеевич",
            "Белова Галина Олеговна", "Волков Денис Петрович", "Громова Елена Игоревна",
            "Демин Захар Андреевич", "Егорова Инна Львовна", "Жуков Кирилл Павлович",
            "Зайцева Лидия Романовна"
        ];
        var owners = ownerNames.Select((name, index) => new Owner
        {
            Id = index + 1,
            FullName = name,
            Address = $"Вымышленный город, Тестовая улица, дом {index + 1}",
            Phone = $"+7 (000) 000-00-{index + 1:00}"
        }).ToArray();

        (string Name, int BreedIndex, int OwnerIndex, decimal WeightKg)[] petRows =
        [
            ("Арчи", 0, 0, 4.2m), ("Ася", 0, 1, 3.8m), ("Барсик", 1, 0, 4.5m),
            ("Зефир", 2, 2, 28m), ("Лада", 3, 2, 8.1m), ("Кнопка", 4, 3, 1.8m),
            ("Кеша", 5, 4, 0.04m), ("Хома", 6, 5, 0.12m), ("Рыжик", 7, 6, 0.9m),
            ("Нордик", 8, 7, 1.1m), ("Тортилла", 9, 8, 1.4m), ("Яша", 1, 9, 4m),
            ("Ася", 1, 1, 3.2m)
        ];
        var pets = petRows.Select((row, index) => new Pet
        {
            Id = index + 1,
            Name = row.Name,
            Breed = breeds[row.BreedIndex],
            Owner = owners[row.OwnerIndex],
            BirthDate = new DateOnly(2020 + index % 4, 1 + index % 12, 1),
            WeightKg = row.WeightKg
        }).ToArray();

        string[] specializationNames =
        [
            "Терапия кошек", "Хирургия кошек", "Терапия собак", "Хирургия собак",
            "Лечение кроликов", "Лечение попугаев", "Лечение хомяков",
            "Лечение морских свинок", "Лечение хорьков", "Лечение черепах"
        ];
        var specializations = specializationNames.Select((name, index) => new Specialization
        {
            Id = index + 1,
            Name = name,
            Species = breeds[index].Species
        }).ToArray();

        string[] veterinarianNames =
        [
            "Орлов Антон Михайлович", "Петрова Анна Викторовна", "Романов Вадим Олегович",
            "Соколова Дарья Ильинична", "Тихонов Егор Петрович", "Усова Жанна Сергеевна",
            "Фролов Игорь Денисович", "Харитонова Кира Павловна", "Цветков Лев Андреевич",
            "Шилова Мария Борисовна"
        ];
        var veterinarians = veterinarianNames.Select((name, index) => new Veterinarian
        {
            Id = index + 1,
            PassportNumber = $"ТЕСТ-{index + 1:000000}",
            FullName = name,
            BirthYear = 1979 + index,
            Specialization = specializations[index],
            ExperienceYears = 4 + index
        }).ToArray();

        var monthStart = new DateTime(asOfDate.Year, asOfDate.Month, 1);
        var nextMonthStart = monthStart.AddMonths(1);

        Appointment Visit(int id, int petId, int veterinarianId, DateTime startsAt,
            int roomNumber, bool isFollowUp) => new()
            {
                Id = id,
                Pet = pets[petId - 1],
                Veterinarian = veterinarians[veterinarianId - 1],
                StartsAt = startsAt,
                RoomNumber = roomNumber,
                IsFollowUp = isFollowUp
            };

        Appointment[] appointments =
        [
            Visit(1, 1, 1, monthStart.AddHours(9), 101, false),
            Visit(2, 2, 1, monthStart.AddHours(10), 101, true),
            Visit(3, 1, 1, monthStart.AddDays(2).AddHours(11), 102, true),
            Visit(4, 3, 1, monthStart.AddDays(3).AddHours(12), 101, false),
            Visit(5, 4, 3, monthStart.AddDays(4).AddHours(9), 102, true),
            Visit(6, 5, 4, monthStart.AddDays(5).AddHours(9), 101, true),
            Visit(7, 6, 5, monthStart.AddDays(6).AddHours(9), 101, false),
            Visit(8, 7, 6, monthStart.AddDays(7).AddHours(9), 102, false),
            Visit(9, 8, 7, monthStart.AddDays(8).AddHours(9), 101, true),
            Visit(10, 9, 8, monthStart.AddDays(9).AddHours(9), 102, false),
            Visit(11, 10, 9, nextMonthStart.AddTicks(-1), 101, true),
            Visit(12, 11, 10, nextMonthStart, 101, true),
            Visit(13, 12, 2, monthStart.AddTicks(-1), 101, true),
            Visit(14, 13, 1, nextMonthStart.AddHours(8), 102, false),
            Visit(15, 1, 1, monthStart, 101, true),
            Visit(16, 1, 1, nextMonthStart.AddDays(1), 101, true),
            Visit(17, 11, 10, monthStart.AddYears(-1).AddHours(9), 101, false)
        ];

        return new ClinicData(breeds, owners, pets, specializations, veterinarians, appointments);
    }
}
