using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VeterinaryClinic.Domain.Repositories;
using VeterinaryClinic.Infrastructure.PostgreSQL.Repositories;

namespace VeterinaryClinic.Infrastructure.PostgreSQL;

/// <summary>
/// Подключение репозиториев ветеринарной клиники к PostgreSQL.
/// </summary>
public static class PostgreSqlHostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Регистрирует общий для запроса контекст и репозитории сущностей.
        /// </summary>
        /// <param name="connectionName">Имя строки подключения к базе клиники.</param>
        /// <returns>Построитель приложения.</returns>
        public IHostApplicationBuilder AddPostgreSqlRepositories(string connectionName)
        {
            builder.AddNpgsqlDbContext<ClinicDbContext>(connectionName, settings => settings.DisableRetry = true);
            builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
            builder.Services.AddScoped<IBreedRepository, BreedRepository>();
            builder.Services.AddScoped<IPetRepository, PetRepository>();
            builder.Services.AddScoped<ISpecializationRepository, SpecializationRepository>();
            builder.Services.AddScoped<IVeterinarianRepository, VeterinarianRepository>();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            return builder;
        }
    }
}
