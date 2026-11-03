namespace VeterinaryClinic.Domain.Exceptions;

/// <summary>
/// Запрошенная запись не найдена.
/// </summary>
/// <param name="message">Описание причины отказа.</param>
public class EntityNotFoundException(string message) : Exception(message);
