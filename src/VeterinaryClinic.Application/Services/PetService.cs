using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Breeds;
using VeterinaryClinic.Contracts.Owners;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Data;
using VeterinaryClinic.Models;
using VeterinaryClinic.Repositories;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Выполняет операции с данными питомца.
/// </summary>
/// <param name="repository">Репозиторий питомцев.</param>
/// <param name="breeds">Репозиторий пород.</param>
/// <param name="owners">Репозиторий владельцев.</param>
/// <param name="appointments">Репозиторий приёмов.</param>
/// <param name="synchronization">Общая блокировка связанных данных.</param>
/// <param name="logger">Журнал операций.</param>
public class PetService(
    IPetRepository repository,
    IBreedRepository breeds,
    IOwnerRepository owners,
    IAppointmentRepository appointments,
    ClinicLock synchronization,
    ILogger<PetService> logger) : IPetService
{
    /// <inheritdoc />
    public IReadOnlyList<PetDto> GetAll()
    {
        lock (synchronization.SyncRoot)
        {
            logger.LogDebug("Чтение списка питомцев");
            return [.. repository.GetAll().Select(ClinicMapping.ToDto)];
        }
    }

    /// <inheritdoc />
    public PetDto GetById(int id)
    {
        lock (synchronization.SyncRoot)
        {
            logger.LogDebug("Чтение питомца {Id}", id);
            return ClinicMapping.ToDto(Find(id));
        }
    }

    /// <inheritdoc />
    public PetDto Create(PetRequest request)
    {
        lock (synchronization.SyncRoot)
        {
            var breed = breeds.GetById(request.BreedId)
                ?? throw new EntityNotFoundException("Связанная запись породы не найдена.");
            var owner = owners.GetById(request.OwnerId)
                ?? throw new EntityNotFoundException("Связанная запись владельца не найдена.");
            if (request.BirthDate == default || request.BirthDate > DateOnly.FromDateTime(DateTime.Today))
            {
                throw new ValidationException(
                    "Дата рождения питомца должна быть указана и не может быть в будущем.");
            }

            var entity = new Pet
            {
                Id = 0,
                Name = request.Name,
                Breed = breed,
                Owner = owner,
                BirthDate = request.BirthDate,
                Weight = request.Weight
            };

            var created = repository.Add(entity);
            logger.LogInformation("Создана запись питомца {Id}", created.Id);
            return ClinicMapping.ToDto(created);
        }
    }

    /// <inheritdoc />
    public void Update(int id, PetRequest request)
    {
        lock (synchronization.SyncRoot)
        {
            _ = Find(id);
            var breed = breeds.GetById(request.BreedId)
                ?? throw new EntityNotFoundException("Связанная запись породы не найдена.");
            var owner = owners.GetById(request.OwnerId)
                ?? throw new EntityNotFoundException("Связанная запись владельца не найдена.");
            if (request.BirthDate == default || request.BirthDate > DateOnly.FromDateTime(DateTime.Today))
            {
                throw new ValidationException(
                    "Дата рождения питомца должна быть указана и не может быть в будущем.");
            }

            if (appointments.GetByPet(id)
                .Any(appointment => DateOnly.FromDateTime(appointment.StartsAt) < request.BirthDate))
            {
                throw new ValidationException("Дата рождения не может быть позже существующего приёма питомца.");
            }

            var entity = new Pet
            {
                Id = id,
                Name = request.Name,
                Breed = breed,
                Owner = owner,
                BirthDate = request.BirthDate,
                Weight = request.Weight
            };

            repository.Update(entity);
            logger.LogInformation("Обновлена запись питомца {Id}", id);
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

            if (appointments.GetByPet(id).Count > 0)
            {
                throw new EntityInUseException("Запись питомца используется другими записями.");
            }

            repository.Delete(id);
            logger.LogInformation("Удалена запись питомца {Id}", id);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<AppointmentDto> GetAppointments(int id)
    {
        lock (synchronization.SyncRoot)
        {
            _ = Find(id);
            return [.. appointments.GetByPet(id).Select(ClinicMapping.ToDto)];
        }
    }

    /// <inheritdoc />
    public OwnerDto GetOwner(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return ClinicMapping.ToDto(Find(id).Owner);
        }
    }

    /// <inheritdoc />
    public BreedDto GetBreed(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return ClinicMapping.ToDto(Find(id).Breed);
        }
    }

    /// <summary>
    /// Возвращает существующую запись или сообщает об отсутствии записи.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Найденная запись.</returns>
    private Pet Find(int id)
    {
        return repository.GetById(id)
            ?? throw new EntityNotFoundException("Запись питомца не найдена.");
    }
}
