using VeterinaryClinic.Models;

namespace VeterinaryClinic.Tests;

/// <summary>
/// Подготавливает данные ветеринарной клиники для тестов.
/// </summary>
public class ClinicFixture
{
    /// <summary>
    /// Справочник пород животных.
    /// </summary>
    private static readonly IReadOnlyList<Breed> _breeds =
    [
        new Breed
        {
            Id = 1,
            Name = "Британская короткошёрстная",
            Species = AnimalSpecies.Cat
        },
        new Breed
        {
            Id = 2,
            Name = "Сиамская",
            Species = AnimalSpecies.Cat
        },
        new Breed
        {
            Id = 3,
            Name = "Лабрадор",
            Species = AnimalSpecies.Dog
        },
        new Breed
        {
            Id = 4,
            Name = "Такса",
            Species = AnimalSpecies.Dog
        },
        new Breed
        {
            Id = 5,
            Name = "Карликовый баран",
            Species = AnimalSpecies.Rabbit
        },
        new Breed
        {
            Id = 6,
            Name = "Волнистый",
            Species = AnimalSpecies.Parrot
        },
        new Breed
        {
            Id = 7,
            Name = "Сирийский",
            Species = AnimalSpecies.Hamster
        },
        new Breed
        {
            Id = 8,
            Name = "Абиссинская",
            Species = AnimalSpecies.GuineaPig
        },
        new Breed
        {
            Id = 9,
            Name = "Домашний",
            Species = AnimalSpecies.Ferret
        },
        new Breed
        {
            Id = 10,
            Name = "Красноухая",
            Species = AnimalSpecies.Turtle
        }
    ];

    /// <summary>
    /// Общий набор данных с фиксированными датами приёмов.
    /// </summary>
    internal ClinicData Data { get; }

    /// <summary>
    /// Создаёт общий набор данных для тестового класса.
    /// </summary>
    public ClinicFixture()
    {
        var breeds = _breeds;

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

        (string Name, int BreedIndex, int OwnerIndex, decimal Weight)[] petRows =
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
            Weight = row.Weight
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

        Appointment Visit(int id, int petId, int veterinarianId, DateTime startsAt,
            string roomNumber, bool isFollowUp) => new()
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
            Visit(1, 1, 1, new DateTime(2026, 9, 20, 9, 0, 0), "101А", false),
            Visit(2, 2, 1, new DateTime(2026, 10, 14, 23, 59, 59), "101А", true),
            Visit(3, 1, 1, new DateTime(2026, 9, 17, 11, 0, 0), "102", true),
            Visit(4, 3, 1, new DateTime(2026, 10, 25, 12, 0, 0), "101А", false),
            Visit(5, 4, 3, new DateTime(2026, 9, 19, 9, 0, 0), "102", true),
            Visit(6, 5, 4, new DateTime(2026, 2, 27, 23, 59, 59), "101А", true),
            Visit(7, 6, 5, new DateTime(2026, 2, 28, 0, 0, 0), "101А", false),
            Visit(8, 7, 6, new DateTime(2027, 1, 10, 9, 0, 0), "102", false),
            Visit(9, 8, 7, new DateTime(2026, 1, 31, 0, 0, 0), "101А", true),
            Visit(10, 9, 8, new DateTime(2026, 12, 31, 0, 0, 0), "102", false),
            Visit(11, 10, 9, new DateTime(2024, 2, 28, 23, 59, 59), "101А", true),
            Visit(12, 11, 10, new DateTime(2024, 2, 29, 0, 0, 0), "101А", true),
            Visit(13, 12, 2, new DateTime(2026, 9, 14, 23, 59, 59), "101А", true),
            Visit(14, 13, 1, new DateTime(2027, 1, 31, 0, 0, 0), "102", false),
            Visit(15, 1, 1, new DateTime(2026, 9, 15, 0, 0, 0), "101А", true),
            Visit(16, 1, 1, new DateTime(2026, 10, 15, 0, 0, 0), "101А", true),
            Visit(17, 11, 10, new DateTime(2024, 1, 31, 0, 0, 0), "101А", false)
        ];

        Data = new ClinicData(breeds, owners, pets, specializations, veterinarians, appointments);
    }
}
