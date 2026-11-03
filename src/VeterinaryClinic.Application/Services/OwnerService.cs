using Microsoft.Extensions.Logging;
using VeterinaryClinic.Contracts.Owners;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Выполняет операции с данными владельца.
/// </summary>
/// <param name="repository">Репозиторий владельцев.</param>
/// <param name="pets">Репозиторий питомцев.</param>
/// <param name="logger">Журнал операций.</param>
public class OwnerService(
    IOwnerRepository repository,
    IPetRepository pets,
    ILogger<OwnerService> logger) : IOwnerService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<OwnerDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Чтение списка владельцев");
        var records = await repository.GetAllAsync(cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }

    /// <inheritdoc />
    public async Task<OwnerDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        logger.LogDebug("Чтение владельца {Id}", id);
        return ClinicMapping.ToDto(await FindAsync(id, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<OwnerDto> CreateAsync(OwnerRequest request, CancellationToken cancellationToken)
    {
        var entity = new Owner
        {
            Id = 0,
            FullName = request.FullName,
            Address = request.Address,
            Phone = request.Phone
        };

        var created = await repository.AddAsync(entity, cancellationToken);
        logger.LogInformation("Создана запись владельца {Id}", created.Id);
        return ClinicMapping.ToDto(created);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, OwnerRequest request, CancellationToken cancellationToken)
    {
        var entity = new Owner
        {
            Id = id,
            FullName = request.FullName,
            Address = request.Address,
            Phone = request.Phone
        };

        if (!await repository.UpdateAsync(entity, cancellationToken))
        {
            throw new EntityNotFoundException("Запись владельца не найдена.");
        }

        logger.LogInformation("Обновлена запись владельца {Id}", id);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (await repository.DeleteAsync(id, cancellationToken))
        {
            logger.LogInformation("Удалена запись владельца {Id}", id);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PetDto>> GetPetsAsync(int id, CancellationToken cancellationToken)
    {
        _ = await FindAsync(id, cancellationToken);
        var records = await pets.GetByOwnerAsync(id, cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }

    /// <summary>
    /// Возвращает существующую запись или сообщает об отсутствии записи.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Найденная запись.</returns>
    private async Task<Owner> FindAsync(int id, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException("Запись владельца не найдена.");
    }
}
