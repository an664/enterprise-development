using VeterinaryClinic.Domain.Models;

namespace VeterinaryClinic.Infrastructure.InMemory;

/// <summary>
/// Копирует записи для чтения вне блокировки репозитория.
/// </summary>
internal static class EntitySnapshot
{

    /// <summary>
    /// Создаёт независимую копию записи владельца.
    /// </summary>
    internal static Owner Copy(Owner source)
    {
        return new()
        {
            Id = source.Id,
            FullName = source.FullName,
            Address = source.Address,
            Phone = source.Phone
        };
    }

    /// <summary>
    /// Создаёт независимую копию записи породы.
    /// </summary>
    internal static Breed Copy(Breed source)
    {
        return new()
        {
            Id = source.Id,
            Name = source.Name,
            Species = source.Species
        };
    }

    /// <summary>
    /// Создаёт независимую копию записи специализации.
    /// </summary>
    internal static Specialization Copy(Specialization source)
    {
        return new()
        {
            Id = source.Id,
            Name = source.Name,
            Species = source.Species
        };
    }

    /// <summary>
    /// Создаёт независимую копию записи ветеринара.
    /// </summary>
    internal static Veterinarian Copy(Veterinarian source)
    {
        return new()
        {
            Id = source.Id,
            PassportNumber = source.PassportNumber,
            FullName = source.FullName,
            BirthYear = source.BirthYear,
            Specialization = Copy(source.Specialization),
            ExperienceYears = source.ExperienceYears
        };
    }

    /// <summary>
    /// Создаёт независимую копию записи питомца.
    /// </summary>
    internal static Pet Copy(Pet source)
    {
        return new()
        {
            Id = source.Id,
            Name = source.Name,
            Breed = Copy(source.Breed),
            Owner = Copy(source.Owner),
            BirthDate = source.BirthDate,
            Weight = source.Weight
        };
    }

    /// <summary>
    /// Создаёт независимую копию записи приёма.
    /// </summary>
    internal static Appointment Copy(Appointment source)
    {
        return new()
        {
            Id = source.Id,
            Pet = Copy(source.Pet),
            Veterinarian = Copy(source.Veterinarian),
            StartsAt = source.StartsAt,
            RoomNumber = source.RoomNumber,
            IsFollowUp = source.IsFollowUp
        };
    }
}
