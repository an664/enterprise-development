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

        Owner[] owners =
        [
            new Owner
            {
                Id = 1,
                FullName = "Яковлев Артём Ильич",
                Address = "Вымышленный город, Тестовая улица, дом 1",
                Phone = "+7 (000) 000-00-01"
            },
            new Owner
            {
                Id = 2,
                FullName = "Алексеева Вера Павловна",
                Address = "Вымышленный город, Тестовая улица, дом 2",
                Phone = "+7 (000) 000-00-02"
            },
            new Owner
            {
                Id = 3,
                FullName = "Иванов Борис Сергеевич",
                Address = "Вымышленный город, Тестовая улица, дом 3",
                Phone = "+7 (000) 000-00-03"
            },
            new Owner
            {
                Id = 4,
                FullName = "Белова Галина Олеговна",
                Address = "Вымышленный город, Тестовая улица, дом 4",
                Phone = "+7 (000) 000-00-04"
            },
            new Owner
            {
                Id = 5,
                FullName = "Волков Денис Петрович",
                Address = "Вымышленный город, Тестовая улица, дом 5",
                Phone = "+7 (000) 000-00-05"
            },
            new Owner
            {
                Id = 6,
                FullName = "Громова Елена Игоревна",
                Address = "Вымышленный город, Тестовая улица, дом 6",
                Phone = "+7 (000) 000-00-06"
            },
            new Owner
            {
                Id = 7,
                FullName = "Демин Захар Андреевич",
                Address = "Вымышленный город, Тестовая улица, дом 7",
                Phone = "+7 (000) 000-00-07"
            },
            new Owner
            {
                Id = 8,
                FullName = "Егорова Инна Львовна",
                Address = "Вымышленный город, Тестовая улица, дом 8",
                Phone = "+7 (000) 000-00-08"
            },
            new Owner
            {
                Id = 9,
                FullName = "Жуков Кирилл Павлович",
                Address = "Вымышленный город, Тестовая улица, дом 9",
                Phone = "+7 (000) 000-00-09"
            },
            new Owner
            {
                Id = 10,
                FullName = "Зайцева Лидия Романовна",
                Address = "Вымышленный город, Тестовая улица, дом 10",
                Phone = "+7 (000) 000-00-10"
            }
        ];

        Pet[] pets =
        [
            new Pet
            {
                Id = 1,
                Name = "Арчи",
                Breed = breeds[0],
                Owner = owners[0],
                BirthDate = new DateOnly(2020, 1, 1),
                Weight = 4.2m
            },
            new Pet
            {
                Id = 2,
                Name = "Ася",
                Breed = breeds[0],
                Owner = owners[1],
                BirthDate = new DateOnly(2021, 2, 1),
                Weight = 3.8m
            },
            new Pet
            {
                Id = 3,
                Name = "Барсик",
                Breed = breeds[1],
                Owner = owners[0],
                BirthDate = new DateOnly(2022, 3, 1),
                Weight = 4.5m
            },
            new Pet
            {
                Id = 4,
                Name = "Зефир",
                Breed = breeds[2],
                Owner = owners[2],
                BirthDate = new DateOnly(2023, 4, 1),
                Weight = 28m
            },
            new Pet
            {
                Id = 5,
                Name = "Лада",
                Breed = breeds[3],
                Owner = owners[2],
                BirthDate = new DateOnly(2020, 5, 1),
                Weight = 8.1m
            },
            new Pet
            {
                Id = 6,
                Name = "Кнопка",
                Breed = breeds[4],
                Owner = owners[3],
                BirthDate = new DateOnly(2021, 6, 1),
                Weight = 1.8m
            },
            new Pet
            {
                Id = 7,
                Name = "Кеша",
                Breed = breeds[5],
                Owner = owners[4],
                BirthDate = new DateOnly(2022, 7, 1),
                Weight = 0.04m
            },
            new Pet
            {
                Id = 8,
                Name = "Хома",
                Breed = breeds[6],
                Owner = owners[5],
                BirthDate = new DateOnly(2023, 8, 1),
                Weight = 0.12m
            },
            new Pet
            {
                Id = 9,
                Name = "Рыжик",
                Breed = breeds[7],
                Owner = owners[6],
                BirthDate = new DateOnly(2020, 9, 1),
                Weight = 0.9m
            },
            new Pet
            {
                Id = 10,
                Name = "Нордик",
                Breed = breeds[8],
                Owner = owners[7],
                BirthDate = new DateOnly(2021, 10, 1),
                Weight = 1.1m
            },
            new Pet
            {
                Id = 11,
                Name = "Тортилла",
                Breed = breeds[9],
                Owner = owners[8],
                BirthDate = new DateOnly(2022, 11, 1),
                Weight = 1.4m
            },
            new Pet
            {
                Id = 12,
                Name = "Яша",
                Breed = breeds[1],
                Owner = owners[9],
                BirthDate = new DateOnly(2023, 12, 1),
                Weight = 4m
            },
            new Pet
            {
                Id = 13,
                Name = "Ася",
                Breed = breeds[1],
                Owner = owners[1],
                BirthDate = new DateOnly(2020, 1, 1),
                Weight = 3.2m
            }
        ];

        Specialization[] specializations =
        [
            new Specialization
            {
                Id = 1,
                Name = "Терапия кошек",
                Species = AnimalSpecies.Cat
            },
            new Specialization
            {
                Id = 2,
                Name = "Хирургия кошек",
                Species = AnimalSpecies.Cat
            },
            new Specialization
            {
                Id = 3,
                Name = "Терапия собак",
                Species = AnimalSpecies.Dog
            },
            new Specialization
            {
                Id = 4,
                Name = "Хирургия собак",
                Species = AnimalSpecies.Dog
            },
            new Specialization
            {
                Id = 5,
                Name = "Лечение кроликов",
                Species = AnimalSpecies.Rabbit
            },
            new Specialization
            {
                Id = 6,
                Name = "Лечение попугаев",
                Species = AnimalSpecies.Parrot
            },
            new Specialization
            {
                Id = 7,
                Name = "Лечение хомяков",
                Species = AnimalSpecies.Hamster
            },
            new Specialization
            {
                Id = 8,
                Name = "Лечение морских свинок",
                Species = AnimalSpecies.GuineaPig
            },
            new Specialization
            {
                Id = 9,
                Name = "Лечение хорьков",
                Species = AnimalSpecies.Ferret
            },
            new Specialization
            {
                Id = 10,
                Name = "Лечение черепах",
                Species = AnimalSpecies.Turtle
            }
        ];

        Veterinarian[] veterinarians =
        [
            new Veterinarian
            {
                Id = 1,
                PassportNumber = "ТЕСТ-000001",
                FullName = "Орлов Антон Михайлович",
                BirthYear = 1979,
                Specialization = specializations[0],
                ExperienceYears = 4
            },
            new Veterinarian
            {
                Id = 2,
                PassportNumber = "ТЕСТ-000002",
                FullName = "Петрова Анна Викторовна",
                BirthYear = 1980,
                Specialization = specializations[1],
                ExperienceYears = 5
            },
            new Veterinarian
            {
                Id = 3,
                PassportNumber = "ТЕСТ-000003",
                FullName = "Романов Вадим Олегович",
                BirthYear = 1981,
                Specialization = specializations[2],
                ExperienceYears = 6
            },
            new Veterinarian
            {
                Id = 4,
                PassportNumber = "ТЕСТ-000004",
                FullName = "Соколова Дарья Ильинична",
                BirthYear = 1982,
                Specialization = specializations[3],
                ExperienceYears = 7
            },
            new Veterinarian
            {
                Id = 5,
                PassportNumber = "ТЕСТ-000005",
                FullName = "Тихонов Егор Петрович",
                BirthYear = 1983,
                Specialization = specializations[4],
                ExperienceYears = 8
            },
            new Veterinarian
            {
                Id = 6,
                PassportNumber = "ТЕСТ-000006",
                FullName = "Усова Жанна Сергеевна",
                BirthYear = 1984,
                Specialization = specializations[5],
                ExperienceYears = 9
            },
            new Veterinarian
            {
                Id = 7,
                PassportNumber = "ТЕСТ-000007",
                FullName = "Фролов Игорь Денисович",
                BirthYear = 1985,
                Specialization = specializations[6],
                ExperienceYears = 10
            },
            new Veterinarian
            {
                Id = 8,
                PassportNumber = "ТЕСТ-000008",
                FullName = "Харитонова Кира Павловна",
                BirthYear = 1986,
                Specialization = specializations[7],
                ExperienceYears = 11
            },
            new Veterinarian
            {
                Id = 9,
                PassportNumber = "ТЕСТ-000009",
                FullName = "Цветков Лев Андреевич",
                BirthYear = 1987,
                Specialization = specializations[8],
                ExperienceYears = 12
            },
            new Veterinarian
            {
                Id = 10,
                PassportNumber = "ТЕСТ-000010",
                FullName = "Шилова Мария Борисовна",
                BirthYear = 1988,
                Specialization = specializations[9],
                ExperienceYears = 13
            }
        ];

        Appointment[] appointments =
        [
            new Appointment
            {
                Id = 1,
                Pet = pets[0],
                Veterinarian = veterinarians[0],
                StartsAt = new DateTime(2026, 9, 20, 9, 0, 0),
                RoomNumber = "101А",
                IsFollowUp = false
            },
            new Appointment
            {
                Id = 2,
                Pet = pets[1],
                Veterinarian = veterinarians[0],
                StartsAt = new DateTime(2026, 10, 14, 23, 59, 59),
                RoomNumber = "101А",
                IsFollowUp = true
            },
            new Appointment
            {
                Id = 3,
                Pet = pets[0],
                Veterinarian = veterinarians[0],
                StartsAt = new DateTime(2026, 9, 17, 11, 0, 0),
                RoomNumber = "102",
                IsFollowUp = true
            },
            new Appointment
            {
                Id = 4,
                Pet = pets[2],
                Veterinarian = veterinarians[0],
                StartsAt = new DateTime(2026, 10, 25, 12, 0, 0),
                RoomNumber = "101А",
                IsFollowUp = false
            },
            new Appointment
            {
                Id = 5,
                Pet = pets[3],
                Veterinarian = veterinarians[2],
                StartsAt = new DateTime(2026, 9, 19, 9, 0, 0),
                RoomNumber = "102",
                IsFollowUp = true
            },
            new Appointment
            {
                Id = 6,
                Pet = pets[4],
                Veterinarian = veterinarians[3],
                StartsAt = new DateTime(2026, 2, 27, 23, 59, 59),
                RoomNumber = "101А",
                IsFollowUp = true
            },
            new Appointment
            {
                Id = 7,
                Pet = pets[5],
                Veterinarian = veterinarians[4],
                StartsAt = new DateTime(2026, 2, 28, 0, 0, 0),
                RoomNumber = "101А",
                IsFollowUp = false
            },
            new Appointment
            {
                Id = 8,
                Pet = pets[6],
                Veterinarian = veterinarians[5],
                StartsAt = new DateTime(2027, 1, 10, 9, 0, 0),
                RoomNumber = "102",
                IsFollowUp = false
            },
            new Appointment
            {
                Id = 9,
                Pet = pets[7],
                Veterinarian = veterinarians[6],
                StartsAt = new DateTime(2026, 1, 31, 0, 0, 0),
                RoomNumber = "101А",
                IsFollowUp = true
            },
            new Appointment
            {
                Id = 10,
                Pet = pets[8],
                Veterinarian = veterinarians[7],
                StartsAt = new DateTime(2026, 12, 31, 0, 0, 0),
                RoomNumber = "102",
                IsFollowUp = false
            },
            new Appointment
            {
                Id = 11,
                Pet = pets[9],
                Veterinarian = veterinarians[8],
                StartsAt = new DateTime(2024, 2, 28, 23, 59, 59),
                RoomNumber = "101А",
                IsFollowUp = true
            },
            new Appointment
            {
                Id = 12,
                Pet = pets[10],
                Veterinarian = veterinarians[9],
                StartsAt = new DateTime(2024, 2, 29, 0, 0, 0),
                RoomNumber = "101А",
                IsFollowUp = true
            },
            new Appointment
            {
                Id = 13,
                Pet = pets[11],
                Veterinarian = veterinarians[1],
                StartsAt = new DateTime(2026, 9, 14, 23, 59, 59),
                RoomNumber = "101А",
                IsFollowUp = true
            },
            new Appointment
            {
                Id = 14,
                Pet = pets[12],
                Veterinarian = veterinarians[0],
                StartsAt = new DateTime(2027, 1, 31, 0, 0, 0),
                RoomNumber = "102",
                IsFollowUp = false
            },
            new Appointment
            {
                Id = 15,
                Pet = pets[0],
                Veterinarian = veterinarians[0],
                StartsAt = new DateTime(2026, 9, 15, 0, 0, 0),
                RoomNumber = "101А",
                IsFollowUp = true
            },
            new Appointment
            {
                Id = 16,
                Pet = pets[0],
                Veterinarian = veterinarians[0],
                StartsAt = new DateTime(2026, 10, 15, 0, 0, 0),
                RoomNumber = "101А",
                IsFollowUp = true
            },
            new Appointment
            {
                Id = 17,
                Pet = pets[10],
                Veterinarian = veterinarians[9],
                StartsAt = new DateTime(2024, 1, 31, 0, 0, 0),
                RoomNumber = "101А",
                IsFollowUp = false
            }
        ];

        Data = new ClinicData(breeds, owners, pets, specializations, veterinarians, appointments);
    }
}
