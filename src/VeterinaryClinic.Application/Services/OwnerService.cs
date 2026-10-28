using Microsoft.Extensions.Logging;
using VeterinaryClinic.Contracts.Owners;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Data;
using VeterinaryClinic.Models;
using VeterinaryClinic.Repositories;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Выполняет операции с данными владельца.
/// </summary>
/// <param name="repository">Репозиторий владельцев.</param>
/// <param name="pets">Репозиторий питомцев.</param>
/// <param name="synchronization">Общая блокировка связанных данных.</param>
/// <param name="logger">Журнал операций.</param>
public class OwnerService(
    IOwnerRepository repository,
    IPetRepository pets,
    ClinicLock synchronization,
    ILogger<OwnerService> logger) : IOwnerService
{
    /// <inheritdoc />
    public IReadOnlyList<OwnerDto> GetAll()
    {
        lock (synchronization.SyncRoot)
        {
            logger.LogDebug("Чтение списка владельцев");
            return [.. repository.GetAll().Select(ClinicMapping.ToDto)];
        }
    }

    /// <inheritdoc />
    public OwnerDto GetById(int id)
    {
        lock (synchronization.SyncRoot)
        {
            logger.LogDebug("Чтение владельца {Id}", id);
            return ClinicMapping.ToDto(Find(id));
        }
    }

    /// <inheritdoc />
    public OwnerDto Create(OwnerRequest request)
    {
        lock (synchronization.SyncRoot)
        {

            var entity = new Owner
            {
                Id = 0,
                FullName = request.FullName,
                Address = request.Address,
                Phone = request.Phone
            };

            var created = repository.Add(entity);
            logger.LogInformation("Создана запись владельца {Id}", created.Id);
            return ClinicMapping.ToDto(created);
        }
    }

    /// <inheritdoc />
    public void Update(int id, OwnerRequest request)
    {
        lock (synchronization.SyncRoot)
        {
            _ = Find(id);

            var entity = new Owner
            {
                Id = id,
                FullName = request.FullName,
                Address = request.Address,
                Phone = request.Phone
            };

            repository.Update(entity);
            logger.LogInformation("Обновлена запись владельца {Id}", id);
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

            if (pets.GetByOwner(id).Count > 0)
            {
                throw new EntityInUseException("Запись владельца используется другими записями.");
            }

            repository.Delete(id);
            logger.LogInformation("Удалена запись владельца {Id}", id);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<PetDto> GetPets(int id)
    {
        lock (synchronization.SyncRoot)
        {
            _ = Find(id);
            return [.. pets.GetByOwner(id).Select(ClinicMapping.ToDto)];
        }
    }

    /// <summary>
    /// Возвращает существующую запись или сообщает об отсутствии записи.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Найденная запись.</returns>
    private Owner Find(int id)
    {
        return repository.GetById(id)
            ?? throw new EntityNotFoundException("Запись владельца не найдена.");
    }
}
