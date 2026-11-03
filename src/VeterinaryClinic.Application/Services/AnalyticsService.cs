using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using VeterinaryClinic.Contracts.Analytics;
using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Contracts.Veterinarians;
using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Domain.Repositories;
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
/// <param name="logger">Журнал аналитических запросов.</param>
public class AnalyticsService(
    IVeterinarianRepository veterinarians,
    IPetRepository pets,
    IBreedRepository breeds,
    IOwnerRepository owners,
    IAppointmentRepository appointments,
    ILogger<AnalyticsService> logger) : IAnalyticsService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<VeterinarianDto>> GetVeterinariansAsync(
        AnimalSpecies species,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Выборка врачей по виду {Species}", species);
        var records = await veterinarians.GetBySpeciesAsync(species, cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PetDto>> GetPetsAsync(int veterinarianId, CancellationToken cancellationToken)
    {
        _ = await veterinarians.GetByIdAsync(veterinarianId, cancellationToken)
            ?? throw new EntityNotFoundException("Ветеринар не найден.");
        logger.LogInformation("Выборка питомцев врача {Id}", veterinarianId);
        var records = await pets.GetByVeterinarianAsync(veterinarianId, cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }

    /// <inheritdoc />
    public async Task<FollowUpCountDto> GetFollowUpCountAsync(int breedId, CancellationToken cancellationToken)
    {
        _ = await breeds.GetByIdAsync(breedId, cancellationToken)
            ?? throw new EntityNotFoundException("Порода не найдена.");
        logger.LogInformation("Подсчёт повторных приёмов породы {Id}", breedId);
        var count = await appointments.CountFollowUpsByBreedAsync(breedId, cancellationToken);
        return new(breedId, count);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<OwnerWithPetCountDto>> GetOwnersWithMultiplePetsAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Выборка владельцев нескольких питомцев");
        var records = await owners.GetWithMultiplePetsAsync(cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AppointmentDto>> GetAppointmentsAsync(
        DateOnly asOfDate,
        string roomNumber,
        CancellationToken cancellationToken)
    {
        if (asOfDate == default || (asOfDate.Year == 9999 && asOfDate.Month == 12))
        {
            throw new ValidationException("Укажите дату, для которой можно определить конец месячного периода.");
        }

        var room = roomNumber.Trim();
        var from = asOfDate.ToDateTime(TimeOnly.MinValue);
        var to = from.AddMonths(1);

        logger.LogInformation("Выборка приёмов за период {From} - {To}", from, to);
        var records = await appointments.GetForPeriodAsync(from, to, room, cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }
}
