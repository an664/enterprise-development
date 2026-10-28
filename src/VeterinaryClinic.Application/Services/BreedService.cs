using Microsoft.Extensions.Logging;
using VeterinaryClinic.Contracts.Breeds;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Data;
using VeterinaryClinic.Models;
using VeterinaryClinic.Repositories;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Выполняет операции с данными породы.
/// </summary>
/// <param name="repository">Репозиторий пород.</param>
/// <param name="pets">Репозиторий питомцев.</param>
/// <param name="synchronization">Общая блокировка связанных данных.</param>
/// <param name="logger">Журнал операций.</param>
public class BreedService(
    IBreedRepository repository,
    IPetRepository pets,
    ClinicLock synchronization,
    ILogger<BreedService> logger) : IBreedService
{
    /// <inheritdoc />
    public IReadOnlyList<BreedDto> GetAll()
    {
        lock (synchronization.SyncRoot)
        {
            logger.LogDebug("Чтение списка пород");
            return [.. repository.GetAll().Select(ClinicMapping.ToDto)];
        }
    }

    /// <inheritdoc />
    public BreedDto GetById(int id)
    {
        lock (synchronization.SyncRoot)
        {
            logger.LogDebug("Чтение породы {Id}", id);
            return ClinicMapping.ToDto(Find(id));
        }
    }

    /// <inheritdoc />
    public BreedDto Create(BreedRequest request)
    {
        lock (synchronization.SyncRoot)
        {

            var entity = new Breed
            {
                Id = 0,
                Name = request.Name,
                Species = request.Species
            };

            var created = repository.Add(entity);
            logger.LogInformation("Создана запись породы {Id}", created.Id);
            return ClinicMapping.ToDto(created);
        }
    }

    /// <inheritdoc />
    public void Update(int id, BreedRequest request)
    {
        lock (synchronization.SyncRoot)
        {
            _ = Find(id);

            var entity = new Breed
            {
                Id = id,
                Name = request.Name,
                Species = request.Species
            };

            repository.Update(entity);
            logger.LogInformation("Обновлена запись породы {Id}", id);
        }
    }

    /// <inheritdoc />
    public void Delete(int id)
    {
        lock (synchronization.SyncRoot)
        {
            if (repository.GetById(id) is null)
            {
                return;
            }

            if (pets.GetByBreed(id).Count > 0)
            {
                throw new EntityInUseException("Запись породы используется другими записями.");
            }

            repository.Delete(id);
            logger.LogInformation("Удалена запись породы {Id}", id);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<PetDto> GetPets(int id)
    {
        lock (synchronization.SyncRoot)
        {
            _ = Find(id);
            return [.. pets.GetByBreed(id).Select(ClinicMapping.ToDto)];
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<BreedDto> GetBySpecies(AnimalSpecies species)
    {
        lock (synchronization.SyncRoot)
        {
            return [.. repository.GetBySpecies(species).Select(ClinicMapping.ToDto)];
        }
    }

    /// <summary>
    /// Возвращает существующую запись или сообщает об отсутствии записи.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Найденная запись.</returns>
    private Breed Find(int id)
    {
        return repository.GetById(id)
            ?? throw new EntityNotFoundException("Запись породы не найдена.");
    }
}
