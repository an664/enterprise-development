using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Contracts.Veterinarians;
using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Выполняет операции с данными приёма.
/// </summary>
/// <param name="repository">Репозиторий приёмов.</param>
/// <param name="pets">Репозиторий питомцев.</param>
/// <param name="veterinarians">Репозиторий ветеринаров.</param>
/// <param name="logger">Журнал операций.</param>
public class AppointmentService(
    IAppointmentRepository repository,
    IPetRepository pets,
    IVeterinarianRepository veterinarians,
    ILogger<AppointmentService> logger) : IAppointmentService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<AppointmentDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Чтение списка приёмов");
        var records = await repository.GetAllAsync(cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }

    /// <inheritdoc />
    public async Task<AppointmentDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        logger.LogDebug("Чтение приёма {Id}", id);
        return ClinicMapping.ToDto(await FindAsync(id, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<AppointmentDto> CreateAsync(AppointmentRequest request, CancellationToken cancellationToken)
    {
        var entity = await BuildAppointmentAsync(0, request, cancellationToken);
        var created = await repository.AddAsync(entity, cancellationToken);
        logger.LogInformation("Создана запись приёма {Id}", created.Id);
        return ClinicMapping.ToDto(created);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, AppointmentRequest request, CancellationToken cancellationToken)
    {
        _ = await FindAsync(id, cancellationToken);
        var entity = await BuildAppointmentAsync(id, request, cancellationToken);

        if (!await repository.UpdateAsync(entity, cancellationToken))
        {
            throw new EntityNotFoundException("Запись приёма не найдена.");
        }

        logger.LogInformation("Обновлена запись приёма {Id}", id);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (await repository.DeleteAsync(id, cancellationToken))
        {
            logger.LogInformation("Удалена запись приёма {Id}", id);
        }
    }

    /// <inheritdoc />
    public async Task<PetDto> GetPetAsync(int id, CancellationToken cancellationToken)
    {
        var appointment = await FindAsync(id, cancellationToken);
        return ClinicMapping.ToDto(appointment.Pet);
    }

    /// <inheritdoc />
    public async Task<VeterinarianDto> GetVeterinarianAsync(int id, CancellationToken cancellationToken)
    {
        var appointment = await FindAsync(id, cancellationToken);
        return ClinicMapping.ToDto(appointment.Veterinarian);
    }

    /// <summary>
    /// Проверяет данные приёма и связывает запись с существующими сущностями.
    /// </summary>
    /// <param name="id">Идентификатор записи или ноль для новой записи.</param>
    /// <param name="request">Данные приёма из запроса.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Проверенная запись для сохранения.</returns>
    private async Task<Appointment> BuildAppointmentAsync(
        int id,
        AppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var pet = await pets.GetByIdAsync(request.PetId, cancellationToken)
            ?? throw new EntityNotFoundException("Связанная запись питомца не найдена.");
        var veterinarian = await veterinarians.GetByIdAsync(request.VeterinarianId, cancellationToken)
            ?? throw new EntityNotFoundException("Связанная запись ветеринара не найдена.");
        if (request.StartsAt == default || request.StartsAt.Kind != DateTimeKind.Unspecified)
        {
            throw new ValidationException("Укажите местные дату и время приёма без смещения часового пояса.");
        }

        if (request.StartsAt.Ticks % TimeSpan.TicksPerMicrosecond != 0)
        {
            throw new ValidationException("Допустимая точность времени приёма - до микросекунд.");
        }

        if (DateOnly.FromDateTime(request.StartsAt) < pet.BirthDate)
        {
            throw new ValidationException("Приём не может быть раньше рождения питомца.");
        }

        return new()
        {
            Id = id,
            Pet = pet,
            Veterinarian = veterinarian,
            StartsAt = request.StartsAt,
            RoomNumber = request.RoomNumber.Trim(),
            IsFollowUp = request.IsFollowUp
        };
    }

    /// <summary>
    /// Возвращает существующую запись или сообщает об отсутствии записи.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Найденная запись.</returns>
    private async Task<Appointment> FindAsync(int id, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException("Запись приёма не найдена.");
    }
}
