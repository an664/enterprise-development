using VeterinaryClinic.Data;
using VeterinaryClinic.Models;

namespace VeterinaryClinic.Infrastructure;

/// <summary>
/// Хранит связанные сущности ветеринарной клиники в памяти.
/// </summary>
public class ClinicStore
{
    /// <summary>
    /// Коллекция пород по идентификаторам.
    /// </summary>
    internal Dictionary<int, Breed> Breeds { get; }

    /// <summary>
    /// Коллекция владельцев по идентификаторам.
    /// </summary>
    internal Dictionary<int, Owner> Owners { get; }

    /// <summary>
    /// Коллекция питомцев по идентификаторам.
    /// </summary>
    internal Dictionary<int, Pet> Pets { get; }

    /// <summary>
    /// Коллекция специализаций по идентификаторам.
    /// </summary>
    internal Dictionary<int, Specialization> Specializations { get; }

    /// <summary>
    /// Коллекция ветеринаров по идентификаторам.
    /// </summary>
    internal Dictionary<int, Veterinarian> Veterinarians { get; }

    /// <summary>
    /// Коллекция приёмов по идентификаторам.
    /// </summary>
    internal Dictionary<int, Appointment> Appointments { get; }

    /// <summary>
    /// Заполняет хранилище независимым набором исходных данных.
    /// </summary>
    public ClinicStore()
    {
        var data = ClinicSeed.Create();

        Breeds = data.Breeds.ToDictionary(entity => entity.Id);
        Owners = data.Owners.ToDictionary(entity => entity.Id);
        Pets = data.Pets.ToDictionary(entity => entity.Id);
        Specializations = data.Specializations.ToDictionary(entity => entity.Id);
        Veterinarians = data.Veterinarians.ToDictionary(entity => entity.Id);
        Appointments = data.Appointments.ToDictionary(entity => entity.Id);
    }
}
