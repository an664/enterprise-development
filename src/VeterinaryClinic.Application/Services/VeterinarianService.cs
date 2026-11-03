using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Specializations;
using VeterinaryClinic.Contracts.Veterinarians;
using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Выполняет операции с данными ветеринара.
/// </summary>
/// <param name="repository">Репозиторий ветеринаров.</param>
/// <param name="specializations">Репозиторий специализаций.</param>
/// <param name="appointments">Репозиторий приёмов.</param>
/// <param name="logger">Журнал операций.</param>
public class VeterinarianService(
    IVeterinarianRepository repository,
    ISpecializationRepository specializations,
    IAppointmentRepository appointments,
    ILogger<VeterinarianService> logger) : IVeterinarianService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<VeterinarianDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Чтение списка ветеринаров");
        var records = await repository.GetAllAsync(cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }

    /// <inheritdoc />
    public async Task<VeterinarianDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        logger.LogDebug("Чтение ветеринара {Id}", id);
        return ClinicMapping.ToDto(await FindAsync(id, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<VeterinarianDto> CreateAsync(VeterinarianRequest request, CancellationToken cancellationToken)
    {
        var entity = await BuildVeterinarianAsync(0, request, cancellationToken);
        var created = await repository.AddAsync(entity, cancellationToken);
        logger.LogInformation("Создана запись ветеринара {Id}", created.Id);
        return ClinicMapping.ToDto(created);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, VeterinarianRequest request, CancellationToken cancellationToken)
    {
        var entity = await BuildVeterinarianAsync(id, request, cancellationToken);

        if (!await repository.UpdateAsync(entity, cancellationToken))
        {
            throw new EntityNotFoundException("Запись ветеринара не найдена.");
        }

        logger.LogInformation("Обновлена запись ветеринара {Id}", id);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (await repository.DeleteAsync(id, cancellationToken))
        {
            logger.LogInformation("Удалена запись ветеринара {Id}", id);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AppointmentDto>> GetAppointmentsAsync(
        int id,
        CancellationToken cancellationToken)
    {
        _ = await FindAsync(id, cancellationToken);
        var records = await appointments.GetByVeterinarianAsync(id, cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }

    /// <inheritdoc />
    public async Task<SpecializationDto> GetSpecializationAsync(int id, CancellationToken cancellationToken)
    {
        var veterinarian = await FindAsync(id, cancellationToken);
        return ClinicMapping.ToDto(veterinarian.Specialization);
    }

    /// <summary>
    /// Проверяет данные ветеринара и связывает запись с существующими сущностями.
    /// </summary>
    /// <param name="id">Идентификатор записи или ноль для новой записи.</param>
    /// <param name="request">Данные ветеринара из запроса.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Проверенная запись для сохранения.</returns>
    private async Task<Veterinarian> BuildVeterinarianAsync(
        int id,
        VeterinarianRequest request,
        CancellationToken cancellationToken)
    {
        var specialization = await specializations.GetByIdAsync(request.SpecializationId, cancellationToken)
            ?? throw new EntityNotFoundException("Связанная запись специализации не найдена.");
        if (request.BirthYear > DateTime.Today.Year
            || request.ExperienceYears > DateTime.Today.Year - request.BirthYear)
        {
            throw new ValidationException("Год рождения или стаж ветеринара указан неверно.");
        }

        return new()
        {
            Id = id,
            PassportNumber = request.PassportNumber,
            FullName = request.FullName,
            BirthYear = request.BirthYear,
            Specialization = specialization,
            ExperienceYears = request.ExperienceYears
        };
    }

    /// <summary>
    /// Возвращает существующую запись или сообщает об отсутствии записи.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Найденная запись.</returns>
    private async Task<Veterinarian> FindAsync(int id, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException("Запись ветеринара не найдена.");
    }
}
