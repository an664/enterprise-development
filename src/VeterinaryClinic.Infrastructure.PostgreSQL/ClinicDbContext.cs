using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Domain.Data;
using VeterinaryClinic.Domain.Models;

namespace VeterinaryClinic.Infrastructure.PostgreSQL;

/// <summary>
/// Данные ветеринарной клиники в PostgreSQL.
/// </summary>
/// <param name="options">Настройки подключения и провайдера.</param>
public class ClinicDbContext(DbContextOptions<ClinicDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Имя индекса, который запрещает два приёма в одном кабинете в одно время.
    /// </summary>
    public const string AppointmentSlotIndex = "ux_appointments_room_start";

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var pets = modelBuilder.Entity<Pet>();
        pets.HasOne(pet => pet.Breed).WithMany().HasForeignKey("breed_id").OnDelete(DeleteBehavior.Restrict);
        pets.HasOne(pet => pet.Owner).WithMany().HasForeignKey("owner_id").OnDelete(DeleteBehavior.Restrict);
        pets.Navigation(pet => pet.Breed).AutoInclude();
        pets.Navigation(pet => pet.Owner).AutoInclude();

        var veterinarians = modelBuilder.Entity<Veterinarian>();
        veterinarians.HasOne(vet => vet.Specialization).WithMany()
            .HasForeignKey("specialization_id").OnDelete(DeleteBehavior.Restrict);
        veterinarians.Navigation(vet => vet.Specialization).AutoInclude();

        var appointments = modelBuilder.Entity<Appointment>();
        appointments.HasOne(visit => visit.Pet).WithMany().HasForeignKey("pet_id").OnDelete(DeleteBehavior.Restrict);
        appointments.HasOne(visit => visit.Veterinarian).WithMany()
            .HasForeignKey("veterinarian_id").OnDelete(DeleteBehavior.Restrict);
        appointments.Navigation(visit => visit.Pet).AutoInclude();
        appointments.Navigation(visit => visit.Veterinarian).AutoInclude();
        appointments.HasIndex(visit => new { visit.RoomNumber, visit.StartsAt })
            .IsUnique()
            .HasDatabaseName(AppointmentSlotIndex);

        Seed(modelBuilder);
    }

    /// <summary>
    /// Формирует строки миграции из фиксированного набора первой лабораторной.
    /// Вместо ссылок на объекты в таблицы записываются внешние ключи.
    /// </summary>
    /// <param name="modelBuilder">Модель базы данных.</param>
    private static void Seed(ModelBuilder modelBuilder)
    {
        var data = ClinicSeed.Create();
        modelBuilder.Entity<Breed>().HasData(data.Breeds);
        modelBuilder.Entity<Owner>().HasData(data.Owners);
        modelBuilder.Entity<Specialization>().HasData(data.Specializations);
        modelBuilder.Entity<Pet>().HasData(data.Pets.Select(pet => new
        {
            pet.Id,
            pet.Name,
            breed_id = pet.Breed.Id,
            owner_id = pet.Owner.Id,
            pet.BirthDate,
            pet.Weight
        }));
        modelBuilder.Entity<Veterinarian>().HasData(data.Veterinarians.Select(vet => new
        {
            vet.Id,
            vet.FullName,
            vet.PassportNumber,
            vet.BirthYear,
            specialization_id = vet.Specialization.Id,
            vet.ExperienceYears
        }));
        modelBuilder.Entity<Appointment>().HasData(data.Appointments.Select(visit => new
        {
            visit.Id,
            pet_id = visit.Pet.Id,
            veterinarian_id = visit.Veterinarian.Id,
            visit.StartsAt,
            visit.RoomNumber,
            visit.IsFollowUp
        }));
    }
}
