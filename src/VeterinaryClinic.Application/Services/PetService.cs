using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Breeds;
using VeterinaryClinic.Contracts.Owners;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Выполняет операции с данными питомца.
/// </summary>
/// <param name="repository">Репозиторий питомцев.</param>
/// <param name="breeds">Репозиторий пород.</param>
/// <param name="owners">Репозиторий владельцев.</param>
/// <param name="appointments">Репозиторий приёмов.</param>
/// <param name="logger">Журнал операций.</param>
public class PetService(
    IPetRepository repository,
    IBreedRepository breeds,
    IOwnerRepository owners,
    IAppointmentRepository appointments,
    ILogger<PetService> logger) : IPetService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<PetDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Чтение списка питомцев");
        var records = await repository.GetAllAsync(cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }

    /// <inheritdoc />
    public async Task<PetDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        logger.LogDebug("Чтение питомца {Id}", id);
        return ClinicMapping.ToDto(await FindAsync(id, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<PetDto> CreateAsync(PetRequest request, CancellationToken cancellationToken)
    {
        var entity = await BuildPetAsync(0, request, cancellationToken);
        var created = await repository.AddAsync(entity, cancellationToken);
        logger.LogInformation("Создана запись питомца {Id}", created.Id);
        return ClinicMapping.ToDto(created);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, PetRequest request, CancellationToken cancellationToken)
    {
        var current = await FindAsync(id, cancellationToken);
        var entity = await BuildPetAsync(id, request, cancellationToken);
        if (entity.BirthDate > current.BirthDate
            && await appointments.HasAppointmentBeforeAsync(id, entity.BirthDate, cancellationToken))
        {
            throw new ValidationException("Дата рождения не может быть позже существующего приёма питомца.");
        }

        if (!await repository.UpdateAsync(entity, cancellationToken))
        {
            throw new EntityNotFoundException("Запись питомца не найдена.");
        }

        logger.LogInformation("Обновлена запись питомца {Id}", id);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (await repository.DeleteAsync(id, cancellationToken))
        {
            logger.LogInformation("Удалена запись питомца {Id}", id);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AppointmentDto>> GetAppointmentsAsync(
        int id,
        CancellationToken cancellationToken)
    {
        _ = await FindAsync(id, cancellationToken);
        var records = await appointments.GetByPetAsync(id, cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }

    /// <inheritdoc />
    public async Task<OwnerDto> GetOwnerAsync(int id, CancellationToken cancellationToken)
    {
        var pet = await FindAsync(id, cancellationToken);
        return ClinicMapping.ToDto(pet.Owner);
    }

    /// <inheritdoc />
    public async Task<BreedDto> GetBreedAsync(int id, CancellationToken cancellationToken)
    {
        var pet = await FindAsync(id, cancellationToken);
        return ClinicMapping.ToDto(pet.Breed);
    }

    /// <summary>
    /// Проверяет данные питомца и связывает запись с существующими сущностями.
    /// </summary>
    /// <param name="id">Идентификатор записи или ноль для новой записи.</param>
    /// <param name="request">Данные питомца из запроса.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Проверенная запись для сохранения.</returns>
    private async Task<Pet> BuildPetAsync(
        int id,
        PetRequest request,
        CancellationToken cancellationToken)
    {
        var breed = await breeds.GetByIdAsync(request.BreedId, cancellationToken)
            ?? throw new EntityNotFoundException("Связанная запись породы не найдена.");
        var owner = await owners.GetByIdAsync(request.OwnerId, cancellationToken)
            ?? throw new EntityNotFoundException("Связанная запись владельца не найдена.");
        if (request.BirthDate == default || request.BirthDate > DateOnly.FromDateTime(DateTime.Today))
        {
            throw new ValidationException(
                "Дата рождения питомца должна быть указана и не может быть в будущем.");
        }

        return new()
        {
            Id = id,
            Name = request.Name,
            Breed = breed,
            Owner = owner,
            BirthDate = request.BirthDate,
            Weight = request.Weight
        };
    }

    /// <summary>
    /// Возвращает существующую запись или сообщает об отсутствии записи.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Найденная запись.</returns>
    private async Task<Pet> FindAsync(int id, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException("Запись питомца не найдена.");
    }
}
