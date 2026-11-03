using Microsoft.Extensions.Logging;
using VeterinaryClinic.Contracts.Specializations;
using VeterinaryClinic.Contracts.Veterinarians;
using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Выполняет операции с данными специализации.
/// </summary>
/// <param name="repository">Репозиторий специализаций.</param>
/// <param name="veterinarians">Репозиторий ветеринаров.</param>
/// <param name="logger">Журнал операций.</param>
public class SpecializationService(
    ISpecializationRepository repository,
    IVeterinarianRepository veterinarians,
    ILogger<SpecializationService> logger) : ISpecializationService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<SpecializationDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Чтение списка специализаций");
        var records = await repository.GetAllAsync(cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }

    /// <inheritdoc />
    public async Task<SpecializationDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        logger.LogDebug("Чтение специализации {Id}", id);
        return ClinicMapping.ToDto(await FindAsync(id, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<SpecializationDto> CreateAsync(
        SpecializationRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new Specialization
        {
            Id = 0,
            Name = request.Name,
            Species = request.Species
        };

        var created = await repository.AddAsync(entity, cancellationToken);
        logger.LogInformation("Создана запись специализации {Id}", created.Id);
        return ClinicMapping.ToDto(created);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, SpecializationRequest request, CancellationToken cancellationToken)
    {
        var entity = new Specialization
        {
            Id = id,
            Name = request.Name,
            Species = request.Species
        };

        if (!await repository.UpdateAsync(entity, cancellationToken))
        {
            throw new EntityNotFoundException("Запись специализации не найдена.");
        }

        logger.LogInformation("Обновлена запись специализации {Id}", id);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (await repository.DeleteAsync(id, cancellationToken))
        {
            logger.LogInformation("Удалена запись специализации {Id}", id);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<VeterinarianDto>> GetVeterinariansAsync(
        int id,
        CancellationToken cancellationToken)
    {
        _ = await FindAsync(id, cancellationToken);
        var records = await veterinarians.GetBySpecializationAsync(id, cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }

    /// <summary>
    /// Возвращает существующую запись или сообщает об отсутствии записи.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Найденная запись.</returns>
    private async Task<Specialization> FindAsync(int id, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException("Запись специализации не найдена.");
    }
}
