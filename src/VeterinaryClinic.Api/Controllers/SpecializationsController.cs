using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.Application.Services;
using VeterinaryClinic.Contracts.Specializations;
using VeterinaryClinic.Contracts.Veterinarians;

namespace VeterinaryClinic.Api.Controllers;

/// <summary>
/// Управление данными специализаций.
/// </summary>
/// <param name="service">Служба работы с данными специализаций.</param>
[ApiController]
[Route("api/specializations")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class SpecializationsController(ISpecializationService service) : ControllerBase
{
    /// <summary>
    /// Возвращает список специализаций.
    /// </summary>
    /// <returns>Список специализаций.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SpecializationDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<SpecializationDto>> GetAll()
    {
        return Ok(service.GetAll());
    }

    /// <summary>
    /// Возвращает данные специализации по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SpecializationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<SpecializationDto> GetById([Range(1, int.MaxValue)] int id)
    {
        return Ok(service.GetById(id));
    }

    /// <summary>
    /// Создаёт запись специализации.
    /// </summary>
    /// <param name="request">Данные новой записи.</param>
    /// <returns>Созданная запись и ссылка на неё.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SpecializationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<SpecializationDto> Create([FromBody] SpecializationRequest request)
    {
        var created = service.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Изменяет данные специализации.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="request">Полный набор новых данных.</param>
    /// <returns>Подтверждение изменения без тела ответа.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult Update([Range(1, int.MaxValue)] int id, [FromBody] SpecializationRequest request)
    {
        service.Update(id, request);
        return NoContent();
    }

    /// <summary>
    /// Удаляет запись специализации, если она существует.
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
    /// Возвращает врачей выбранной специализации.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    [HttpGet("{id:int}/veterinarians")]
    [ProducesResponseType(typeof(IReadOnlyList<VeterinarianDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<IReadOnlyList<VeterinarianDto>> GetVeterinarians([Range(1, int.MaxValue)] int id)
    {
        return Ok(service.GetVeterinarians(id));
    }
}
