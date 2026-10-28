using Microsoft.Extensions.Logging;
using VeterinaryClinic.Contracts.Specializations;
using VeterinaryClinic.Contracts.Veterinarians;
using VeterinaryClinic.Data;
using VeterinaryClinic.Models;
using VeterinaryClinic.Repositories;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Выполняет операции с данными специализации.
/// </summary>
/// <param name="repository">Репозиторий специализаций.</param>
/// <param name="veterinarians">Репозиторий ветеринаров.</param>
/// <param name="synchronization">Общая блокировка связанных данных.</param>
/// <param name="logger">Журнал операций.</param>
public class SpecializationService(
    ISpecializationRepository repository,
    IVeterinarianRepository veterinarians,
    ClinicLock synchronization,
    ILogger<SpecializationService> logger) : ISpecializationService
{
    /// <inheritdoc />
    public IReadOnlyList<SpecializationDto> GetAll()
    {
        lock (synchronization.SyncRoot)
        {
            logger.LogDebug("Чтение списка специализаций");
            return [.. repository.GetAll().Select(ClinicMapping.ToDto)];
        }
    }

    /// <inheritdoc />
    public SpecializationDto GetById(int id)
    {
        lock (synchronization.SyncRoot)
        {
            logger.LogDebug("Чтение специализации {Id}", id);
            return ClinicMapping.ToDto(Find(id));
        }
    }

    /// <inheritdoc />
    public SpecializationDto Create(SpecializationRequest request)
    {
        lock (synchronization.SyncRoot)
        {

            var entity = new Specialization
            {
                Id = 0,
                Name = request.Name,
                Species = request.Species
            };

            var created = repository.Add(entity);
            logger.LogInformation("Создана запись специализации {Id}", created.Id);
            return ClinicMapping.ToDto(created);
        }
    }

    /// <inheritdoc />
    public void Update(int id, SpecializationRequest request)
    {
        lock (synchronization.SyncRoot)
        {
            _ = Find(id);

            var entity = new Specialization
            {
                Id = id,
                Name = request.Name,
                Species = request.Species
            };

            repository.Update(entity);
            logger.LogInformation("Обновлена запись специализации {Id}", id);
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

            if (veterinarians.GetBySpecialization(id).Count > 0)
            {
                throw new EntityInUseException("Запись специализации используется другими записями.");
            }

            repository.Delete(id);
            logger.LogInformation("Удалена запись специализации {Id}", id);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<VeterinarianDto> GetVeterinarians(int id)
    {
        lock (synchronization.SyncRoot)
        {
            _ = Find(id);
            return [.. veterinarians.GetBySpecialization(id).Select(ClinicMapping.ToDto)];
        }
    }

    /// <summary>
    /// Возвращает существующую запись или сообщает об отсутствии записи.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Найденная запись.</returns>
    private Specialization Find(int id)
    {
        return repository.GetById(id)
            ?? throw new EntityNotFoundException("Запись специализации не найдена.");
    }
}
