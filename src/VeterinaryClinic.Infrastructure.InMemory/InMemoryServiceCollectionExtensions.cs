using Microsoft.Extensions.DependencyInjection;
using VeterinaryClinic.Domain.Data;
using VeterinaryClinic.Domain.Repositories;
using VeterinaryClinic.Infrastructure.InMemory.Repositories;

namespace VeterinaryClinic.Infrastructure.InMemory;

/// <summary>
/// Регистрирует репозитории ветеринарной клиники с хранением в памяти.
/// </summary>
public static class InMemoryServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Создаёт связанные начальные записи и регистрирует по одному репозиторию каждого типа.
        /// </summary>
        /// <returns>Коллекция с зарегистрированными репозиториями.</returns>
        public IServiceCollection AddInMemoryRepositories()
        {
            var data = ClinicSeed.Create();
            var owners = data.Owners.ToDictionary(record => record.Id);
            var breeds = data.Breeds.ToDictionary(record => record.Id);
            var specializations = data.Specializations.ToDictionary(record => record.Id);
            var veterinarians = data.Veterinarians.ToDictionary(record => record.Id);
            var pets = data.Pets.ToDictionary(record => record.Id);
            var appointments = data.Appointments.ToDictionary(record => record.Id);

            services.AddSingleton<IOwnerRepository>(
                new OwnerRepository(owners, pets));
            services.AddSingleton<IBreedRepository>(
                new BreedRepository(breeds, pets));
            services.AddSingleton<ISpecializationRepository>(
                new SpecializationRepository(specializations, veterinarians));
            services.AddSingleton<IVeterinarianRepository>(
                new VeterinarianRepository(specializations, veterinarians, appointments));
            services.AddSingleton<IPetRepository>(
                new PetRepository(owners, breeds, pets, appointments));
            services.AddSingleton<IAppointmentRepository>(
                new AppointmentRepository(owners, breeds, specializations, veterinarians, pets, appointments));
            return services;
        }
    }
}
