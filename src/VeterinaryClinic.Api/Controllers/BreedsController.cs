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
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Список пород.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BreedDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BreedDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await service.GetAllAsync(cancellationToken));
    }

    /// <summary>
    /// Возвращает данные породы по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BreedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BreedDto>> GetById(
        [Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        return Ok(await service.GetByIdAsync(id, cancellationToken));
    }

    /// <summary>
    /// Создаёт запись породы.
    /// </summary>
    /// <param name="request">Данные новой записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Созданная запись и ссылка на неё.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BreedDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BreedDto>> Create(
        [FromBody] BreedRequest request,
        CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Изменяет данные породы.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="request">Полный набор новых данных.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Подтверждение изменения без тела ответа.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(
        [Range(1, int.MaxValue)] int id,
        [FromBody] BreedRequest request,
        CancellationToken cancellationToken)
    {
        await service.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Удаляет запись породы, если она существует.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Подтверждение удаления без тела ответа.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Delete([Range(1, int.MaxValue)] int id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Возвращает питомцев выбранной породы.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    [HttpGet("{id:int}/pets")]
    [ProducesResponseType(typeof(IReadOnlyList<PetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<PetDto>>> GetPets(
        [Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        return Ok(await service.GetPetsAsync(id, cancellationToken));
    }

    /// <summary>
    /// Возвращает породы выбранного вида животных.
    /// </summary>
    /// <param name="species">Вид животного.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Породы выбранного вида.</returns>
    [HttpGet("by-species/{species}")]
    [ProducesResponseType(typeof(IReadOnlyList<BreedDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<BreedDto>>> GetBySpecies(
        [EnumDataType(typeof(AnimalSpecies))] AnimalSpecies species,
        CancellationToken cancellationToken)
    {
        return Ok(await service.GetBySpeciesAsync(species, cancellationToken));
    }
}
