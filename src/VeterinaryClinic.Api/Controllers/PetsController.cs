using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.Application.Services;
using VeterinaryClinic.Contracts.Appointments;
using VeterinaryClinic.Contracts.Breeds;
using VeterinaryClinic.Contracts.Owners;
using VeterinaryClinic.Contracts.Pets;

namespace VeterinaryClinic.Api.Controllers;

/// <summary>
/// Управление данными питомцев.
/// </summary>
/// <param name="service">Служба работы с данными питомцев.</param>
[ApiController]
[Route("api/pets")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class PetsController(IPetService service) : ControllerBase
{
    /// <summary>
    /// Возвращает список питомцев.
    /// </summary>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Список питомцев.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PetDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await service.GetAllAsync(cancellationToken));
    }

    /// <summary>
    /// Возвращает данные питомца по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PetDto>> GetById(
        [Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        return Ok(await service.GetByIdAsync(id, cancellationToken));
    }

    /// <summary>
    /// Создаёт запись питомца.
    /// </summary>
    /// <param name="request">Данные новой записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Созданная запись и ссылка на неё.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PetDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PetDto>> Create(
        [FromBody] PetRequest request,
        CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Изменяет данные питомца.
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
        [FromBody] PetRequest request,
        CancellationToken cancellationToken)
    {
        await service.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Удаляет запись питомца, если она существует.
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
    /// Возвращает приёмы питомца.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    [HttpGet("{id:int}/appointments")]
    [ProducesResponseType(typeof(IReadOnlyList<AppointmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<AppointmentDto>>> GetAppointments(
        [Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        return Ok(await service.GetAppointmentsAsync(id, cancellationToken));
    }

    /// <summary>
    /// Возвращает владельца питомца.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    [HttpGet("{id:int}/owner")]
    [ProducesResponseType(typeof(OwnerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OwnerDto>> GetOwner(
        [Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        return Ok(await service.GetOwnerAsync(id, cancellationToken));
    }

    /// <summary>
    /// Возвращает породу питомца.
    /// </summary>
    /// <param name="id">Идентификатор записи.</param>
    /// <param name="cancellationToken">Сигнал отмены запроса.</param>
    /// <returns>Результат операции.</returns>
    [HttpGet("{id:int}/breed")]
    [ProducesResponseType(typeof(BreedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BreedDto>> GetBreed(
        [Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        return Ok(await service.GetBreedAsync(id, cancellationToken));
    }
}
