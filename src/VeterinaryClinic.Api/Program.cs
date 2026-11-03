using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Api.Errors;
using VeterinaryClinic.Api.Filters;
using VeterinaryClinic.Application;
using VeterinaryClinic.Infrastructure.InMemory;
using VeterinaryClinic.Infrastructure.PostgreSQL;
using VeterinaryClinic.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var provider = builder.Configuration["Storage:Provider"] ?? "PostgreSQL";
switch (provider)
{
    case "InMemory":
        builder.Services.AddInMemoryRepositories();
        break;
    case "PostgreSQL":
        var connectionString = builder.Configuration.GetConnectionString("clinic")
            ?? throw new InvalidOperationException("Для подключения к PostgreSQL запустите VeterinaryClinic.AppHost.");
        builder.Services.AddPostgreSqlRepositories(connectionString);
        builder.Services.AddScoped<WriteTransactionFilter>();
        break;
    default:
        throw new InvalidOperationException($"Неизвестное хранилище: {provider}.");
}

builder.Services.AddClinicServices();
builder.Services.AddScoped<RequestLoggingFilter>();
builder.Services.AddControllers(options =>
    {
        options.Filters.AddService<RequestLoggingFilter>(int.MinValue);
        if (provider == "PostgreSQL")
        {
            options.Filters.AddService<WriteTransactionFilter>();
        }
    })
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
if (provider == "PostgreSQL")
{
    await using var scope = app.Services.CreateAsyncScope();
    var database = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();
    await database.Database.MigrateAsync();
}

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Veterinary Clinic API v1"));
}
app.MapControllers();
app.MapDefaultEndpoints();
app.Run();

/// <summary>
/// Точка входа API ветеринарной клиники.
/// </summary>
public partial class Program
{
}
