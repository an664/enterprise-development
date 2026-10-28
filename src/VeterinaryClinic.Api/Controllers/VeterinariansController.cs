using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.Application.Services;
using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Specializations;
using VeterinaryClinic.Contracts.Veterinarians;

namespace VeterinaryClinic.Api.Controllers;

/// <summary>
/// Управление данными ветеринаров.
/// </summary>
/// <param name="service">Служба работы с данными ветеринаров.</param>
[ApiController]
[Route("api/veterinarians")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class VeterinariansController(IVeterinarianService service) : ControllerBase
{
    /// <summary>
    /// Возвращает список ветеринаров.
    /// </summary>
    /// <returns>Список ветеринаров.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<VeterinarianDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<VeterinarianDto>> GetAll()
    {
        return Ok(service.GetAll());
    }

    /// <summary>
    /// Возвращает данные ветеринара по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VeterinarianDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<VeterinarianDto> GetById([Range(1, int.MaxValue)] int id)
    {
        return Ok(service.GetById(id));
    }

    /// <summary>
    /// Создаёт запись ветеринара.
    /// </summary>
    /// <param name="request">Данные новой записи.</param>
    /// <returns>Созданная запись и ссылка на неё.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(VeterinarianDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<VeterinarianDto> Create([FromBody] VeterinarianRequest request)
    {
        var created = service.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Изменяет данные ветеринара.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="request">Полный набор новых данных.</param>
    /// <returns>Подтверждение изменения без тела ответа.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult Update([Range(1, int.MaxValue)] int id, [FromBody] VeterinarianRequest request)
    {
        service.Update(id, request);
        return NoContent();
    }

    /// <summary>
    /// Удаляет запись ветеринара, если она существует.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Подтверждение удаления без тела ответа.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public ActionResult Delete([Range(1, int.MaxValue)] int id)
    {
        service.Delete(id);
        return NoContent();
    }

    /// <summary>
    /// Возвращает приёмы ветеринара.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    [HttpGet("{id:int}/appointments")]
    [ProducesResponseType(typeof(IReadOnlyList<AppointmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<IReadOnlyList<AppointmentDto>> GetAppointments([Range(1, int.MaxValue)] int id)
    {
        return Ok(service.GetAppointments(id));
    }

    /// <summary>
    /// Возвращает специализацию ветеринара.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    [HttpGet("{id:int}/specialization")]
    [ProducesResponseType(typeof(SpecializationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<SpecializationDto> GetSpecialization([Range(1, int.MaxValue)] int id)
    {
        return Ok(service.GetSpecialization(id));
    }
}
