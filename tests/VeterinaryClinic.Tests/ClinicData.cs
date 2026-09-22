using VeterinaryClinic.Models;

namespace VeterinaryClinic.Tests;

/// <summary>
/// Набор данных ветеринарной клиники для тестов.
/// </summary>
/// <param name="Breeds">Породы животных.</param>
/// <param name="Owners">Владельцы питомцев.</param>
/// <param name="Pets">Питомцы клиники.</param>
/// <param name="Specializations">Специализации врачей.</param>
/// <param name="Veterinarians">Ветеринарные врачи.</param>
/// <param name="Appointments">Записи на приём.</param>
internal sealed record ClinicData(
    IReadOnlyList<Breed> Breeds,
    IReadOnlyList<Owner> Owners,
    IReadOnlyList<Pet> Pets,
    IReadOnlyList<Specialization> Specializations,
    IReadOnlyList<Veterinarian> Veterinarians,
    IReadOnlyList<Appointment> Appointments);
