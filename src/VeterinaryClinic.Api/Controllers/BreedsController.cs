using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.Application.Services;
using VeterinaryClinic.Contracts.Breeds;
using VeterinaryClinic.Contracts.Pets;
using VeterinaryClinic.Shared;

namespace VeterinaryClinic.Api.Controllers;

/// <summary>
/// Управление данными пород.
/// </summary>
/// <param name="service">Служба работы с данными пород.</param>
[ApiController]
[Route("api/breeds")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class BreedsController(IBreedService service) : ControllerBase
{
    /// <summary>
    /// Возвращает список пород.
    /// </summary>
    /// <returns>Список пород.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BreedDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<BreedDto>> GetAll()
    {
        return Ok(service.GetAll());
    }

    /// <summary>
    /// Возвращает данные породы по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <returns>Результат операции.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BreedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<BreedDto> GetById([Range(1, int.MaxValue)] int id)
    {
        return Ok(service.GetById(id));
    }

    /// <summary>
    /// Создаёт запись породы.
    /// </summary>
    /// <param name="request">Данные новой записи.</param>
    /// <returns>Созданная запись и ссылка на неё.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BreedDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<BreedDto> Create([FromBody] BreedRequest request)
    {
        var created = service.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Изменяет данные породы.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="request">Полный набор новых данных.</param>
    /// <returns>Подтверждение изменения без тела ответа.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult Update([Range(1, int.MaxValue)] int id, [FromBody] BreedRequest request)
    {
        service.Update(id, request);
        return NoContent();
    }

    /// <summary>
    /// Удаляет запись породы, если она существует.
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
    /// Возвращает питомцев выбранной породы.
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

    /// <summary>
    /// Возвращает породы выбранного вида животных.
    /// </summary>
    /// <param name="species">Вид животного.</param>
    /// <returns>Породы выбранного вида.</returns>
    [HttpGet("by-species/{species}")]
    [ProducesResponseType(typeof(IReadOnlyList<BreedDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<IReadOnlyList<BreedDto>> GetBySpecies(
        [EnumDataType(typeof(AnimalSpecies))] AnimalSpecies species)
    {
        return Ok(service.GetBySpecies(species));
    }
}
