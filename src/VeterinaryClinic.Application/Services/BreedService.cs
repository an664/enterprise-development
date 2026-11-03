using Microsoft.Extensions.Logging;
using VeterinaryClinic.Contracts.Breeds;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Domain.Exceptions;
using VeterinaryClinic.Domain.Models;
using VeterinaryClinic.Domain.Repositories;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Выполняет операции с данными породы.
/// </summary>
/// <param name="repository">Репозиторий пород.</param>
/// <param name="pets">Репозиторий питомцев.</param>
/// <param name="logger">Журнал операций.</param>
public class BreedService(
    IBreedRepository repository,
    IPetRepository pets,
    ILogger<BreedService> logger) : IBreedService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<BreedDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Чтение списка пород");
        var records = await repository.GetAllAsync(cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }

    /// <inheritdoc />
    public async Task<BreedDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        logger.LogDebug("Чтение породы {Id}", id);
        return ClinicMapping.ToDto(await FindAsync(id, cancellationToken));
    }

    /// <inheritdoc />
    public async Task<BreedDto> CreateAsync(BreedRequest request, CancellationToken cancellationToken)
    {
        var entity = new Breed
        {
            Id = 0,
            Name = request.Name,
            Species = request.Species
        };

        var created = await repository.AddAsync(entity, cancellationToken);
        logger.LogInformation("Создана запись породы {Id}", created.Id);
        return ClinicMapping.ToDto(created);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, BreedRequest request, CancellationToken cancellationToken)
    {
        var entity = new Breed
        {
            Id = id,
            Name = request.Name,
            Species = request.Species
        };

        if (!await repository.UpdateAsync(entity, cancellationToken))
        {
            throw new EntityNotFoundException("Запись породы не найдена.");
        }

        logger.LogInformation("Обновлена запись породы {Id}", id);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (await repository.DeleteAsync(id, cancellationToken))
        {
            logger.LogInformation("Удалена запись породы {Id}", id);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PetDto>> GetPetsAsync(int id, CancellationToken cancellationToken)
    {
        _ = await FindAsync(id, cancellationToken);
        var records = await pets.GetByBreedAsync(id, cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<BreedDto>> GetBySpeciesAsync(
        AnimalSpecies species,
        CancellationToken cancellationToken)
    {
        var records = await repository.GetBySpeciesAsync(species, cancellationToken);
        return [.. records.Select(ClinicMapping.ToDto)];
    }

    /// <summary>
    /// Возвращает существующую запись или сообщает об отсутствии записи.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Найденная запись.</returns>
    private async Task<Breed> FindAsync(int id, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException("Запись породы не найдена.");
    }
}
