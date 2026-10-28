using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Specializations;
using VeterinaryClinic.Contracts.Veterinarians;
using VeterinaryClinic.Data;
using VeterinaryClinic.Models;
using VeterinaryClinic.Repositories;

namespace VeterinaryClinic.Application.Services;

/// <summary>
/// Выполняет операции с данными ветеринара.
/// </summary>
/// <param name="repository">Репозиторий ветеринаров.</param>
/// <param name="specializations">Репозиторий специализаций.</param>
/// <param name="appointments">Репозиторий приёмов.</param>
/// <param name="synchronization">Общая блокировка связанных данных.</param>
/// <param name="logger">Журнал операций.</param>
public class VeterinarianService(
    IVeterinarianRepository repository,
    ISpecializationRepository specializations,
    IAppointmentRepository appointments,
    ClinicLock synchronization,
    ILogger<VeterinarianService> logger) : IVeterinarianService
{
    /// <inheritdoc />
    public IReadOnlyList<VeterinarianDto> GetAll()
    {
        lock (synchronization.SyncRoot)
        {
            logger.LogDebug("Чтение списка ветеринаров");
            return [.. repository.GetAll().Select(ClinicMapping.ToDto)];
        }
    }

    /// <inheritdoc />
    public VeterinarianDto GetById(int id)
    {
        lock (synchronization.SyncRoot)
        {
            logger.LogDebug("Чтение ветеринара {Id}", id);
            return ClinicMapping.ToDto(Find(id));
        }
    }

    /// <inheritdoc />
    public VeterinarianDto Create(VeterinarianRequest request)
    {
        lock (synchronization.SyncRoot)
        {
            var specialization = specializations.GetById(request.SpecializationId)
                ?? throw new EntityNotFoundException("Связанная запись специализации не найдена.");
            if (request.BirthYear > DateTime.Today.Year
                || request.ExperienceYears > DateTime.Today.Year - request.BirthYear)
            {
                throw new ValidationException("Год рождения или стаж ветеринара указан неверно.");
            }

            var entity = new Veterinarian
            {
                Id = 0,
                PassportNumber = request.PassportNumber,
                FullName = request.FullName,
                BirthYear = request.BirthYear,
                Specialization = specialization,
                ExperienceYears = request.ExperienceYears
            };

            var created = repository.Add(entity);
            logger.LogInformation("Создана запись ветеринара {Id}", created.Id);
            return ClinicMapping.ToDto(created);
        }
    }

    /// <inheritdoc />
    public void Update(int id, VeterinarianRequest request)
    {
        lock (synchronization.SyncRoot)
        {
            _ = Find(id);
            var specialization = specializations.GetById(request.SpecializationId)
                ?? throw new EntityNotFoundException("Связанная запись специализации не найдена.");
            if (request.BirthYear > DateTime.Today.Year
                || request.ExperienceYears > DateTime.Today.Year - request.BirthYear)
            {
                throw new ValidationException("Год рождения или стаж ветеринара указан неверно.");
            }

            var entity = new Veterinarian
            {
                Id = id,
                PassportNumber = request.PassportNumber,
                FullName = request.FullName,
                BirthYear = request.BirthYear,
                Specialization = specialization,
                ExperienceYears = request.ExperienceYears
            };

            repository.Update(entity);
            logger.LogInformation("Обновлена запись ветеринара {Id}", id);
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

            if (appointments.GetByVeterinarian(id).Count > 0)
            {
                throw new EntityInUseException("Запись ветеринара используется другими записями.");
            }

            repository.Delete(id);
            logger.LogInformation("Удалена запись ветеринара {Id}", id);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<AppointmentDto> GetAppointments(int id)
    {
        lock (synchronization.SyncRoot)
        {
            _ = Find(id);
            return [.. appointments.GetByVeterinarian(id).Select(ClinicMapping.ToDto)];
        }
    }

    /// <inheritdoc />
    public SpecializationDto GetSpecialization(int id)
    {
        lock (synchronization.SyncRoot)
        {
            return ClinicMapping.ToDto(Find(id).Specialization);
        }
    }

    /// <summary>
    /// Возвращает существующую запись или сообщает об отсутствии записи.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Найденная запись.</returns>
    private Veterinarian Find(int id)
    {
        return repository.GetById(id)
            ?? throw new EntityNotFoundException("Запись ветеринара не найдена.");
    }
}
