using VeterinaryClinic.Contracts.Analytics;
using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Breeds;
using VeterinaryClinic.Contracts.Owners;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Contracts.Specializations;
using VeterinaryClinic.Contracts.Veterinarians;
using VeterinaryClinic.Domain.Models;

namespace VeterinaryClinic.Application;

/// <summary>
/// Преобразует доменные сущности в ответы API.
/// </summary>
internal static class ClinicMapping
{
    /// <summary>
    /// Формирует DTO для владельца.
    /// </summary>
    /// <param name="entity">Доменная запись.</param>
    /// <returns>DTO без вложенных доменных объектов.</returns>
    internal static OwnerDto ToDto(Owner entity)
    {
        return new(entity.Id, entity.FullName, entity.Address, entity.Phone);
    }

    /// <summary>
    /// Формирует ответ с количеством питомцев у владельца.
    /// </summary>
    /// <param name="result">Владелец и количество его питомцев.</param>
    /// <returns>Сведения о владельце и количестве питомцев.</returns>
    internal static OwnerWithPetCountDto ToDto(OwnerWithPetCount result)
    {
        var owner = result.Owner;
        return new(owner.Id, owner.FullName, owner.Address, owner.Phone, result.PetCount);
    }

    /// <summary>
    /// Формирует DTO для породы.
    /// </summary>
    /// <param name="entity">Доменная запись.</param>
    /// <returns>DTO без вложенных доменных объектов.</returns>
    internal static BreedDto ToDto(Breed entity)
    {
        return new(entity.Id, entity.Name, entity.Species);
    }

    /// <summary>
    /// Формирует DTO для специализации.
    /// </summary>
    /// <param name="entity">Доменная запись.</param>
    /// <returns>DTO без вложенных доменных объектов.</returns>
    internal static SpecializationDto ToDto(Specialization entity)
    {
        return new(entity.Id, entity.Name, entity.Species);
    }

    /// <summary>
    /// Формирует DTO для питомца.
    /// </summary>
    /// <param name="entity">Доменная запись.</param>
    /// <returns>DTO без вложенных доменных объектов.</returns>
    internal static PetDto ToDto(Pet entity)
    {
        return new(
            entity.Id,
            entity.Name,
            entity.Breed.Id,
            entity.Owner.Id,
            entity.BirthDate,
            entity.Weight,
            entity.Species);
    }

    /// <summary>
    /// Формирует DTO для ветеринара.
    /// </summary>
    /// <param name="entity">Доменная запись.</param>
    /// <returns>DTO без вложенных доменных объектов.</returns>
    internal static VeterinarianDto ToDto(Veterinarian entity)
    {
        return new(
            entity.Id,
            entity.PassportNumber,
            entity.FullName,
            entity.BirthYear,
            entity.Specialization.Id,
            entity.ExperienceYears);
    }

    /// <summary>
    /// Формирует DTO для приёма.
    /// </summary>
    /// <param name="entity">Доменная запись.</param>
    /// <returns>DTO без вложенных доменных объектов.</returns>
    internal static AppointmentDto ToDto(Appointment entity)
    {
        return new(
            entity.Id,
            entity.Pet.Id,
            entity.Veterinarian.Id,
            entity.StartsAt,
            entity.RoomNumber,
            entity.IsFollowUp);
    }
}
