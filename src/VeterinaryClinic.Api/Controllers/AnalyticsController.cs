using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VeterinaryClinic.Application.Services;
using VeterinaryClinic.Contracts.Analytics;
using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Owners;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Contracts.Veterinarians;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Api.Controllers;

/// <summary>
/// Аналитические запросы ветеринарной клиники.
/// </summary>
/// <param name="service">Служба аналитических запросов.</param>
[ApiController]
[Route("api/analytics")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class AnalyticsController(IAnalyticsService service) : ControllerBase
{
    /// <summary>
    /// Возвращает врачей, специализирующихся на выбранном виде животных.
    /// </summary>
    /// <param name="species">Вид животных.</param>
    /// <returns>Подходящие врачи.</returns>
    [HttpGet("veterinarians")]
    [ProducesResponseType(typeof(IReadOnlyList<VeterinarianDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<IReadOnlyList<VeterinarianDto>> GetVeterinarians(
        [FromQuery, BindRequired, EnumDataType(typeof(AnimalSpecies))] AnimalSpecies species)
    {
        return Ok(service.GetVeterinarians(species));
    }

    /// <summary>
    /// Возвращает питомцев выбранного врача без повторов, по кличке.
    /// </summary>
    /// <param name="veterinarianId">Идентификатор врача.</param>
    /// <returns>Питомцы врача.</returns>
    [HttpGet("pets")]
    [ProducesResponseType(typeof(IReadOnlyList<PetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<IReadOnlyList<PetDto>> GetPets(
        [FromQuery, BindRequired, Range(1, int.MaxValue)] int veterinarianId)
    {
        return Ok(service.GetPets(veterinarianId));
    }

    /// <summary>
    /// Считает повторные приёмы питомцев выбранной породы.
    /// </summary>
    /// <param name="breedId">Идентификатор породы.</param>
    /// <returns>Количество повторных приёмов.</returns>
    [HttpGet("follow-up-count")]
    [ProducesResponseType(typeof(FollowUpCountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<FollowUpCountDto> GetFollowUpCount(
        [FromQuery, BindRequired, Range(1, int.MaxValue)] int breedId)
    {
        return Ok(service.GetFollowUpCount(breedId));
    }

    /// <summary>
    /// Возвращает владельцев нескольких питомцев, по ФИО.
    /// </summary>
    /// <returns>Владельцы более одного питомца.</returns>
    [HttpGet("owners-with-multiple-pets")]
    [ProducesResponseType(typeof(IReadOnlyList<OwnerDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<OwnerDto>> GetOwnersWithMultiplePets()
    {
        return Ok(service.GetOwnersWithMultiplePets());
    }

    /// <summary>
    /// Возвращает приёмы кабинета за месяц от выбранной даты, по времени.
    /// </summary>
    /// <param name="asOfDate">Начальная дата, включаемая в выборку.</param>
    /// <param name="roomNumber">Номер кабинета.</param>
    /// <returns>Приёмы до той же даты следующего месяца, не включая её.</returns>
    [HttpGet("appointments")]
    [ProducesResponseType(typeof(IReadOnlyList<AppointmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<IReadOnlyList<AppointmentDto>> GetAppointments(
        [FromQuery, BindRequired] DateOnly asOfDate,
        [FromQuery, Required, StringLength(20)] string roomNumber)
    {
        return Ok(service.GetAppointments(asOfDate, roomNumber));
    }
}
