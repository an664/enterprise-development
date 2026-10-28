using System.Text.Json.Serialization;
using VeterinaryClinic.Api.Errors;
using VeterinaryClinic.Api.Filters;
using VeterinaryClinic.Application.Services;
using VeterinaryClinic.Data;
using VeterinaryClinic.Infrastructure;
using VeterinaryClinic.Infrastructure.Repositories;
using VeterinaryClinic.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ClinicLock>();
builder.Services.AddSingleton<ClinicStore>();
builder.Services.AddSingleton<IOwnerRepository, OwnerRepository>();
builder.Services.AddSingleton<IBreedRepository, BreedRepository>();
builder.Services.AddSingleton<IPetRepository, PetRepository>();
builder.Services.AddSingleton<ISpecializationRepository, SpecializationRepository>();
builder.Services.AddSingleton<IVeterinarianRepository, VeterinarianRepository>();
builder.Services.AddSingleton<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IBreedService, BreedService>();
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<ISpecializationService, SpecializationService>();
builder.Services.AddScoped<IVeterinarianService, VeterinarianService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<RequestLoggingFilter>();
builder.Services.AddControllers(options => options.Filters.AddService<RequestLoggingFilter>(int.MinValue))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
    });
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
});
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Veterinary Clinic API v1"));
}
app.MapControllers();
app.Run();

/// <summary>
/// Точка входа API ветеринарной клиники.
/// </summary>
public partial class Program
{
}
