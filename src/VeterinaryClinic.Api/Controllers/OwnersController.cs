using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.Application.Services;
using VeterinaryClinic.Contracts.Owners;
using VeterinaryClinic.Contracts.Pets;

namespace VeterinaryClinic.Api.Controllers;

/// <summary>
/// Управление данными владельцев.
/// </summary>
/// <param name="service">Служба работы с данными владельцев.</param>
[ApiController]
[Route("api/owners")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class OwnersController(IOwnerService service) : ControllerBase
{
    /// <summary>
    /// Возвращает список владельцев.
    /// </summary>
    /// <returns>Список владельцев.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OwnerDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<OwnerDto>> GetAll()
    {
        return Ok(service.GetAll());
    }

    /// <summary>
    /// Возвращает данные владельца по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OwnerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<OwnerDto> GetById([Range(1, int.MaxValue)] int id)
    {
        return Ok(service.GetById(id));
    }

    /// <summary>
    /// Создаёт запись владельца.
    /// </summary>
    /// <param name="request">Данные новой записи.</param>
    /// <returns>Созданная запись и ссылка на неё.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(OwnerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<OwnerDto> Create([FromBody] OwnerRequest request)
    {
        var created = service.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Изменяет данные владельца.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="request">Полный набор новых данных.</param>
    /// <returns>Подтверждение изменения без тела ответа.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult Update([Range(1, int.MaxValue)] int id, [FromBody] OwnerRequest request)
    {
        service.Update(id, request);
        return NoContent();
    }

    /// <summary>
    /// Удаляет запись владельца, если она существует.
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
    /// Возвращает питомцев владельца.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    [HttpGet("{id:int}/pets")]
    [ProducesResponseType(typeof(IReadOnlyList<PetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<IReadOnlyList<PetDto>> GetPets([Range(1, int.MaxValue)] int id)
    {
        return Ok(service.GetPets(id));
    }
}
