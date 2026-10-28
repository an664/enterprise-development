using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using VeterinaryClinic.Contracts.Analytics;
using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Owners;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Contracts.Veterinarians;
using VeterinaryClinic.Data;
using VeterinaryClinic.Repositories;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Выполняет аналитику над текущими данными репозиториев.
/// </summary>
/// <param name="veterinarians">Репозиторий ветеринаров.</param>
/// <param name="pets">Репозиторий питомцев.</param>
/// <param name="breeds">Репозиторий пород.</param>
/// <param name="owners">Репозиторий владельцев.</param>
/// <param name="appointments">Репозиторий приёмов.</param>
/// <param name="synchronization">Общая блокировка связанных данных.</param>
/// <param name="logger">Журнал аналитических запросов.</param>
public class AnalyticsService(
    IVeterinarianRepository veterinarians,
    IPetRepository pets,
    IBreedRepository breeds,
    IOwnerRepository owners,
    IAppointmentRepository appointments,
    ClinicLock synchronization,
    ILogger<AnalyticsService> logger) : IAnalyticsService
{
    /// <inheritdoc />
    public IReadOnlyList<VeterinarianDto> GetVeterinarians(AnimalSpecies species)
    {
        lock (synchronization.SyncRoot)
        {
            logger.LogInformation("Выборка врачей по виду {Species}", species);
            return [.. veterinarians.GetBySpecies(species).Select(ClinicMapping.ToDto)];
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<PetDto> GetPets(int veterinarianId)
    {
        lock (synchronization.SyncRoot)
        {
            _ = veterinarians.GetById(veterinarianId)
                ?? throw new EntityNotFoundException("Ветеринар не найден.");
            logger.LogInformation("Выборка питомцев врача {Id}", veterinarianId);
            return [.. pets.GetByVeterinarian(veterinarianId).Select(ClinicMapping.ToDto)];
        }
    }

    /// <inheritdoc />
    public FollowUpCountDto GetFollowUpCount(int breedId)
    {
        lock (synchronization.SyncRoot)
        {
            _ = breeds.GetById(breedId)
                ?? throw new EntityNotFoundException("Порода не найдена.");
            logger.LogInformation("Подсчёт повторных приёмов породы {Id}", breedId);
            return new(breedId, appointments.CountFollowUpsByBreed(breedId));
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<OwnerDto> GetOwnersWithMultiplePets()
    {
        lock (synchronization.SyncRoot)
        {
            logger.LogInformation("Выборка владельцев нескольких питомцев");
            return [.. owners.GetWithMultiplePets().Select(ClinicMapping.ToDto)];
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<AppointmentDto> GetAppointments(DateOnly asOfDate, string roomNumber)
    {
        if (asOfDate == default || (asOfDate.Year == 9999 && asOfDate.Month == 12))
        {
            throw new ValidationException("Укажите дату, для которой можно определить конец месячного периода.");
        }

        var from = asOfDate.ToDateTime(TimeOnly.MinValue);
        var to = from.AddMonths(1);
        lock (synchronization.SyncRoot)
        {
            logger.LogInformation("Выборка приёмов за период {From} - {To}", from, to);
            return [.. appointments.GetForPeriod(from, to, roomNumber).Select(ClinicMapping.ToDto)];
        }
    }
}
