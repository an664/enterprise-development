using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Contracts.Veterinarians;
using VeterinaryClinic.Data;
using VeterinaryClinic.Models;
using VeterinaryClinic.Repositories;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Выполняет операции с данными приёма.
/// </summary>
/// <param name="repository">Репозиторий приёмов.</param>
/// <param name="pets">Репозиторий питомцев.</param>
/// <param name="veterinarians">Репозиторий ветеринаров.</param>
/// <param name="synchronization">Общая блокировка связанных данных.</param>
/// <param name="logger">Журнал операций.</param>
public class AppointmentService(
    IAppointmentRepository repository,
    IPetRepository pets,
    IVeterinarianRepository veterinarians,
    ClinicLock synchronization,
    ILogger<AppointmentService> logger) : IAppointmentService
{
    /// <inheritdoc />
    public IReadOnlyList<AppointmentDto> GetAll()
    {
        lock (synchronization.SyncRoot)
        {
            logger.LogDebug("Чтение списка приёмов");
            return [.. repository.GetAll().Select(ClinicMapping.ToDto)];
        }
    }

    /// <inheritdoc />
    public AppointmentDto GetById(int id)
    {
        lock (synchronization.SyncRoot)
        {
            logger.LogDebug("Чтение приёма {Id}", id);
            return ClinicMapping.ToDto(Find(id));
        }
    }

    /// <inheritdoc />
    public AppointmentDto Create(AppointmentRequest request)
    {
        lock (synchronization.SyncRoot)
        {
            var pet = pets.GetById(request.PetId)
                ?? throw new EntityNotFoundException("Связанная запись питомца не найдена.");
            var veterinarian = veterinarians.GetById(request.VeterinarianId)
                ?? throw new EntityNotFoundException("Связанная запись ветеринара не найдена.");
            if (request.StartsAt == default || request.StartsAt.Kind != DateTimeKind.Unspecified)
            {
                throw new ValidationException("Укажите местные дату и время приёма без смещения часового пояса.");
            }

            if (DateOnly.FromDateTime(request.StartsAt) < pet.BirthDate)
            {
                throw new ValidationException("Приём не может быть раньше рождения питомца.");
            }

            var entity = new Appointment
            {
                Id = 0,
                Pet = pet,
                Veterinarian = veterinarian,
                StartsAt = request.StartsAt,
                RoomNumber = request.RoomNumber,
                IsFollowUp = request.IsFollowUp
            };

            var created = repository.Add(entity);
            logger.LogInformation("Создана запись приёма {Id}", created.Id);
            return ClinicMapping.ToDto(created);
        }
    }

    /// <inheritdoc />
    public void Update(int id, AppointmentRequest request)
    {
        lock (synchronization.SyncRoot)
        {
            _ = Find(id);
            var pet = pets.GetById(request.PetId)
                ?? throw new EntityNotFoundException("Связанная запись питомца не найдена.");
            var veterinarian = veterinarians.GetById(request.VeterinarianId)
                ?? throw new EntityNotFoundException("Связанная запись ветеринара не найдена.");
            if (request.StartsAt == default || request.StartsAt.Kind != DateTimeKind.Unspecified)
            {
                throw new ValidationException("Укажите местные дату и время приёма без смещения часового пояса.");
            }

            if (DateOnly.FromDateTime(request.StartsAt) < pet.BirthDate)
            {
                throw new ValidationException("Приём не может быть раньше рождения питомца.");
            }

            var entity = new Appointment
            {
                Id = id,
                Pet = pet,
                Veterinarian = veterinarian,
                StartsAt = request.StartsAt,
                RoomNumber = request.RoomNumber,
                IsFollowUp = request.IsFollowUp
            };

            repository.Update(entity);
            logger.LogInformation("Обновлена запись приёма {Id}", id);
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

            repository.Delete(id);
            logger.LogInformation("Удалена запись приёма {Id}", id);
        }
    }

    /// <inheritdoc />
    public PetDto GetPet(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return ClinicMapping.ToDto(Find(id).Pet);
        }
    }

    /// <inheritdoc />
    public VeterinarianDto GetVeterinarian(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return ClinicMapping.ToDto(Find(id).Veterinarian);
        }
    }

    /// <summary>
    /// Возвращает существующую запись или сообщает об отсутствии записи.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Найденная запись.</returns>
    private Appointment Find(int id)
    {
        return repository.GetById(id)
            ?? throw new EntityNotFoundException("Запись приёма не найдена.");
    }
}
