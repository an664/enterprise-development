using Microsoft.Extensions.DependencyInjection;
using VeterinaryClinic.Application.Services;

namespace VeterinaryClinic.Application;

/// <summary>
/// Регистрация служб ветеринарной клиники.
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует службы работы с данными клиники и аналитических запросов.
        /// </summary>
        /// <returns>Коллекция служб приложения.</returns>
        public IServiceCollection AddClinicServices()
        {
            services.AddScoped<IOwnerService, OwnerService>();
            services.AddScoped<IBreedService, BreedService>();
            services.AddScoped<IPetService, PetService>();
            services.AddScoped<ISpecializationService, SpecializationService>();
            services.AddScoped<IVeterinarianService, VeterinarianService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            return services;
        }
    }
}
