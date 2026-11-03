using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VeterinaryClinic.Domain.Repositories;
using VeterinaryClinic.Infrastructure.PostgreSQL.Repositories;

namespace VeterinaryClinic.Infrastructure.PostgreSQL;

/// <summary>
/// Подключение репозиториев ветеринарной клиники к PostgreSQL.
/// </summary>
public static class PostgreSqlServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует общий для запроса контекст и репозитории сущностей.
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе клиники.</param>
        /// <returns>Коллекция служб приложения.</returns>
        public IServiceCollection AddPostgreSqlRepositories(string connectionString)
        {
            services.AddDbContext<ClinicDbContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<IOwnerRepository, OwnerRepository>();
            services.AddScoped<IBreedRepository, BreedRepository>();
            services.AddScoped<IPetRepository, PetRepository>();
            services.AddScoped<ISpecializationRepository, SpecializationRepository>();
            services.AddScoped<IVeterinarianRepository, VeterinarianRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            return services;
        }
    }
}
