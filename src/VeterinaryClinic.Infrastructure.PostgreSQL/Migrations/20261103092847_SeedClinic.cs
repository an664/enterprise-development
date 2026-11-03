using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VeterinaryClinic.Infrastructure.PostgreSQL.Migrations;

/// <inheritdoc />
public partial class SeedClinic : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "breeds",
            columns: new[] { "id", "name", "species" },
            values: new object[,]
            {
                { 1, "Британская короткошёрстная", 0 },
                { 2, "Сиамская", 0 },
                { 3, "Лабрадор", 1 },
                { 4, "Такса", 1 },
                { 5, "Карликовый баран", 2 },
                { 6, "Волнистый", 3 },
                { 7, "Сирийский", 4 },
                { 8, "Абиссинская", 5 },
                { 9, "Домашний", 6 },
                { 10, "Красноухая", 7 }
            });

        migrationBuilder.InsertData(
            table: "owners",
            columns: new[] { "id", "address", "full_name", "phone" },
            values: new object[,]
            {
                { 1, "Вымышленный город, Тестовая улица, дом 1", "Яковлев Артём Ильич", "+7 (000) 000-00-01" },
                { 2, "Вымышленный город, Тестовая улица, дом 2", "Алексеева Вера Павловна", "+7 (000) 000-00-02" },
                { 3, "Вымышленный город, Тестовая улица, дом 3", "Иванов Борис Сергеевич", "+7 (000) 000-00-03" },
                { 4, "Вымышленный город, Тестовая улица, дом 4", "Белова Галина Олеговна", "+7 (000) 000-00-04" },
                { 5, "Вымышленный город, Тестовая улица, дом 5", "Волков Денис Петрович", "+7 (000) 000-00-05" },
                { 6, "Вымышленный город, Тестовая улица, дом 6", "Громова Елена Игоревна", "+7 (000) 000-00-06" },
                { 7, "Вымышленный город, Тестовая улица, дом 7", "Демин Захар Андреевич", "+7 (000) 000-00-07" },
                { 8, "Вымышленный город, Тестовая улица, дом 8", "Егорова Инна Львовна", "+7 (000) 000-00-08" },
                { 9, "Вымышленный город, Тестовая улица, дом 9", "Жуков Кирилл Павлович", "+7 (000) 000-00-09" },
                { 10, "Вымышленный город, Тестовая улица, дом 10", "Зайцева Лидия Романовна", "+7 (000) 000-00-10" }
            });

        migrationBuilder.InsertData(
            table: "specializations",
            columns: new[] { "id", "name", "species" },
            values: new object[,]
            {
                { 1, "Терапия кошек", 0 },
                { 2, "Хирургия кошек", 0 },
                { 3, "Терапия собак", 1 },
                { 4, "Хирургия собак", 1 },
                { 5, "Лечение кроликов", 2 },
                { 6, "Лечение попугаев", 3 },
                { 7, "Лечение хомяков", 4 },
                { 8, "Лечение морских свинок", 5 },
                { 9, "Лечение хорьков", 6 },
                { 10, "Лечение черепах", 7 }
            });

        migrationBuilder.InsertData(
            table: "pets",
            columns: new[] { "id", "birth_date", "name", "weight", "breed_id", "owner_id" },
            values: new object[,]
            {
                { 1, new DateOnly(2020, 1, 1), "Арчи", 4.2m, 1, 1 },
                { 2, new DateOnly(2021, 2, 1), "Ася", 3.8m, 1, 2 },
                { 3, new DateOnly(2022, 3, 1), "Барсик", 4.5m, 2, 1 },
                { 4, new DateOnly(2023, 4, 1), "Зефир", 28m, 3, 3 },
                { 5, new DateOnly(2020, 5, 1), "Лада", 8.1m, 4, 3 },
                { 6, new DateOnly(2021, 6, 1), "Кнопка", 1.8m, 5, 4 },
                { 7, new DateOnly(2022, 7, 1), "Кеша", 0.04m, 6, 5 },
                { 8, new DateOnly(2023, 8, 1), "Хома", 0.12m, 7, 6 },
                { 9, new DateOnly(2020, 9, 1), "Рыжик", 0.9m, 8, 7 },
                { 10, new DateOnly(2021, 10, 1), "Нордик", 1.1m, 9, 8 },
                { 11, new DateOnly(2022, 11, 1), "Тортилла", 1.4m, 10, 9 },
                { 12, new DateOnly(2023, 12, 1), "Яша", 4m, 2, 10 },
                { 13, new DateOnly(2020, 1, 1), "Ася", 3.2m, 2, 2 }
            });

        migrationBuilder.InsertData(
            table: "veterinarians",
            columns: new[] { "id", "birth_year", "experience_years", "full_name", "passport_number", "specialization_id" },
            values: new object[,]
            {
                { 1, 1979, 4, "Орлов Антон Михайлович", "ТЕСТ-000001", 1 },
                { 2, 1980, 5, "Петрова Анна Викторовна", "ТЕСТ-000002", 2 },
                { 3, 1981, 6, "Романов Вадим Олегович", "ТЕСТ-000003", 3 },
                { 4, 1982, 7, "Соколова Дарья Ильинична", "ТЕСТ-000004", 4 },
                { 5, 1983, 8, "Тихонов Егор Петрович", "ТЕСТ-000005", 5 },
                { 6, 1984, 9, "Усова Жанна Сергеевна", "ТЕСТ-000006", 6 },
                { 7, 1985, 10, "Фролов Игорь Денисович", "ТЕСТ-000007", 7 },
                { 8, 1986, 11, "Харитонова Кира Павловна", "ТЕСТ-000008", 8 },
                { 9, 1987, 12, "Цветков Лев Андреевич", "ТЕСТ-000009", 9 },
                { 10, 1988, 13, "Шилова Мария Борисовна", "ТЕСТ-000010", 10 }
            });

        migrationBuilder.InsertData(
            table: "appointments",
            columns: new[] { "id", "is_follow_up", "room_number", "starts_at", "pet_id", "veterinarian_id" },
            values: new object[,]
            {
                { 1, false, "101А", new DateTime(2026, 9, 20, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                { 2, true, "101А", new DateTime(2026, 10, 14, 23, 59, 59, 0, DateTimeKind.Unspecified), 2, 1 },
                { 3, true, "102", new DateTime(2026, 9, 17, 11, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                { 4, false, "101А", new DateTime(2026, 10, 25, 12, 0, 0, 0, DateTimeKind.Unspecified), 3, 1 },
                { 5, true, "102", new DateTime(2026, 9, 19, 9, 0, 0, 0, DateTimeKind.Unspecified), 4, 3 },
                { 6, true, "101А", new DateTime(2026, 2, 27, 23, 59, 59, 0, DateTimeKind.Unspecified), 5, 4 },
                { 7, false, "101А", new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 5 },
                { 8, false, "102", new DateTime(2027, 1, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), 7, 6 },
                { 9, true, "101А", new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 7 },
                { 10, false, "102", new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 8 },
                { 11, true, "101А", new DateTime(2024, 2, 28, 23, 59, 59, 0, DateTimeKind.Unspecified), 10, 9 },
                { 12, true, "101А", new DateTime(2024, 2, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, 10 },
                { 13, true, "101А", new DateTime(2026, 9, 14, 23, 59, 59, 0, DateTimeKind.Unspecified), 12, 2 },
                { 14, false, "102", new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 13, 1 },
                { 15, true, "101А", new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                { 16, true, "101А", new DateTime(2026, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                { 17, false, "101А", new DateTime(2024, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, 10 }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 5);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 6);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 7);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 8);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 9);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 10);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 11);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 12);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 13);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 14);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 15);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 16);

        migrationBuilder.DeleteData(
            table: "appointments",
            keyColumn: "id",
            keyValue: 17);

        migrationBuilder.DeleteData(
            table: "pets",
            keyColumn: "id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "pets",
            keyColumn: "id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "pets",
            keyColumn: "id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "pets",
            keyColumn: "id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "pets",
            keyColumn: "id",
            keyValue: 5);

        migrationBuilder.DeleteData(
            table: "pets",
            keyColumn: "id",
            keyValue: 6);

        migrationBuilder.DeleteData(
            table: "pets",
            keyColumn: "id",
            keyValue: 7);

        migrationBuilder.DeleteData(
            table: "pets",
            keyColumn: "id",
            keyValue: 8);

        migrationBuilder.DeleteData(
            table: "pets",
            keyColumn: "id",
            keyValue: 9);

        migrationBuilder.DeleteData(
            table: "pets",
            keyColumn: "id",
            keyValue: 10);

        migrationBuilder.DeleteData(
            table: "pets",
            keyColumn: "id",
            keyValue: 11);

        migrationBuilder.DeleteData(
            table: "pets",
            keyColumn: "id",
            keyValue: 12);

        migrationBuilder.DeleteData(
            table: "pets",
            keyColumn: "id",
            keyValue: 13);

        migrationBuilder.DeleteData(
            table: "veterinarians",
            keyColumn: "id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "veterinarians",
            keyColumn: "id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "veterinarians",
            keyColumn: "id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "veterinarians",
            keyColumn: "id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "veterinarians",
            keyColumn: "id",
            keyValue: 5);

        migrationBuilder.DeleteData(
            table: "veterinarians",
            keyColumn: "id",
            keyValue: 6);

        migrationBuilder.DeleteData(
            table: "veterinarians",
            keyColumn: "id",
            keyValue: 7);

        migrationBuilder.DeleteData(
            table: "veterinarians",
            keyColumn: "id",
            keyValue: 8);

        migrationBuilder.DeleteData(
            table: "veterinarians",
            keyColumn: "id",
            keyValue: 9);

        migrationBuilder.DeleteData(
            table: "veterinarians",
            keyColumn: "id",
            keyValue: 10);

        migrationBuilder.DeleteData(
            table: "breeds",
            keyColumn: "id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "breeds",
            keyColumn: "id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "breeds",
            keyColumn: "id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "breeds",
            keyColumn: "id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "breeds",
            keyColumn: "id",
            keyValue: 5);

        migrationBuilder.DeleteData(
            table: "breeds",
            keyColumn: "id",
            keyValue: 6);

        migrationBuilder.DeleteData(
            table: "breeds",
            keyColumn: "id",
            keyValue: 7);

        migrationBuilder.DeleteData(
            table: "breeds",
            keyColumn: "id",
            keyValue: 8);

        migrationBuilder.DeleteData(
            table: "breeds",
            keyColumn: "id",
            keyValue: 9);

        migrationBuilder.DeleteData(
            table: "breeds",
            keyColumn: "id",
            keyValue: 10);

        migrationBuilder.DeleteData(
            table: "owners",
            keyColumn: "id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "owners",
            keyColumn: "id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "owners",
            keyColumn: "id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "owners",
            keyColumn: "id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "owners",
            keyColumn: "id",
            keyValue: 5);

        migrationBuilder.DeleteData(
            table: "owners",
            keyColumn: "id",
            keyValue: 6);

        migrationBuilder.DeleteData(
            table: "owners",
            keyColumn: "id",
            keyValue: 7);

        migrationBuilder.DeleteData(
            table: "owners",
            keyColumn: "id",
            keyValue: 8);

        migrationBuilder.DeleteData(
            table: "owners",
            keyColumn: "id",
            keyValue: 9);

        migrationBuilder.DeleteData(
            table: "owners",
            keyColumn: "id",
            keyValue: 10);

        migrationBuilder.DeleteData(
            table: "specializations",
            keyColumn: "id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "specializations",
            keyColumn: "id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "specializations",
            keyColumn: "id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "specializations",
            keyColumn: "id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "specializations",
            keyColumn: "id",
            keyValue: 5);

        migrationBuilder.DeleteData(
            table: "specializations",
            keyColumn: "id",
            keyValue: 6);

        migrationBuilder.DeleteData(
            table: "specializations",
            keyColumn: "id",
            keyValue: 7);

        migrationBuilder.DeleteData(
            table: "specializations",
            keyColumn: "id",
            keyValue: 8);

        migrationBuilder.DeleteData(
            table: "specializations",
            keyColumn: "id",
            keyValue: 9);

        migrationBuilder.DeleteData(
            table: "specializations",
            keyColumn: "id",
            keyValue: 10);
    }
}
